using System;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000008 RID: 8
	public interface IDebugLauncher
	{
		// Token: 0x06000016 RID: 22
		bool StartDebugger(SoftDebuggerSession session, StartInfo startInfo);
	}
}
