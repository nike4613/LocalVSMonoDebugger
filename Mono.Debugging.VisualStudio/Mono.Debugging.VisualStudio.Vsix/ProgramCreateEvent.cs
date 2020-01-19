using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000022 RID: 34
	public sealed class ProgramCreateEvent : ProcessEvent, IDebugProgramCreateEvent2
	{
		// Token: 0x0600008C RID: 140 RVA: 0x00003806 File Offset: 0x00001A06
		public ProgramCreateEvent(Process process) : base(process, new Guid("96CD11EE-ECD4-4E89-957E-B5D496FC4139"))
		{
		}

		// Token: 0x04000024 RID: 36
		public const string IID = "96CD11EE-ECD4-4E89-957E-B5D496FC4139";
	}
}
