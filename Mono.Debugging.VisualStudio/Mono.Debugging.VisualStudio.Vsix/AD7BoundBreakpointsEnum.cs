using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000019 RID: 25
	public class AD7BoundBreakpointsEnum : AD7Enum<IDebugBoundBreakpoint2, IEnumDebugBoundBreakpoints2>, IEnumDebugBoundBreakpoints2
	{
		// Token: 0x06000072 RID: 114 RVA: 0x000036AF File Offset: 0x000018AF
		public AD7BoundBreakpointsEnum(params IDebugBoundBreakpoint2[] breakpoints) : base(breakpoints)
		{
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000036B8 File Offset: 0x000018B8
		public new int Next(uint celt, IDebugBoundBreakpoint2[] rgelt, ref uint celtFetched)
		{
			return base.Next(celt, rgelt, out celtFetched);
		}
	}
}
