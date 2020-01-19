using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200003B RID: 59
	public class ErrorBreakpointResolution : IDebugErrorBreakpointResolution2
	{
		// Token: 0x06000129 RID: 297 RVA: 0x00005575 File Offset: 0x00003775
		public ErrorBreakpointResolution(Process process, string message)
		{
			this.process = process;
			this.message = message;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00002F7E File Offset: 0x0000117E
		public int GetBreakpointType(enum_BP_TYPE[] type)
		{
			type[0] = enum_BP_TYPE.BPT_CODE;
			return 0;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000558C File Offset: 0x0000378C
		public int GetResolutionInfo(enum_BPERESI_FIELDS fields, BP_ERROR_RESOLUTION_INFO[] info)
		{
			if ((fields & enum_BPERESI_FIELDS.BPERESI_PROGRAM) != 0)
			{
				info[0].pProgram = this.process;
				info[0].dwFields |= enum_BPERESI_FIELDS.BPERESI_PROGRAM;
			}
			if ((fields & enum_BPERESI_FIELDS.BPERESI_MESSAGE) != 0)
			{
				info[0].bstrMessage = this.message;
				info[0].dwFields |= enum_BPERESI_FIELDS.BPERESI_MESSAGE;
			}
			if ((fields & enum_BPERESI_FIELDS.BPERESI_TYPE) != 0)
			{
				info[0].dwType = enum_BP_ERROR_TYPE.BPET_GENERAL_ERROR;
				info[0].dwFields |= enum_BPERESI_FIELDS.BPERESI_TYPE;
			}
			return 0;
		}

		// Token: 0x0400008A RID: 138
		public Process process;

		// Token: 0x0400008B RID: 139
		public string message;
	}
}
