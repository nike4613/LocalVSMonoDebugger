using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000005 RID: 5
	public class DebuggingOptions
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002255 File Offset: 0x00000455
		// (set) Token: 0x0600000A RID: 10 RVA: 0x0000225D File Offset: 0x0000045D
		public int? EvaluationTimeout { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002266 File Offset: 0x00000466
		// (set) Token: 0x0600000C RID: 12 RVA: 0x0000226E File Offset: 0x0000046E
		public int? MemberEvaluationTimeout { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002277 File Offset: 0x00000477
		// (set) Token: 0x0600000E RID: 14 RVA: 0x0000227F File Offset: 0x0000047F
		public int? ModificationTimeout { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002288 File Offset: 0x00000488
		// (set) Token: 0x06000010 RID: 16 RVA: 0x00002290 File Offset: 0x00000490
		public int SocketTimeout { get; set; }
	}
}
