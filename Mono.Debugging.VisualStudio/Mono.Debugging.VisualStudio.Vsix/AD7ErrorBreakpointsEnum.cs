using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200001A RID: 26
	public class AD7ErrorBreakpointsEnum : AD7Enum<IDebugErrorBreakpoint2, IEnumDebugErrorBreakpoints2>, IEnumDebugErrorBreakpoints2
	{
		// Token: 0x06000074 RID: 116 RVA: 0x000036C3 File Offset: 0x000018C3
		public AD7ErrorBreakpointsEnum(params IDebugErrorBreakpoint2[] breakpoints) : base(breakpoints)
		{
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000036CC File Offset: 0x000018CC
		public new int Next(uint celt, IDebugErrorBreakpoint2[] rgelt, ref uint celtFetched)
		{
			return base.Next(celt, rgelt, out celtFetched);
		}
	}
}
