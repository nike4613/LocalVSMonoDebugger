using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000039 RID: 57
	internal class BreakpointsAdapter : IBreakpointsAdapter
	{
		// Token: 0x06000113 RID: 275 RVA: 0x00005184 File Offset: 0x00003384
		public BreakpointsAdapter(Process process, IEventSender eventSender, SoftDebuggerSession session)
		{
			this.eventSender = eventSender;
			this.threading = process;
			this.process = process;
			this.session = session;
			session.TargetHitBreakpoint += this.OnTargetHitBreakpoint;
			session.Breakpoints.BreakpointStatusChanged += this.OnBreakpointStatusChanged;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000051E7 File Offset: 0x000033E7
		private bool TryGetPendingBreakpoint(Breakpoint breakpoint, out PendingBreakpoint pending)
		{
			pending = null;
			return breakpoint != null && this.pending_breakpoints.TryGetValue(breakpoint, out pending);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000051FE File Offset: 0x000033FE
		private static bool TryGetBoundBreakpoint(Breakpoint breakpoint, PendingBreakpoint pending, out AD7BoundBreakpoint bound)
		{
			bound = pending.BoundBreakpoints.FirstOrDefault<AD7BoundBreakpoint>();
			return bound != null;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00005214 File Offset: 0x00003414
		private void OnTargetHitBreakpoint(object sender, TargetEventArgs args)
		{
			try
			{
				Thread threadFromTargetArgs = this.threading.GetThreadFromTargetArgs("OnTargetHitBreakpoint()", args);
				Breakpoint breakpoint = args.BreakEvent as Breakpoint;
				PendingBreakpoint pending;
				AD7BoundBreakpoint bound;
				if (this.TryGetPendingBreakpoint(breakpoint, out pending) && BreakpointsAdapter.TryGetBoundBreakpoint(breakpoint, pending, out bound))
				{
					this.eventSender.SendEvent(new BreakpointEvent(threadFromTargetArgs, bound));
				}
				else if (!breakpoint.NonUserBreakpoint)
				{
					Utils.Message("OnTargetHitBreakpoint(): Unknown breakpoint!", new object[0]);
					this.eventSender.SendEvent(new StepCompleteEvent(threadFromTargetArgs));
				}
			}
			catch (Exception ex)
			{
				Utils.Message("OnTargetHitBreakpoint Exception: {0}", new object[]
				{
					ex
				});
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000052C0 File Offset: 0x000034C0
		private void OnBreakpointStatusChanged(object sender, BreakpointEventArgs args)
		{
			Breakpoint breakpoint = args.Breakpoint;
			PendingBreakpoint pendingBreakpoint;
			if (this.TryGetPendingBreakpoint(args.Breakpoint, out pendingBreakpoint))
			{
				BreakEventStatus status = args.Breakpoint.GetStatus(this.session);
				AD7BoundBreakpoint bound;
				if (status == BreakEventStatus.Bound)
				{
					if (!BreakpointsAdapter.TryGetBoundBreakpoint(breakpoint, pendingBreakpoint, out bound))
					{
						bound = new AD7BoundBreakpoint(pendingBreakpoint, args.Breakpoint, this.process);
						pendingBreakpoint.OnBreakpointBound(bound);
						return;
					}
				}
				else if (BreakpointsAdapter.TryGetBoundBreakpoint(breakpoint, pendingBreakpoint, out bound))
				{
					pendingBreakpoint.OnBreakpointUnbound(bound);
				}
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00005332 File Offset: 0x00003532
		public PendingBreakpoint CreatePendingBreakpoint(IDebugBreakpointRequest2 request)
		{
			return new PendingBreakpoint(this, this.eventSender, this.process, new PendingBreakpointRequest(request));
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000534C File Offset: 0x0000354C
		public void DeletePendingBreakpoint(PendingBreakpoint pending)
		{
			Breakpoint handle = pending.Handle;
			if (handle == null)
			{
				return;
			}
			this.Breakpoints.Remove(handle);
			this.pending_breakpoints.TryRemove(handle, out pending);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00005380 File Offset: 0x00003580
		public void DeleteBoundBreakpoint(AD7BoundBreakpoint bound)
		{
			if (bound.PendingBreakpoint.BoundBreakpoints.Contains(bound))
			{
				this.DeletePendingBreakpoint(bound.PendingBreakpoint);
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000053A4 File Offset: 0x000035A4
		public Breakpoint BindBreakpoint(PendingBreakpoint pending, SourceLocation location)
		{
			Breakpoint result;
			lock (this)
			{
				Breakpoint breakpoint = new Breakpoint(location.FileName, location.Line)
				{
					HitCount = pending.HitCount,
					HitCountMode = pending.HitCountMode,
					ConditionExpression = pending.ConditionExpression,
					BreakIfConditionChanges = pending.BreakIfConditionChanges
				};
				this.pending_breakpoints.TryAdd(breakpoint, pending);
				this.Breakpoints.Add(breakpoint);
				result = breakpoint;
			}
			return result;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000543C File Offset: 0x0000363C
		public void RemoveAllSetExceptions()
		{
			this.Breakpoints.ClearCatchpoints();
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00005449 File Offset: 0x00003649
		public void SetException(ExceptionInfo info)
		{
			this.Breakpoints.AddCatchpoint(info.Name, false);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00005460 File Offset: 0x00003660
		public int SetException(EXCEPTION_INFO[] pException)
		{
			if (pException[0].guidType != Guids.guidLanguageCSharp)
			{
				return 1;
			}
			ExceptionState exceptionState = ExceptionState.None;
			if ((pException[0].dwState & enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE) != 0 || (pException[0].dwState & enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_FIRST_CHANCE) != 0)
			{
				exceptionState |= ExceptionState.StopWhenThrown;
			}
			if ((pException[0].dwState & enum_EXCEPTION_STATE.EXCEPTION_STOP_SECOND_CHANCE) != 0 || (pException[0].dwState & enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT) != 0)
			{
				exceptionState |= ExceptionState.StopIfUnhandled;
			}
			ExceptionInfo exception = new ExceptionInfo(pException[0].bstrExceptionName, exceptionState);
			this.SetException(exception);
			return 0;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000054EF File Offset: 0x000036EF
		public int RemoveException(EXCEPTION_INFO[] pException)
		{
			this.Breakpoints.RemoveCatchpoint(pException[0].bstrExceptionName);
			return 0;
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000550A File Offset: 0x0000370A
		public BreakpointStore Breakpoints
		{
			get
			{
				return this.session.Breakpoints;
			}
		}

		// Token: 0x04000083 RID: 131
		private ConcurrentDictionary<Breakpoint, PendingBreakpoint> pending_breakpoints = new ConcurrentDictionary<Breakpoint, PendingBreakpoint>();

		// Token: 0x04000084 RID: 132
		private IEventSender eventSender;

		// Token: 0x04000085 RID: 133
		private IThreadingAdapter threading;

		// Token: 0x04000086 RID: 134
		private Process process;

		// Token: 0x04000087 RID: 135
		private SoftDebuggerSession session;
	}
}
