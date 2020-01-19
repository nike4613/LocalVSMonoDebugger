using System;
using System.Diagnostics;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000004 RID: 4
	public class DebuggerSession
	{
		// Token: 0x06000004 RID: 4 RVA: 0x000020F0 File Offset: 0x000002F0
		public DebuggerSession(StartInfo startInfo, IProgress<string> progress, SoftDebuggerSession session, IDebugLauncher debugLauncher = null)
		{
			DebuggerSession.tracer.Verbose("Entering constructor for: {0}", new object[]
			{
				this
			});
			this.progress = progress;
			this.startInfo = startInfo;
			this.debuggerSession = session;
			this.debugLauncher = (debugLauncher ?? new DebugLauncher());
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002144 File Offset: 0x00000344
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
			SoftDebuggerSession softDebuggerSession = debuggerSession;
			softDebuggerSession.ExceptionHandler = (ExceptionHandler)Delegate.Combine(softDebuggerSession.ExceptionHandler, new ExceptionHandler(delegate(Exception e)
			{
				progress.Report(e.Message);
				DebuggerSession.tracer.Error(e);
				return true;
			}));
			debuggerSession.LogWriter = debuggerSession.OutputWriter;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021D0 File Offset: 0x000003D0
		public void Start()
		{
			DebuggerSession.tracer.Verbose("Entering Start() for: {0}", new object[]
			{
				this
			});
			this.ConfigureSession(this.debuggerSession, this.progress);
			this.debugLauncher.StartDebugger(this.debuggerSession, this.startInfo);
			this.debuggerSession.Run(this.startInfo, this.startInfo.SessionOptions);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000223C File Offset: 0x0000043C
		public virtual void Exit()
		{
			this.debuggerSession.Exit();
		}

		// Token: 0x04000001 RID: 1
		private static readonly ITracer tracer = Tracer.Get<DebuggerSession>();

		// Token: 0x04000002 RID: 2
		private SoftDebuggerSession debuggerSession;

		// Token: 0x04000003 RID: 3
		private IDebugLauncher debugLauncher;

		// Token: 0x04000004 RID: 4
		private StartInfo startInfo;

		// Token: 0x04000005 RID: 5
		private IProgress<string> progress;
	}
}
