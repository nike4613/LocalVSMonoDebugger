using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200001E RID: 30
	public abstract class ThreadEvent : SynchronousEngineEvent
	{
		// Token: 0x06000085 RID: 133 RVA: 0x000037AF File Offset: 0x000019AF
		protected ThreadEvent(Thread thread, Guid iid) : base(thread, iid, 0U)
		{
		}
	}
}
