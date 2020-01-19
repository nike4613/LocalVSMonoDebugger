using System;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200004A RID: 74
	internal interface IThreadingAdapter
	{
		// Token: 0x060001A3 RID: 419
		Thread GetThreadFromTargetArgs(string caller, TargetEventArgs args);

		// Token: 0x060001A4 RID: 420
		void AddThread(long id, Thread main_thread);

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001A5 RID: 421
		bool Alive { get; }
	}
}
