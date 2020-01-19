using System;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000C RID: 12
	public class SessionMarshalling : MarshalByRefObject
	{
		// Token: 0x0600002F RID: 47 RVA: 0x000029A4 File Offset: 0x00000BA4
		public SessionMarshalling(SoftDebuggerSession session, StartInfo startInfo)
		{
			this.Session = session;
			this.StartInfo = startInfo;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000029BA File Offset: 0x00000BBA
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000029C2 File Offset: 0x00000BC2
		public SoftDebuggerSession Session { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000029CB File Offset: 0x00000BCB
		// (set) Token: 0x06000033 RID: 51 RVA: 0x000029D3 File Offset: 0x00000BD3
		public StartInfo StartInfo { get; private set; }
	}
}
