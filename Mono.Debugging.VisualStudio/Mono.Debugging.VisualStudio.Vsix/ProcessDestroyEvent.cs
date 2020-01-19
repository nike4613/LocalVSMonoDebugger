using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200002E RID: 46
	public sealed class ProcessDestroyEvent : ProcessEvent, IDebugProcessDestroyEvent2
	{
		// Token: 0x060000A0 RID: 160 RVA: 0x0000397D File Offset: 0x00001B7D
		public ProcessDestroyEvent(Process process) : base(process, new Guid("3E2A0832-17E1-4886-8C0E-204DA242995F"))
		{
		}

		// Token: 0x04000035 RID: 53
		public const string IID = "3E2A0832-17E1-4886-8C0E-204DA242995F";
	}
}
