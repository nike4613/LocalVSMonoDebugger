using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000025 RID: 37
	public sealed class ThreadDestroyEvent : ThreadEvent, IDebugThreadDestroyEvent2
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00003851 File Offset: 0x00001A51
		public ThreadDestroyEvent(Thread thread, uint exit_code) : base(thread, new Guid("2C3B7532-A36F-4A6E-9072-49BE649B8541"))
		{
			this.exitCode = exit_code;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000386B File Offset: 0x00001A6B
		public int GetExitCode(out uint exit_code)
		{
			exit_code = this.exitCode;
			return 0;
		}

		// Token: 0x04000028 RID: 40
		public const string IID = "2C3B7532-A36F-4A6E-9072-49BE649B8541";

		// Token: 0x04000029 RID: 41
		private uint exitCode;
	}
}
