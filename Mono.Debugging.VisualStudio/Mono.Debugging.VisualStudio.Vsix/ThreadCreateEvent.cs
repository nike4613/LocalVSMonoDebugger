using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000024 RID: 36
	public sealed class ThreadCreateEvent : ThreadEvent, IDebugThreadCreateEvent2
	{
		// Token: 0x0600008F RID: 143 RVA: 0x0000383E File Offset: 0x00001A3E
		public ThreadCreateEvent(Thread thread) : base(thread, new Guid("2090CCFC-70C5-491D-A5E8-BAD2DD9EE3EA"))
		{
		}

		// Token: 0x04000027 RID: 39
		public const string IID = "2090CCFC-70C5-491D-A5E8-BAD2DD9EE3EA";
	}
}
