using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200002D RID: 45
	public sealed class ProcessCreateEvent : ProcessEvent, IDebugProcessCreateEvent2
	{
		// Token: 0x0600009F RID: 159 RVA: 0x0000396A File Offset: 0x00001B6A
		public ProcessCreateEvent(Process process) : base(process, new Guid("BAC3780F-04DA-4726-901C-BA6A4633E1CA"))
		{
		}

		// Token: 0x04000034 RID: 52
		public const string IID = "BAC3780F-04DA-4726-901C-BA6A4633E1CA";
	}
}
