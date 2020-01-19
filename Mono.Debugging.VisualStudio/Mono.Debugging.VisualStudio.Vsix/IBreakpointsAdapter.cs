using System;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000038 RID: 56
	internal interface IBreakpointsAdapter
	{
		// Token: 0x0600010B RID: 267
		PendingBreakpoint CreatePendingBreakpoint(IDebugBreakpointRequest2 request);

		// Token: 0x0600010C RID: 268
		void DeletePendingBreakpoint(PendingBreakpoint pending);

		// Token: 0x0600010D RID: 269
		void DeleteBoundBreakpoint(AD7BoundBreakpoint bound);

		// Token: 0x0600010E RID: 270
		void RemoveAllSetExceptions();

		// Token: 0x0600010F RID: 271
		int SetException(EXCEPTION_INFO[] pException);

		// Token: 0x06000110 RID: 272
		int RemoveException(EXCEPTION_INFO[] pException);

		// Token: 0x06000111 RID: 273
		Breakpoint BindBreakpoint(PendingBreakpoint pending, SourceLocation location);

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000112 RID: 274
		BreakpointStore Breakpoints { get; }
	}
}
