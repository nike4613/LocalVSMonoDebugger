using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000027 RID: 39
	public sealed class StepCompleteEvent : StoppingEvent, IDebugStepCompleteEvent2
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00003889 File Offset: 0x00001A89
		public StepCompleteEvent(Thread thread) : base(thread, new Guid("0f7f24c1-74d9-4ea6-a3ea-7edb2d81441d"))
		{
		}

		// Token: 0x0400002B RID: 43
		public const string IID = "0f7f24c1-74d9-4ea6-a3ea-7edb2d81441d";
	}
}
