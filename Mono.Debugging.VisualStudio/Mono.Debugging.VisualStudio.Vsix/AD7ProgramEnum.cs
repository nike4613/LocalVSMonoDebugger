using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000012 RID: 18
	public class AD7ProgramEnum : AD7Enum<IDebugProgram2, IEnumDebugPrograms2>, IEnumDebugPrograms2
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00003634 File Offset: 0x00001834
		public AD7ProgramEnum(IDebugProgram2[] data) : base(data)
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000363D File Offset: 0x0000183D
		public new int Next(uint celt, IDebugProgram2[] rgelt, ref uint celtFetched)
		{
			return base.Next(celt, rgelt, out celtFetched);
		}
	}
}
