using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200001F RID: 31
	public abstract class ProcessEvent : SynchronousEngineEvent
	{
		// Token: 0x06000086 RID: 134 RVA: 0x000037BA File Offset: 0x000019BA
		protected ProcessEvent(Process process, Guid iid) : base(process, iid, 0U)
		{
		}
	}
}
