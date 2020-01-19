using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000026 RID: 38
	public sealed class LoadCompleteEvent : ThreadEvent, IDebugLoadCompleteEvent2
	{
		// Token: 0x06000092 RID: 146 RVA: 0x00003876 File Offset: 0x00001A76
		public LoadCompleteEvent(Thread thread) : base(thread, new Guid("B1844850-1349-45D4-9F12-495212F5EB0B"))
		{
		}

		// Token: 0x0400002A RID: 42
		public const string IID = "B1844850-1349-45D4-9F12-495212F5EB0B";
	}
}
