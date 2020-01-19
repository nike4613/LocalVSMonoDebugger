using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000043 RID: 67
	public sealed class PortProgramCreateEvent : PortEvent, IDebugProgramCreateEvent2
	{
		// Token: 0x0600014D RID: 333 RVA: 0x00005A4E File Offset: 0x00003C4E
		public PortProgramCreateEvent() : base(new Guid("96CD11EE-ECD4-4E89-957E-B5D496FC4139"), 0U)
		{
		}

		// Token: 0x0400009D RID: 157
		public const string IID = "96CD11EE-ECD4-4E89-957E-B5D496FC4139";
	}
}
