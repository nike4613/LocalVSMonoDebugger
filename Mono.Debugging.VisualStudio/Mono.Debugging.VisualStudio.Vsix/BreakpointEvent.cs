using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200002C RID: 44
	public sealed class BreakpointEvent : StoppingEvent, IDebugBreakpointEvent2
	{
		// Token: 0x0600009D RID: 157 RVA: 0x00003937 File Offset: 0x00001B37
		public BreakpointEvent(Thread thread, AD7BoundBreakpoint bound) : base(thread, new Guid("501C1E21-C557-48B8-BA30-A1EAB0BC4A74"))
		{
			this.boundBreakpoints = new AD7BoundBreakpointsEnum(new IDebugBoundBreakpoint2[]
			{
				bound
			});
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000395F File Offset: 0x00001B5F
		public int EnumBreakpoints(out IEnumDebugBoundBreakpoints2 e)
		{
			e = this.boundBreakpoints;
			return 0;
		}

		// Token: 0x04000032 RID: 50
		public const string IID = "501C1E21-C557-48B8-BA30-A1EAB0BC4A74";

		// Token: 0x04000033 RID: 51
		private IEnumDebugBoundBreakpoints2 boundBreakpoints;
	}
}
