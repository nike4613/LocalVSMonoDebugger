using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000037 RID: 55
	public class Thread : IDebugThread2
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00004D47 File Offset: 0x00002F47
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x00004D4F File Offset: 0x00002F4F
		public Process Process { get; private set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00004D58 File Offset: 0x00002F58
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x00004D60 File Offset: 0x00002F60
		public long ID { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00004D69 File Offset: 0x00002F69
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00004D71 File Offset: 0x00002F71
		public bool IsAlive { get; private set; }

		// Token: 0x060000FA RID: 250 RVA: 0x00004D7A File Offset: 0x00002F7A
		internal Thread(Process process, long id, IEventSender eventSender)
		{
			this.Process = process;
			this.ID = id;
			this.IsAlive = true;
			this.eventSender = eventSender;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00004D9E File Offset: 0x00002F9E
		internal void NotifyDeath()
		{
			this.IsAlive = false;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00004DA8 File Offset: 0x00002FA8
		private ThreadInfo GetThreadInfo()
		{
			if (!this.IsAlive)
			{
				throw new InvalidOperationException();
			}
			ThreadInfo[] threads = this.Process.Session.GetProcesses()[0].GetThreads();
			foreach (ThreadInfo threadInfo in threads)
			{
				if (threadInfo.Id == this.ID)
				{
					return threadInfo;
				}
			}
			throw new ArgumentException();
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00004E04 File Offset: 0x00003004
		public StackFrame GetFrame()
		{
			ThreadInfo threadInfo = this.GetThreadInfo();
			Backtrace backtrace = threadInfo.Backtrace;
			if (backtrace == null)
			{
				return null;
			}
			StackFrame frame = backtrace.GetFrame(0);
			if (frame == null)
			{
				return null;
			}
			return new StackFrame(this, frame, this.eventSender);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00004E40 File Offset: 0x00003040
		public StackFrame[] GetBacktrace()
		{
			ThreadInfo threadInfo = this.GetThreadInfo();
			Backtrace backtrace = threadInfo.Backtrace;
			if (backtrace == null)
			{
				return null;
			}
			StackFrame[] array = new StackFrame[backtrace.FrameCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new StackFrame(this, backtrace.GetFrame(i), this.eventSender);
			}
			return array;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00004E91 File Offset: 0x00003091
		public int CanSetNextStatement(IDebugStackFrame2 pStackFrame, IDebugCodeContext2 pCodeContext)
		{
			return 1;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetLogicalThread(IDebugStackFrame2 pStackFrame, out IDebugLogicalThread2 ppLogicalThread)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00004B88 File Offset: 0x00002D88
		public int Resume(out uint pdwSuspendCount)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00004B88 File Offset: 0x00002D88
		public int SetNextStatement(IDebugStackFrame2 pStackFrame, IDebugCodeContext2 pCodeContext)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00004B88 File Offset: 0x00002D88
		public int SetThreadName(string pszName)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00004B88 File Offset: 0x00002D88
		public int Suspend(out uint pdwSuspendCount)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00004E94 File Offset: 0x00003094
		public int EnumFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, out IEnumDebugFrameInfo2 ppEnum)
		{
			if (!this.IsAlive)
			{
				ppEnum = null;
				return -2147467259;
			}
			StackFrame[] backtrace = this.GetBacktrace();
			if (backtrace == null)
			{
				ppEnum = null;
				return 1;
			}
			List<FRAMEINFO> list = new List<FRAMEINFO>();
			if ((dwFieldSpec & 536870912) != null)
			{
				bool flag = false;
				for (int i = 0; i < backtrace.Length; i++)
				{
					if (backtrace[i].Frame.IsExternalCode || !backtrace[i].Frame.HasDebugInfo)
					{
						if (!flag)
						{
							list.Add(this.Create_External_Frame());
							flag = true;
						}
					}
					else
					{
						FRAMEINFO item = default(FRAMEINFO);
						backtrace[i].SetFrameInfo(dwFieldSpec, out item);
						list.Add(item);
					}
				}
			}
			else
			{
				for (int j = 0; j < backtrace.Length; j++)
				{
					FRAMEINFO item2 = default(FRAMEINFO);
					backtrace[j].SetFrameInfo(dwFieldSpec, out item2);
					list.Add(item2);
				}
			}
			ppEnum = new AD7FrameInfoEnum(list.ToArray());
			return 0;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00004F6C File Offset: 0x0000316C
		private FRAMEINFO Create_External_Frame()
		{
			FRAMEINFO result = default(FRAMEINFO);
			result.m_bstrFuncName = "[External Code]";
			result.m_dwValidFields |= 1;
			result.m_fHasDebugInfo = 0;
			result.m_dwValidFields |= 128;
			result.m_fStaleCode = 0;
			result.m_dwValidFields |= 256;
			result.m_dwValidFields |= 512;
			result.m_dwFlags = 0U;
			result.m_dwFlags |= 2U;
			result.m_dwFlags |= 1U;
			return result;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004FFC File Offset: 0x000031FC
		public int GetName(out string pbstrName)
		{
			if (!this.IsAlive)
			{
				pbstrName = string.Empty;
				return -2147467259;
			}
			try
			{
				pbstrName = this.GetThreadInfo().Name;
			}
			catch (ArgumentException)
			{
				pbstrName = "Unknown";
			}
			return 0;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000504C File Offset: 0x0000324C
		public int GetProgram(out IDebugProgram2 program)
		{
			program = this.Process;
			return 0;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00005057 File Offset: 0x00003257
		public int GetThreadId(out uint thread_id)
		{
			thread_id = (uint)this.ID;
			return 0;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00005064 File Offset: 0x00003264
		public int GetThreadProperties(enum_THREADPROPERTY_FIELDS fields, THREADPROPERTIES[] tp)
		{
			tp[0].dwFields = 0;
			if ((fields & 1) != null)
			{
				tp[0].dwThreadId = (uint)this.ID;
				int num = 0;
				tp[num].dwFields = (tp[num].dwFields | 1);
			}
			if ((fields & 4) != null)
			{
				tp[0].dwThreadState = (this.IsAlive ? 2U : 4U);
				int num2 = 0;
				tp[num2].dwFields = (tp[num2].dwFields | 4);
			}
			if (!this.IsAlive)
			{
				return 0;
			}
			if ((fields & 16) != null)
			{
				tp[0].bstrName = this.GetThreadInfo().Name;
				int num3 = 0;
				tp[num3].dwFields = (tp[num3].dwFields | 16);
			}
			if ((fields & 8) != null)
			{
				tp[0].bstrPriority = "Normal";
				int num4 = 0;
				tp[num4].dwFields = (tp[num4].dwFields | 8);
			}
			if ((fields & 32) != null)
			{
				try
				{
					StackFrame frame = this.GetFrame();
					if (frame != null)
					{
						tp[0].bstrLocation = frame.Frame.ToString();
						int num5 = 0;
						tp[num5].dwFields = (tp[num5].dwFields | 32);
					}
				}
				catch
				{
				}
			}
			return 0;
		}

		// Token: 0x0400007F RID: 127
		private IEventSender eventSender;
	}
}
