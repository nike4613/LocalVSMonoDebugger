using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200003A RID: 58
	public class ErrorBreakpoint : IDebugErrorBreakpoint2
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00005517 File Offset: 0x00003717
		// (set) Token: 0x06000122 RID: 290 RVA: 0x0000551F File Offset: 0x0000371F
		public PendingBreakpoint PendingBreakpoint { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00005528 File Offset: 0x00003728
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00005530 File Offset: 0x00003730
		public ErrorBreakpointResolution ErrorResolution { get; private set; }

		// Token: 0x06000125 RID: 293 RVA: 0x00005539 File Offset: 0x00003739
		public ErrorBreakpoint(PendingBreakpoint pending, ErrorBreakpointResolution error)
		{
			this.PendingBreakpoint = pending;
			this.ErrorResolution = error;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000554F File Offset: 0x0000374F
		public ErrorBreakpoint(PendingBreakpoint pending, Process process, string error_message) : this(pending, new ErrorBreakpointResolution(process, error_message))
		{
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000555F File Offset: 0x0000375F
		public int GetBreakpointResolution(out IDebugErrorBreakpointResolution2 error)
		{
			error = this.ErrorResolution;
			return 0;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000556A File Offset: 0x0000376A
		public int GetPendingBreakpoint(out IDebugPendingBreakpoint2 pending)
		{
			pending = this.PendingBreakpoint;
			return 0;
		}
	}
}
