using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000023 RID: 35
	public sealed class ProgramDestroyEvent : ProcessEvent, IDebugProgramDestroyEvent2
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00003819 File Offset: 0x00001A19
		public ProgramDestroyEvent(Process process, uint exit_code) : base(process, new Guid("E147E9E3-6440-4073-A7B7-A65592C714B5"))
		{
			this.exitCode = exit_code;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003833 File Offset: 0x00001A33
		public int GetExitCode(out uint exit_code)
		{
			exit_code = this.exitCode;
			return 0;
		}

		// Token: 0x04000025 RID: 37
		public const string IID = "E147E9E3-6440-4073-A7B7-A65592C714B5";

		// Token: 0x04000026 RID: 38
		private uint exitCode;
	}
}
