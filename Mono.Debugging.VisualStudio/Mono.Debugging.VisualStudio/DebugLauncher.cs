using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000006 RID: 6
	internal class DebugLauncher : IDebugLauncher
	{
		// Token: 0x06000012 RID: 18 RVA: 0x0000229C File Offset: 0x0000049C
		public bool StartDebugger(SoftDebuggerSession session, StartInfo startInfo)
		{
			DebugLauncher.tracer.Verbose("Entering Launch for: {0}", new object[]
			{
				this
			});
			IVsDebugger4 service = ServiceProvider.GlobalProvider.GetService<SVsShellDebugger, IVsDebugger4>();
			SessionMarshalling obj = new SessionMarshalling(session, startInfo);
			VsDebugTargetInfo4 vsDebugTargetInfo = default(VsDebugTargetInfo4);
			vsDebugTargetInfo.dlo = 1U;
			vsDebugTargetInfo.bstrExe = "Mono";
			vsDebugTargetInfo.bstrCurDir = "";
			vsDebugTargetInfo.bstrArg = null;
			vsDebugTargetInfo.bstrRemoteMachine = null;
			vsDebugTargetInfo.fSendToOutputWindow = 0;
			vsDebugTargetInfo.guidPortSupplier = Guids.PortSupplierGuid;
			vsDebugTargetInfo.guidLaunchDebugEngine = Guids.EngineGuid;
			vsDebugTargetInfo.bstrPortName = "Mono";
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				ObjRef graph = RemotingServices.Marshal(obj);
				binaryFormatter.Serialize(memoryStream, graph);
				vsDebugTargetInfo.bstrOptions = Convert.ToBase64String(memoryStream.ToArray());
			}
			bool result;
			try
			{
				VsDebugTargetProcessInfo[] array = new VsDebugTargetProcessInfo[1];
				service.LaunchDebugTargets4(1U, new VsDebugTargetInfo4[]
				{
					vsDebugTargetInfo
				}, array);
				result = true;
			}
			catch (Exception ex)
			{
				DebugLauncher.tracer.Error("Controller.Launch ()", new object[]
				{
					ex
				});
				throw;
			}
			return result;
		}

		// Token: 0x0400000A RID: 10
		private static readonly ITracer tracer = Tracer.Get<DebugLauncher>();
	}
}
