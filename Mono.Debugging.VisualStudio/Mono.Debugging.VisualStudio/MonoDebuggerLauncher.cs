using System;
using System.Diagnostics;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000B RID: 11
	public class MonoDebuggerLauncher
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00002881 File Offset: 0x00000A81
		public MonoDebuggerLauncher(IProgress<string> progress, IDebugLauncher debugLauncher = null)
		{
			MonoDebuggerLauncher.tracer.Verbose("Entering constructor for: {0}", new object[]
			{
				this
			});
			this.progress = progress;
			this.debugLauncher = (debugLauncher ?? new DebugLauncher());
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000028BC File Offset: 0x00000ABC
		public void StartSession(StartInfo startInfo, SoftDebuggerSession session)
		{
			MonoDebuggerLauncher.tracer.Verbose("Entering Start() for: {0}", new object[]
			{
				this
			});
			this.ConfigureSession(session, this.progress);
			this.debugLauncher.StartDebugger(session, startInfo);
			session.Run(startInfo, startInfo.SessionOptions);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000290C File Offset: 0x00000B0C
		private void ConfigureSession(SoftDebuggerSession debuggerSession, IProgress<string> progress)
		{
			debuggerSession.TargetExited += delegate(object s, TargetEventArgs e)
			{
				debuggerSession.Dispose();
			};
			debuggerSession.OutputWriter = delegate(bool is_stderr, string text)
			{
				if (progress != null && text.Trim(new char[]
				{
					'\n',
					'﻿'
				}).Length > 0)
				{
					progress.Report(text.TrimEnd(new char[]
					{
						'\n'
					}));
				}
			};
			SoftDebuggerSession debuggerSession2 = debuggerSession;
			debuggerSession2.ExceptionHandler = (ExceptionHandler)Delegate.Combine(debuggerSession2.ExceptionHandler, new ExceptionHandler(delegate(Exception e)
			{
				progress.Report(e.Message);
				MonoDebuggerLauncher.tracer.Error(e);
				return true;
			}));
			debuggerSession.LogWriter = debuggerSession.OutputWriter;
		}

		// Token: 0x0400001E RID: 30
		private static readonly ITracer tracer = Tracer.Get<MonoDebuggerLauncher>();

		// Token: 0x0400001F RID: 31
		private IDebugLauncher debugLauncher;

		// Token: 0x04000020 RID: 32
		private IProgress<string> progress;
	}
}
