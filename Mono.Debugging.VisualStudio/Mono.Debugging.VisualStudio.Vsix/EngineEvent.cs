using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200001C RID: 28
	public abstract class EngineEvent : Event
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00003709 File Offset: 0x00001909
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00003711 File Offset: 0x00001911
		public Thread Thread { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600007C RID: 124 RVA: 0x0000371A File Offset: 0x0000191A
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00003722 File Offset: 0x00001922
		public Process Process { get; private set; }

		// Token: 0x0600007E RID: 126 RVA: 0x0000372B File Offset: 0x0000192B
		protected EngineEvent(Guid iid, uint attrs) : base(iid, attrs)
		{
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003735 File Offset: 0x00001935
		protected EngineEvent(Process process, Guid iid, uint attrs) : base(iid, attrs)
		{
			this.Process = process;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003746 File Offset: 0x00001946
		protected EngineEvent(Thread thread, Guid iid, uint attrs) : this(thread.Process, iid, attrs)
		{
			this.Thread = thread;
		}
	}
}
