using System;
using System.Threading;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200001D RID: 29
	public abstract class SynchronousEngineEvent : EngineEvent
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000081 RID: 129 RVA: 0x0000375D File Offset: 0x0000195D
		public ManualResetEvent WaitHandle
		{
			get
			{
				return this.wait_handle;
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003765 File Offset: 0x00001965
		protected SynchronousEngineEvent(Guid iid, uint attrs) : base(iid, attrs | 1U)
		{
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000377D File Offset: 0x0000197D
		protected SynchronousEngineEvent(Process process, Guid iid, uint attrs) : base(process, iid, attrs | 1U)
		{
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003796 File Offset: 0x00001996
		protected SynchronousEngineEvent(Thread thread, Guid iid, uint attrs) : base(thread, iid, attrs | 1U)
		{
		}

		// Token: 0x04000021 RID: 33
		private ManualResetEvent wait_handle = new ManualResetEvent(false);
	}
}
