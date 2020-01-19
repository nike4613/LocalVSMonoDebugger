using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200002B RID: 43
	public sealed class BreakpointErrorEvent : AsyncProcessEvent, IDebugBreakpointErrorEvent2
	{
		// Token: 0x0600009B RID: 155 RVA: 0x00003913 File Offset: 0x00001B13
		public BreakpointErrorEvent(ErrorBreakpoint error) : base(new Guid("abb0ca42-f82b-4622-84e4-6903ae90f210"))
		{
			this.errorBreakpoint = error;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000392C File Offset: 0x00001B2C
		public int GetErrorBreakpoint(out IDebugErrorBreakpoint2 error)
		{
			error = this.errorBreakpoint;
			return 0;
		}

		// Token: 0x04000030 RID: 48
		public const string IID = "abb0ca42-f82b-4622-84e4-6903ae90f210";

		// Token: 0x04000031 RID: 49
		private ErrorBreakpoint errorBreakpoint;
	}
}
