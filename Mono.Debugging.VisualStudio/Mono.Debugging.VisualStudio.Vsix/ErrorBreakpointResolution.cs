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
			type[0] = 1;
			return 0;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000558C File Offset: 0x0000378C
		public int GetResolutionInfo(enum_BPERESI_FIELDS fields, BP_ERROR_RESOLUTION_INFO[] info)
		{
			if ((fields & 2) != null)
			{
				info[0].pProgram = this.process;
				int num = 0;
				info[num].dwFields = (info[num].dwFields | 2);
			}
			if ((fields & 8) != null)
			{
				info[0].bstrMessage = this.message;
				int num2 = 0;
				info[num2].dwFields = (info[num2].dwFields | 8);
			}
			if ((fields & 16) != null)
			{
				info[0].dwType = 117440514;
				int num3 = 0;
				info[num3].dwFields = (info[num3].dwFields | 16);
			}
			return 0;
		}

		// Token: 0x0400008A RID: 138
		public Process process;

		// Token: 0x0400008B RID: 139
		public string message;
	}
}
