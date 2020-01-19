using System;
using EnvDTE;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000009 RID: 9
	public interface IStartInfo
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000017 RID: 23
		DebuggingOptions Options { get; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000018 RID: 24
		DebuggerSessionOptions SessionOptions { get; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000019 RID: 25
		Project StartupProject { get; }
	}
}
