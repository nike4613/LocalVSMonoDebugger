using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000044 RID: 68
	public sealed class PortProgramDestroyEvent : PortEvent, IDebugProgramDestroyEvent2
	{
		// Token: 0x0600014E RID: 334 RVA: 0x00005A61 File Offset: 0x00003C61
		public PortProgramDestroyEvent(uint exit_code) : base(new Guid("E147E9E3-6440-4073-A7B7-A65592C714B5"), 0U)
		{
			this.exitCode = exit_code;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00005A7B File Offset: 0x00003C7B
		public int GetExitCode(out uint exit_code)
		{
			exit_code = this.exitCode;
			return 0;
		}

		// Token: 0x0400009E RID: 158
		public const string IID = "E147E9E3-6440-4073-A7B7-A65592C714B5";

		// Token: 0x0400009F RID: 159
		private uint exitCode;
	}
}
