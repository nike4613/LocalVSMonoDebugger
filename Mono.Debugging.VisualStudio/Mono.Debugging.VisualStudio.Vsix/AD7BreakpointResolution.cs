using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000E RID: 14
	public class AD7BreakpointResolution : IDebugBreakpointResolution2
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002F5E File Offset: 0x0000115E
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00002F66 File Offset: 0x00001166
		public Process Process { get; private set; }

		// Token: 0x0600003E RID: 62 RVA: 0x00002F6F File Offset: 0x0000116F
		public AD7BreakpointResolution(Process process)
		{
			this.Process = process;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002F7E File Offset: 0x0000117E
		public int GetBreakpointType(enum_BP_TYPE[] type)
		{
			type[0] = enum_BP_TYPE.BPT_CODE;
			return 0;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002F85 File Offset: 0x00001185
		public int GetResolutionInfo(enum_BPRESI_FIELDS fields, BP_RESOLUTION_INFO[] info)
		{
			if ((fields & enum_BPRESI_FIELDS.BPRESI_PROGRAM) != 0)
			{
				info[0].pProgram = this.Process;
				info[0].dwFields |= enum_BPRESI_FIELDS.BPRESI_PROGRAM;
			}
			return 0;
		}
	}
}
