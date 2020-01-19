using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000018 RID: 24
	public class AD7CodeContextEnum : AD7Enum<IDebugCodeContext2, IEnumDebugCodeContexts2>, IEnumDebugCodeContexts2
	{
		// Token: 0x06000070 RID: 112 RVA: 0x0000369B File Offset: 0x0000189B
		public AD7CodeContextEnum(IDebugCodeContext2[] codeContexts) : base(codeContexts)
		{
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000036A4 File Offset: 0x000018A4
		public new int Next(uint celt, IDebugCodeContext2[] rgelt, ref uint celtFetched)
		{
			return base.Next(celt, rgelt, out celtFetched);
		}
	}
}
