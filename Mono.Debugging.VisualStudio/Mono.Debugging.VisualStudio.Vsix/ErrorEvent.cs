using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200002F RID: 47
	public sealed class ErrorEvent : SynchronousEngineEvent, IDebugErrorEvent2
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00003990 File Offset: 0x00001B90
		public ErrorEvent(string message) : base(new Guid("fdb7a36c-8c53-41da-a337-8bd86b14d5cb"), 0U)
		{
			this.errorMessage = message;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000039AA File Offset: 0x00001BAA
		public ErrorEvent(string message, params object[] args) : this(string.Format(message, args))
		{
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000039B9 File Offset: 0x00001BB9
		public int GetErrorMessage(enum_MESSAGETYPE[] message_type, out string format, out int reason, out uint severity, out string helper_filename, out uint helper_id)
		{
			message_type[0] = enum_MESSAGETYPE.MT_MESSAGEBOX;
			format = this.errorMessage;
			reason = 0;
			severity = 16U;
			helper_filename = null;
			helper_id = 0U;
			return 0;
		}

		// Token: 0x04000036 RID: 54
		public const string IID = "fdb7a36c-8c53-41da-a337-8bd86b14d5cb";

		// Token: 0x04000037 RID: 55
		private string errorMessage;
	}
}
