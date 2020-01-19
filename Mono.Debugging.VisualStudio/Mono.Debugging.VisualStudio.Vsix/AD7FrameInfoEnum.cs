using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000013 RID: 19
	public class AD7FrameInfoEnum : AD7Enum<FRAMEINFO, IEnumDebugFrameInfo2>, IEnumDebugFrameInfo2
	{
		// Token: 0x06000067 RID: 103 RVA: 0x00003648 File Offset: 0x00001848
		public AD7FrameInfoEnum(FRAMEINFO[] data) : base(data)
		{
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003651 File Offset: 0x00001851
		public new int Next(uint celt, FRAMEINFO[] rgelt, ref uint celtFetched)
		{
			return base.Next(celt, rgelt, out celtFetched);
		}
	}
}
