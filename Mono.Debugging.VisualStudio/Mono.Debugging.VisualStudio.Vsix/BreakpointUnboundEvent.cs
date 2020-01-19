using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200002A RID: 42
	public sealed class BreakpointUnboundEvent : AsyncProcessEvent, IDebugBreakpointUnboundEvent2
	{
		// Token: 0x06000098 RID: 152 RVA: 0x000038E8 File Offset: 0x00001AE8
		public BreakpointUnboundEvent(AD7BoundBreakpoint bound) : base(new Guid("78d1db4f-c557-4dc5-a2dd-5369d21b1c8c"))
		{
			this.boundBreakpoint = bound;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003901 File Offset: 0x00001B01
		public int GetBreakpoint(out IDebugBoundBreakpoint2 ppBP)
		{
			ppBP = this.boundBreakpoint;
			return 0;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000390C File Offset: 0x00001B0C
		public int GetReason(enum_BP_UNBOUND_REASON[] pdwUnboundReason)
		{
			pdwUnboundReason[0] = enum_BP_UNBOUND_REASON.BPUR_CODE_UNLOADED;
			return 0;
		}

		// Token: 0x0400002E RID: 46
		public const string IID = "78d1db4f-c557-4dc5-a2dd-5369d21b1c8c";

		// Token: 0x0400002F RID: 47
		private AD7BoundBreakpoint boundBreakpoint;
	}
}
