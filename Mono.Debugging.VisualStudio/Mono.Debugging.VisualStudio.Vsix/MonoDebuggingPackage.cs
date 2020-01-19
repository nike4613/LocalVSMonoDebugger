using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000C RID: 12
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("Mono Debugging for Visual Studio", "Support for debugging Mono processes with Visual Studio.", "4.9.10-pre (f143ac1)")]
	[Guid("2315269F-53E7-4844-8213-8F954654B744")]
	[ProvideBindingPath]
	[RegisterMonoDebugger]
	public class MonoDebuggingPackage : Package
	{
	}
}
