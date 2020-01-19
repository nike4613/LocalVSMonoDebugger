using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugger.Soft;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000046 RID: 70
	public class Process : IDebugProcess2, IDebugProgram3, IDebugProgram2, IDebugProgramNode2, IThreadingAdapter
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00005B85 File Offset: 0x00003D85
		// (set) Token: 0x0600015E RID: 350 RVA: 0x00005B8D File Offset: 0x00003D8D
		public Port Port { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00005B96 File Offset: 0x00003D96
		// (set) Token: 0x06000160 RID: 352 RVA: 0x00005B9E File Offset: 0x00003D9E
		public Engine Engine { get; private set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00005BA7 File Offset: 0x00003DA7
		// (set) Token: 0x06000162 RID: 354 RVA: 0x00005BAF File Offset: 0x00003DAF
		public SoftDebuggerSession Session { get; private set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00005BB8 File Offset: 0x00003DB8
		// (set) Token: 0x06000164 RID: 356 RVA: 0x00005BC0 File Offset: 0x00003DC0
		public bool Alive { get; private set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00005BC9 File Offset: 0x00003DC9
		// (set) Token: 0x06000166 RID: 358 RVA: 0x00005BD1 File Offset: 0x00003DD1
		internal BreakpointsAdapter Breakpoints { get; private set; }

		// Token: 0x06000167 RID: 359 RVA: 0x00005BDC File Offset: 0x00003DDC
		internal Process(Port port, Engine engine, IEventSender eventSender, SoftDebuggerSession session, StartInfo startInfo, string name)
		{
			this.guid = Guid.NewGuid();
			this.Port = port;
			this.Engine = engine;
			this.Session = session;
			this.name = name;
			this.startInfo = startInfo;
			this.eventSender = eventSender;
			session.TargetReady += this.OnTargetReady;
			session.TargetExited += new EventHandler<TargetEventArgs>(this.OnTargetExited);
			session.TargetInterrupted += new EventHandler<TargetEventArgs>(this.OnTargetExited);
			session.TargetStopped += this.OnTargetPaused;
			session.TargetThreadStarted += this.OnThreadStart;
			session.TargetThreadStopped += this.OnThreadDeath;
			this.exceptions = new ExceptionsAdapter(this, eventSender, session);
			this.typeResolver = new TypeResolverAdapter(session);
			this.Breakpoints = new BreakpointsAdapter(this, eventSender, session);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00005CD4 File Offset: 0x00003ED4
		private void OnTargetReady(object sender, TargetEventArgs args)
		{
			Thread thread = new Thread(this, args.Thread.Id, this.eventSender);
			this.AddThread(args.Thread.Id, thread);
			this.eventSender.SendEvent(new ProgramCreateEvent(this));
			this.eventSender.SendEvent(new ThreadCreateEvent(thread));
			this.eventSender.SendEvent(new LoadCompleteEvent(thread));
			this.Alive = true;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00005D45 File Offset: 0x00003F45
		protected virtual void OnTargetExited(object sender, EventArgs args)
		{
			this.Terminate();
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00005D50 File Offset: 0x00003F50
		private void OnTargetPaused(object sender, TargetEventArgs args)
		{
			try
			{
				Thread threadFromTargetArgs = this.GetThreadFromTargetArgs("OnTargetPaused()", args);
				this.eventSender.SendEvent(new StepCompleteEvent(threadFromTargetArgs));
			}
			catch (Exception ex)
			{
				Utils.Message("OnTargetPaused()", new object[]
				{
					ex
				});
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00005DA4 File Offset: 0x00003FA4
		private void OnThreadStart(object sender, TargetEventArgs args)
		{
			if (!this.thread_hash.ContainsKey(args.Thread.Id))
			{
				Thread thread = new Thread(this, args.Thread.Id, this.eventSender);
				this.thread_hash.Add(args.Thread.Id, thread);
				this.eventSender.SendEvent(new ThreadCreateEvent(thread));
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00005E0C File Offset: 0x0000400C
		private void OnThreadDeath(object sender, TargetEventArgs args)
		{
			Thread thread;
			if (this.thread_hash.TryGetValue(args.Thread.Id, out thread))
			{
				thread.NotifyDeath();
				this.thread_hash.Remove(args.Thread.Id);
				this.eventSender.SendEvent(new ThreadDestroyEvent(thread, 0U));
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00005E64 File Offset: 0x00004064
		public Thread GetThreadFromTargetArgs(string caller, TargetEventArgs args)
		{
			Thread thread = null;
			if (args.Thread == null)
			{
				Utils.Message("GetThreadFromTargetArgs({0}): args.Thread == null", new object[]
				{
					caller
				});
			}
			else if (this.thread_hash.ContainsKey(args.Thread.Id))
			{
				thread = this.thread_hash[args.Thread.Id];
			}
			else if (args.Thread.Id > 0L)
			{
				Utils.Message("GetThreadFromTargetArgs({0}): args.Thread.Id ({1}) not in thread array.", new object[]
				{
					caller,
					args.Thread.Id
				});
				long id = args.Thread.Id;
				thread = new Thread(this, id, this.eventSender);
				this.thread_hash.Add(id, thread);
				this.eventSender.SendEvent(new ThreadCreateEvent(thread));
				Utils.Message("GetThreadFromTargetArgs({0}): Sent fake thread create event: {1}", new object[]
				{
					caller,
					id
				});
			}
			return thread;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00005F5A File Offset: 0x0000415A
		public void AddThread(long id, Thread main_thread)
		{
			this.thread_hash.Add(id, main_thread);
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00005F6C File Offset: 0x0000416C
		public void SendEvent(PortEvent e)
		{
			Guid id = e.ID;
			this.Port.PortEventsCP.Event(this.Port.PortSupplier.Server, this.Port, this, this, e, ref id);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00004B88 File Offset: 0x00002D88
		public int Attach(IDebugEventCallback2 pCallback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00004E91 File Offset: 0x00003091
		public int CanDetach()
		{
			return 1;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00005FAB File Offset: 0x000041AB
		public int CauseBreak()
		{
			this.Session.Stop();
			return 0;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00005FB9 File Offset: 0x000041B9
		public int Continue(IDebugThread2 t)
		{
			this.Session.Continue();
			return 0;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00004B88 File Offset: 0x00002D88
		public int Detach()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00004B88 File Offset: 0x00002D88
		public int EnumCodeContexts(IDebugDocumentPosition2 pDocPos, out IEnumDebugCodeContexts2 ppEnum)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00004B88 File Offset: 0x00002D88
		public int EnumCodePaths(string pszHint, IDebugCodeContext2 pStart, IDebugStackFrame2 pFrame, int fSource, out IEnumCodePaths2 ppEnum, out IDebugCodeContext2 ppSafety)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00005FC8 File Offset: 0x000041C8
		public int EnumModules(out IEnumDebugModules2 ppEnum)
		{
			AssemblyMirror[] assemblies = this.Session.VirtualMachine.RootDomain.GetAssemblies();
			ppEnum = new AD7ModuleEnum((from a in assemblies
			select new AD7Module(a, this.startInfo)).ToArray<AD7Module>());
			return 0;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000600A File Offset: 0x0000420A
		public int EnumPrograms(out IEnumDebugPrograms2 ppEnum)
		{
			ppEnum = new AD7ProgramEnum(new IDebugProgram2[]
			{
				this
			});
			return 0;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00006020 File Offset: 0x00004220
		public int EnumThreads(out IEnumDebugThreads2 ppEnum)
		{
			ThreadInfo[] threads = this.Session.GetProcesses()[0].GetThreads();
			ppEnum = new AD7ThreadEnum(from t in this.thread_hash.Values
			where threads.Any((ThreadInfo th) => th.Id == t.ID)
			select t);
			return 0;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00005FB9 File Offset: 0x000041B9
		public int ExecuteOnThread(IDebugThread2 t)
		{
			this.Session.Continue();
			return 0;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetDebugProperty(out IDebugProperty2 ppProperty)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetDisassemblyStream(enum_DISASSEMBLY_STREAM_SCOPE dwScope, IDebugCodeContext2 pCodeContext, out IDebugDisassemblyStream2 ppDisassemblyStream)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetENCUpdate(out object ppUpdate)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000606F File Offset: 0x0000426F
		public int GetEngineInfo(out string name, out Guid guid)
		{
			name = this.Engine.Name;
			guid = this.Engine.Guid;
			return 0;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00006090 File Offset: 0x00004290
		public int GetName(out string name)
		{
			name = this.name;
			return 0;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000609B File Offset: 0x0000429B
		public int GetProgramId(out Guid pguidProgramId)
		{
			pguidProgramId = this.guid;
			return 0;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x000060AC File Offset: 0x000042AC
		public int Step(IDebugThread2 t, enum_STEPKIND u_kind, enum_STEPUNIT u_step)
		{
			Thread thread = (Thread)t;
			if (u_step == enum_STEPUNIT.STEP_INSTRUCTION)
			{
				Utils.Message("STEP INSTRUCTION !");
			}
			switch (u_kind)
			{
			case enum_STEPKIND.STEP_INTO:
				this.Session.StepLine();
				break;
			case enum_STEPKIND.STEP_OVER:
				this.Session.NextLine();
				break;
			case enum_STEPKIND.STEP_OUT:
				this.Session.Finish();
				break;
			default:
				return 1;
			}
			return 0;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00006118 File Offset: 0x00004318
		public int Terminate()
		{
			try
			{
				lock (this)
				{
					if (this.Alive)
					{
						this.Session.Exit();
						this.Alive = false;
					}
				}
			}
			finally
			{
				this.eventSender.SendEvent(new ProcessDestroyEvent(this));
				this.eventSender.SendEvent(new ProgramDestroyEvent(this, 0U));
				this.SendEvent(new PortProgramDestroyEvent(0U));
			}
			return 0;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00004B88 File Offset: 0x00002D88
		public int WriteDump(enum_DUMPTYPE DUMPTYPE, string pszDumpUrl)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00004B88 File Offset: 0x00002D88
		public int Attach(IDebugEventCallback2 pCallback, Guid[] rgguidSpecificEngines, uint celtSpecificEngines, int[] rghrEngineAttach)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000186 RID: 390 RVA: 0x000059EB File Offset: 0x00003BEB
		public int GetAttachedSessionName(out string pbstrSessionName)
		{
			pbstrSessionName = "Mono";
			return 0;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x000061A4 File Offset: 0x000043A4
		public int GetInfo(enum_PROCESS_INFO_FIELDS Fields, PROCESS_INFO[] processInfo)
		{
			processInfo[0].Fields = Fields;
			if (this.IsSet(Fields, enum_PROCESS_INFO_FIELDS.PIF_FILE_NAME))
			{
				processInfo[0].bstrFileName = this.name;
			}
			if (this.IsSet(Fields, enum_PROCESS_INFO_FIELDS.PIF_BASE_NAME))
			{
				processInfo[0].bstrBaseName = Path.GetFileName(this.name);
			}
			if (this.IsSet(Fields, enum_PROCESS_INFO_FIELDS.PIF_TITLE))
			{
				processInfo[0].bstrTitle = Path.GetFileName(this.name);
			}
			return 0;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000621D File Offset: 0x0000441D
		private bool IsSet(enum_PROCESS_INFO_FIELDS f, enum_PROCESS_INFO_FIELDS b)
		{
			return (f & b) > 0;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00006225 File Offset: 0x00004425
		public int GetName(enum_GETNAME_TYPE gnType, out string pbstrName)
		{
			pbstrName = this.name;
			return 0;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00006230 File Offset: 0x00004430
		public int GetPhysicalProcessId(AD_PROCESS_ID[] pProcessId)
		{
			pProcessId[0].ProcessIdType = 1U;
			pProcessId[0].guidProcessId = this.guid;
			return 0;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00006252 File Offset: 0x00004452
		public int GetPort(out IDebugPort2 outPort)
		{
			outPort = this.Port;
			return 0;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000609B File Offset: 0x0000429B
		public int GetProcessId(out Guid pguidProcessId)
		{
			pguidProcessId = this.guid;
			return 0;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetServer(out IDebugCoreServer2 ppServer)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00006230 File Offset: 0x00004430
		public int GetHostPid(AD_PROCESS_ID[] info)
		{
			info[0].ProcessIdType = 1U;
			info[0].guidProcessId = this.guid;
			return 0;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00006225 File Offset: 0x00004425
		public int GetHostName(enum_GETHOSTNAME_TYPE host_name_type, out string process_name)
		{
			process_name = this.name;
			return 0;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00006090 File Offset: 0x00004290
		public int GetProgramName(out string program_name)
		{
			program_name = this.name;
			return 0;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00004B88 File Offset: 0x00002D88
		int IDebugProgram2.Attach(IDebugEventCallback2 pCallback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000625D File Offset: 0x0000445D
		public int GetProcess(out IDebugProcess2 ppProcess)
		{
			ppProcess = this;
			return 0;
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00005FB9 File Offset: 0x000041B9
		public int Execute()
		{
			this.Session.Continue();
			return 0;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00004B88 File Offset: 0x00002D88
		int IDebugProgramNode2.Attach_V7(IDebugProgram2 pMDMProgram, IDebugEventCallback2 pCallback, uint dwReason)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00004B88 File Offset: 0x00002D88
		int IDebugProgramNode2.DetachDebugger_V7()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00004B88 File Offset: 0x00002D88
		int IDebugProgramNode2.GetHostMachineName_V7(out string pbstrHostMachineName)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040000A4 RID: 164
		private Guid guid;

		// Token: 0x040000A5 RID: 165
		public string name;

		// Token: 0x040000A9 RID: 169
		private Dictionary<long, Thread> thread_hash = new Dictionary<long, Thread>();

		// Token: 0x040000AC RID: 172
		private static int nextThreadId = 91000;

		// Token: 0x040000AD RID: 173
		private IEventSender eventSender;

		// Token: 0x040000AE RID: 174
		private StartInfo startInfo;

		// Token: 0x040000AF RID: 175
		private ExceptionsAdapter exceptions;

		// Token: 0x040000B0 RID: 176
		private TypeResolverAdapter typeResolver;
	}
}
