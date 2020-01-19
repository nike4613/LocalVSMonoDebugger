using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200004C RID: 76
	public class ComponentException : Exception
	{
		// Token: 0x060001AF RID: 431 RVA: 0x000065A0 File Offset: 0x000047A0
		public ComponentException(int hr)
		{
			this.hr = hr;
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x000065AF File Offset: 0x000047AF
		public new int HResult
		{
			get
			{
				return this.hr;
			}
		}

		// Token: 0x040000BA RID: 186
		private int hr;
	}
}
