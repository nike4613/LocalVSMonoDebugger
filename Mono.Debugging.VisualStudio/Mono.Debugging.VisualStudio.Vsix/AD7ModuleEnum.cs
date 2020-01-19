using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000016 RID: 22
	public class AD7ModuleEnum : AD7Enum<IDebugModule2, IEnumDebugModules2>, IEnumDebugModules2
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00003687 File Offset: 0x00001887
		public AD7ModuleEnum(IDebugModule2[] modules) : base(modules)
		{
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003690 File Offset: 0x00001890
		public new int Next(uint celt, IDebugModule2[] rgelt, ref uint celtFetched)
		{
			return base.Next(celt, rgelt, out celtFetched);
		}
	}
}
