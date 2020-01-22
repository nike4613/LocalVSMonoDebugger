using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000010 RID: 16
	[ComVisible(true)]
	[Guid("C094C059-1786-49CD-8EB9-9C0EF6CA5454")]
	public class Engine : IDebugEngine2, IDebugEngineLaunch2
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000030F6 File Offset: 0x000012F6
		public Guid Guid
		{
			get
			{
				return Guids.EngineGuid;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000030FD File Offset: 0x000012FD
		public string Name
		{
			get
			{
				return "MonoVS.Engine";
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003104 File Offset: 0x00001304
		public int Attach(IDebugProgram2[] rgpPrograms, IDebugProgramNode2[] rgpProgramNodes, uint celtPrograms, IDebugEventCallback2 pCallback, enum_ATTACH_REASON dwReason)
		{
			if (dwReason != enum_ATTACH_REASON.ATTACH_REASON_USER)
			{
				throw new InvalidOperationException();
			}
			if (celtPrograms != 1U)
			{
				throw new InvalidOperationException();
			}
			if (!this.processes.Any((Process process) => rgpPrograms[0] == process))
			{
				throw new InvalidOperationException();
			}
			Utils.Message("ATTACH: {0}", new object[]
			{
				rgpPrograms[0]
			});
			return 0;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003170 File Offset: 0x00001370
		public int CauseBreak()
		{
			foreach (Process process in this.processes)
			{
				process.CauseBreak();
			}
			return 0;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000031C4 File Offset: 0x000013C4
		public int ContinueFromSynchronousEvent(IDebugEvent2 e)
		{
			((SynchronousEngineEvent)e).WaitHandle.Set();
			return 0;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000031D8 File Offset: 0x000013D8
		public int CreatePendingBreakpoint(IDebugBreakpointRequest2 pBPRequest, out IDebugPendingBreakpoint2 ppPendingBP)
		{
			ppPendingBP = null;
			foreach (Process process in this.processes)
			{
				PendingBreakpoint pendingBreakpoint = process.Breakpoints.CreatePendingBreakpoint(pBPRequest);
				if (pendingBreakpoint != null)
				{
					ppPendingBP = pendingBreakpoint;
				}
			}
			if (ppPendingBP == null)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003244 File Offset: 0x00001444
		public int DestroyProgram(IDebugProgram2 pProgram)
		{
			return 0;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003247 File Offset: 0x00001447
		public int GetEngineId(out Guid guid)
		{
			guid = this.Guid;
			return 0;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003258 File Offset: 0x00001458
		public int RemoveAllSetExceptions(ref Guid guidType)
		{
			if (guidType == Guid.Empty || guidType == Guids.guidLanguageCSharp)
			{
				foreach (Process process in this.processes)
				{
					process.Breakpoints.RemoveAllSetExceptions();
				}
			}
			return 0;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000032D4 File Offset: 0x000014D4
		public int RemoveSetException(EXCEPTION_INFO[] pException)
		{
			foreach (Process process in this.processes)
			{
				process.Breakpoints.RemoveException(pException);
			}
			return 0;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003330 File Offset: 0x00001530
		public int SetException(EXCEPTION_INFO[] pException)
		{
			if ((pException[0].dwState & enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE) != 0)
			{
				using (List<Process>.Enumerator enumerator = this.processes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Process process = enumerator.Current;
						process.Breakpoints.SetException(pException);
					}
					return 0;
				}
			}
			foreach (Process process2 in this.processes)
			{
				process2.Breakpoints.RemoveException(pException);
			}
			return 0;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003244 File Offset: 0x00001444
		public int SetLocale(ushort wLangID)
		{
			return 0;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003244 File Offset: 0x00001444
		public int SetMetric(string pszMetric, object varValue)
		{
			return 0;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003244 File Offset: 0x00001444
		public int SetRegistryRoot(string pszRegistryRoot)
		{
			return 0;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000033E4 File Offset: 0x000015E4
		public int EnumPrograms(out IEnumDebugPrograms2 ppEnum)
		{
			ppEnum = null;
			return 0;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003244 File Offset: 0x00001444
		public int CanTerminateProcess(IDebugProcess2 pProcess)
		{
			return 0;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000033EC File Offset: 0x000015EC
		public int LaunchSuspended(string pszServer, IDebugPort2 pPort, string pszExe, string pszArgs, string pszDir, string bstrEnv, string pszOptions, enum_LAUNCH_FLAGS dwLaunchFlags, uint hStdInput, uint hStdOutput, uint hStdError, IDebugEventCallback2 pCallback, out IDebugProcess2 ppProcess)
		{
			StartInfo startInfo;
			using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(pszOptions)))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				SessionMarshalling sessionMarshalling = (SessionMarshalling)binaryFormatter.Deserialize(memoryStream);
				this.session = sessionMarshalling.Session;
				startInfo = sessionMarshalling.StartInfo;
			}

			this.eventSender = new EventSender(pCallback, this);
			Process process = new Process((Port)pPort, this, this.eventSender, this.session, startInfo, pszExe);
			ppProcess = process;
			this.processes.Add(process);
			this.eventSender.SendEvent(new ProcessCreateEvent(process));
			return 0;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003498 File Offset: 0x00001698
		public int ResumeProcess(IDebugProcess2 process)
		{
			this.eventSender.SendEvent(new EngineCreateEvent(this));
			((Process)process).Port.AddProgramNode((Process)process);
			((Process)process).SendEvent(new PortProgramCreateEvent());
			return 0;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000034D3 File Offset: 0x000016D3
		public int TerminateProcess(IDebugProcess2 pProcess)
		{
			pProcess.Terminate();
			this.processes.Remove((Process)pProcess);
			return 0;
		}

		// Token: 0x04000018 RID: 24
		private List<Process> processes = new List<Process>();

		// Token: 0x04000019 RID: 25
		private SoftDebuggerSession session;

		// Token: 0x0400001A RID: 26
		private IEventSender eventSender;
	}
}
