using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000031 RID: 49
	internal sealed class AD7DebugExceptionEvent : StoppingEvent, IDebugExceptionEvent2
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00003A0F File Offset: 0x00001C0F
		public AD7DebugExceptionEvent(Thread thread, string description) : base(thread, new Guid("51A94113-8788-4A54-AE15-08B74FF922D0"))
		{
			AD7DebugExceptionEvent.tracer.Verbose("AD7DebugExceptionEvent ctor: {0}", new object[]
			{
				description
			});
			this._exception = description;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003A42 File Offset: 0x00001C42
		public int CanPassToDebuggee()
		{
			AD7DebugExceptionEvent.tracer.Verbose("AD7DebugExceptionEvent CanPassToDebuggee ()", new object[0]);
			return 1;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003A5A File Offset: 0x00001C5A
		public int GetException(EXCEPTION_INFO[] pExceptionInfo)
		{
			pExceptionInfo[0].bstrExceptionName = this._exception;
			AD7DebugExceptionEvent.tracer.Verbose("AD7DebugExceptionEvent GetException: {0}", new object[]
			{
				this._exception
			});
			return 0;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003A8D File Offset: 0x00001C8D
		public int GetExceptionDescription(out string pbstrDescription)
		{
			pbstrDescription = this._exception;
			AD7DebugExceptionEvent.tracer.Verbose("AD7DebugExceptionEvent GetExceptionDescription: {0}", new object[]
			{
				pbstrDescription
			});
			return 0;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00003AB2 File Offset: 0x00001CB2
		public int PassToDebuggee(int fPass)
		{
			AD7DebugExceptionEvent.tracer.Verbose("AD7DebugExceptionEvent PassToDebuggee: {0}", new object[]
			{
				fPass
			});
			if (fPass != 0)
			{
				return 0;
			}
			return -2147467259;
		}

		// Token: 0x0400003B RID: 59
		private static readonly ITracer tracer = Tracer.Get<AD7DebugExceptionEvent>();

		// Token: 0x0400003C RID: 60
		public const string IID = "51A94113-8788-4A54-AE15-08B74FF922D0";

		// Token: 0x0400003D RID: 61
		private readonly string _exception;
	}
}
