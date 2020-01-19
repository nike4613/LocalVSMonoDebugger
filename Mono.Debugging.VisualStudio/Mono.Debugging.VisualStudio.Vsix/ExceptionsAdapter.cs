using System;
using Mono.Debugger.Soft;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000040 RID: 64
	internal class ExceptionsAdapter
	{
		// Token: 0x06000131 RID: 305 RVA: 0x000056A8 File Offset: 0x000038A8
		public ExceptionsAdapter(IThreadingAdapter threading, IEventSender eventSender, SoftDebuggerSession session)
		{
			this.threading = threading;
			this.eventSender = eventSender;
			this.session = session;
			session.TargetExceptionThrown += this.OnExceptionThrown;
			session.TargetUnhandledException += this.OnUnhandledException;
			session.ExceptionHandler = (ExceptionHandler)Delegate.Combine(session.ExceptionHandler, new ExceptionHandler(this.OnDebuggerException));
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00005718 File Offset: 0x00003918
		private void OnExceptionThrown(object sender, TargetEventArgs args)
		{
			try
			{
				var exceptionInfo = this.GetExceptionInfo(args.Backtrace, null, false);
				Thread threadFromTargetArgs = this.threading.GetThreadFromTargetArgs("OnExceptionThrown()", args);
				this.eventSender.SendEvent(new AD7DebugExceptionEvent(threadFromTargetArgs, ExceptionsAdapter.GetExceptionMessage(exceptionInfo)));
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00005774 File Offset: 0x00003974
		private static string GetExceptionMessage(Client.ExceptionInfo ex)
		{
			if (ex != null)
			{
				return string.Format("Exception:\n\n{0}: {1}", ex.Type, ExceptionsAdapter.ReplaceMessage(ex));
			}
			return "An exception occured.";
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00005795 File Offset: 0x00003995
		private static string GetUnhandledExceptionMessage(Client.ExceptionInfo ex)
		{
			if (ex != null)
			{
				return string.Format("Unhandled Exception:\n\n{0}: {1}", ex.Type, ExceptionsAdapter.ReplaceMessage(ex));
			}
			return "An unhandled exception occured.";
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000057B6 File Offset: 0x000039B6
		private static string ReplaceMessage(Client.ExceptionInfo ex)
		{
			if (!(ex.Message == "Loading..."))
			{
				return ex.Message;
			}
			return "<Timeout exceeded getting exception details>";
		}

		// Token: 0x06000136 RID: 310 RVA: 0x000057D8 File Offset: 0x000039D8
		private void OnUnhandledException(object sender, TargetEventArgs args)
		{
			try
			{
				EvaluationOptions evaluationOptions = this.session.EvaluationOptions.Clone();
				evaluationOptions.EllipsizeStrings = false;
				var exceptionInfo = this.GetExceptionInfo(args.Backtrace, evaluationOptions, true);
				Thread threadFromTargetArgs = this.threading.GetThreadFromTargetArgs("OnUnhandledException()", args);
				this.eventSender.SendEvent(new AD7DebugExceptionEvent(threadFromTargetArgs, ExceptionsAdapter.GetUnhandledExceptionMessage(exceptionInfo)));
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000584C File Offset: 0x00003A4C
		private static Client.ExceptionInfo GetException(TargetEventArgs args, EvaluationOptions options)
		{
			for (int i = 0; i < args.Backtrace.FrameCount; i++)
			{
				var exception = args.Backtrace.GetFrame(i).GetException(options);
				if (exception != null)
				{
					return exception;
				}
			}
			return null;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00005888 File Offset: 0x00003A88
		private Client.ExceptionInfo GetExceptionInfo(Backtrace backTrace, EvaluationOptions options = null, bool isUnhandled = false)
		{
			Client.ExceptionInfo exceptionInfo = null;
			if (backTrace != null && backTrace.FrameCount > 0)
			{
				for (int i = 0; i < backTrace.FrameCount; i++)
				{
					Mono.Debugging.Client.StackFrame frame = backTrace.GetFrame(i);
					if (isUnhandled || !frame.IsExternalCode)
					{
						try
						{
							exceptionInfo = ((options == null) ? frame.GetException() : frame.GetException(options));
						}
						catch (AbsentInformationException)
						{
							goto IL_4A;
						}
						if (exceptionInfo != null)
						{
							this.ResolveFullException(exceptionInfo);
							break;
						}
					}
					IL_4A:;
				}
			}
			return exceptionInfo;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00005900 File Offset: 0x00003B00
		private void ResolveFullException(Client.ExceptionInfo exception)
		{
			int num = 0;
			if (exception.Instance != null)
			{
				while (num++ < 4 && (exception.IsEvaluating || exception.StackIsEvaluating))
				{
					exception.Instance.WaitHandle.WaitOne(2000);
				}
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00005947 File Offset: 0x00003B47
		private bool OnDebuggerException(Exception ex)
		{
			if (!this.threading.Alive)
			{
				return true;
			}
			this.eventSender.SendEvent(new ErrorEvent("EXCEPTION: {0}", new object[]
			{
				ex
			}));
			return false;
		}

		// Token: 0x04000094 RID: 148
		private IEventSender eventSender;

		// Token: 0x04000095 RID: 149
		private IThreadingAdapter threading;

		// Token: 0x04000096 RID: 150
		private SoftDebuggerSession session;

		// Token: 0x04000097 RID: 151
		private const int MaxExceptionResolveAttempts = 4;
	}
}
