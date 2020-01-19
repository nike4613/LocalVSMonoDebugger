using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000028 RID: 40
	public abstract class AsyncProcessEvent : EngineEvent
	{
		// Token: 0x06000094 RID: 148 RVA: 0x0000389C File Offset: 0x00001A9C
		protected AsyncProcessEvent(Guid iid) : base(iid, 0U)
		{
		}
	}
}
