using System;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000D RID: 13
	public class AD7BoundBreakpoint : IDebugBoundBreakpoint2
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002D1A File Offset: 0x00000F1A
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002D22 File Offset: 0x00000F22
		public PendingBreakpoint PendingBreakpoint { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002D2B File Offset: 0x00000F2B
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002D33 File Offset: 0x00000F33
		public AD7BreakpointResolution Resolution { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002D3C File Offset: 0x00000F3C
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002D44 File Offset: 0x00000F44
		internal Breakpoint Handle { get; private set; }

		// Token: 0x06000032 RID: 50 RVA: 0x00002D4D File Offset: 0x00000F4D
		public AD7BoundBreakpoint(PendingBreakpoint pending, Breakpoint handle, Process process)
		{
			this.PendingBreakpoint = pending;
			this.Handle = handle;
			this.Resolution = new AD7BreakpointResolution(process);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002D70 File Offset: 0x00000F70
		public int Delete()
		{
			int result;
			lock (this)
			{
				if (!this.deleted)
				{
					this.deleted = true;
					this.PendingBreakpoint.Engine.DeleteBoundBreakpoint(this);
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002DC8 File Offset: 0x00000FC8
		public int Enable(int enable)
		{
			int result;
			lock (this)
			{
				if (this.deleted)
				{
					result = 1;
				}
				else
				{
					this.Handle.Enabled = (enable != 0);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002E1C File Offset: 0x0000101C
		public int GetBreakpointResolution(out IDebugBreakpointResolution2 ppBPResolution)
		{
			ppBPResolution = this.Resolution;
			return 0;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002E27 File Offset: 0x00001027
		public int GetPendingBreakpoint(out IDebugPendingBreakpoint2 ppPendingBreakpoint)
		{
			ppPendingBreakpoint = this.PendingBreakpoint;
			return 0;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002E32 File Offset: 0x00001032
		public int GetState(enum_BP_STATE[] state)
		{
			state[0] = 0;
			if (this.deleted)
			{
				state[0] = enum_BP_STATE.BPS_DELETED;
			}
			else if (this.Handle.Enabled)
			{
				state[0] = enum_BP_STATE.BPS_ENABLED;
			}
			else if (!this.Handle.Enabled)
			{
				state[0] = enum_BP_STATE.BPS_DISABLED;
			}
			return 0;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002E6B File Offset: 0x0000106B
		public virtual int GetHitCount(out uint pdwHitCount)
		{
			pdwHitCount = (uint)this.Handle.CurrentHitCount;
			return 0;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002E7B File Offset: 0x0000107B
		public virtual int SetCondition(BP_CONDITION bpCondition)
		{
			if (bpCondition.styleCondition == enum_BP_COND_STYLE.BP_COND_NONE)
			{
				this.Handle.ConditionExpression = null;
			}
			else
			{
				this.Handle.ConditionExpression = bpCondition.bstrCondition;
				this.Handle.BreakIfConditionChanges = (bpCondition.styleCondition == enum_BP_COND_STYLE.BP_COND_WHEN_CHANGED);
			}
			return 0;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002EB9 File Offset: 0x000010B9
		public virtual int SetHitCount(uint dwHitCount)
		{
			this.Handle.CurrentHitCount = (int)dwHitCount;
			return 0;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002EC8 File Offset: 0x000010C8
		public virtual int SetPassCount(BP_PASSCOUNT bpPassCount)
		{
			switch (bpPassCount.stylePassCount)
			{
			case enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_NONE:
				this.Handle.HitCountMode = HitCountMode.None;
				break;
			case enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_EQUAL:
				this.Handle.HitCountMode = HitCountMode.EqualTo;
				this.Handle.HitCount = (int)bpPassCount.dwPassCount;
				break;
			case enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_EQUAL_OR_GREATER:
				this.Handle.HitCountMode = HitCountMode.GreaterThanOrEqualTo;
				this.Handle.HitCount = (int)bpPassCount.dwPassCount;
				break;
			case enum_BP_PASSCOUNT_STYLE.BP_PASSCOUNT_MOD:
				this.Handle.HitCountMode = HitCountMode.MultipleOf;
				this.Handle.HitCount = (int)bpPassCount.dwPassCount;
				break;
			}
			return 0;
		}

		// Token: 0x04000012 RID: 18
		private bool deleted;
	}
}
