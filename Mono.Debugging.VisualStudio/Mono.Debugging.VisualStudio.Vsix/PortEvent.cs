using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000042 RID: 66
	public abstract class PortEvent : Event
	{
		// Token: 0x0600014C RID: 332 RVA: 0x0000372B File Offset: 0x0000192B
		protected PortEvent(Guid iid, uint attrs) : base(iid, attrs)
		{
		}
	}
}
