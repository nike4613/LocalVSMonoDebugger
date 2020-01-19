using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000029 RID: 41
	public sealed class BreakpointBoundEvent : AsyncProcessEvent, IDebugBreakpointBoundEvent2
	{
		// Token: 0x06000095 RID: 149 RVA: 0x000038A6 File Offset: 0x00001AA6
		public BreakpointBoundEvent(AD7BoundBreakpoint bound) : base(new Guid("1dddb704-cf99-4b8a-b746-dabb01dd13a0"))
		{
			this.boundBreakpoint = bound;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000038BF File Offset: 0x00001ABF
		public int EnumBoundBreakpoints(out IEnumDebugBoundBreakpoints2 e)
		{
			e = new AD7BoundBreakpointsEnum(new IDebugBoundBreakpoint2[]
			{
				this.boundBreakpoint
			});
			return 0;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000038D8 File Offset: 0x00001AD8
		public int GetPendingBreakpoint(out IDebugPendingBreakpoint2 pending)
		{
			pending = this.boundBreakpoint.PendingBreakpoint;
			return 0;
		}

		// Token: 0x0400002C RID: 44
		public const string IID = "1dddb704-cf99-4b8a-b746-dabb01dd13a0";

		// Token: 0x0400002D RID: 45
		private AD7BoundBreakpoint boundBreakpoint;
	}
}
