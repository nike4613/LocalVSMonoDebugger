using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200001B RID: 27
	public abstract class Event : IDebugEvent2
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000076 RID: 118 RVA: 0x000036D7 File Offset: 0x000018D7
		// (set) Token: 0x06000077 RID: 119 RVA: 0x000036DF File Offset: 0x000018DF
		public Guid ID { get; private set; }

		// Token: 0x06000078 RID: 120 RVA: 0x000036E8 File Offset: 0x000018E8
		protected Event(Guid iid, uint attrs)
		{
			this.ID = iid;
			this.attributes = attrs;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000036FE File Offset: 0x000018FE
		public int GetAttributes(out uint attrs)
		{
			attrs = this.attributes;
			return 0;
		}

		// Token: 0x0400001D RID: 29
		private uint attributes;
	}
}
