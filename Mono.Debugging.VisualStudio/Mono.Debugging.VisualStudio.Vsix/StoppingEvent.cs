using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000020 RID: 32
	public abstract class StoppingEvent : EngineEvent
	{
		// Token: 0x06000087 RID: 135 RVA: 0x000037C5 File Offset: 0x000019C5
		protected StoppingEvent(Thread thread, Guid iid) : base(thread, iid, 2U)
		{
		}
	}
}
