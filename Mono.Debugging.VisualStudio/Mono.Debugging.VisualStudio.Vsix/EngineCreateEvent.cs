using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000021 RID: 33
	public sealed class EngineCreateEvent : SynchronousEngineEvent, IDebugEngineCreateEvent2
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000088 RID: 136 RVA: 0x000037D0 File Offset: 0x000019D0
		// (set) Token: 0x06000089 RID: 137 RVA: 0x000037D8 File Offset: 0x000019D8
		public IDebugEngine2 Engine { get; private set; }

		// Token: 0x0600008A RID: 138 RVA: 0x000037E1 File Offset: 0x000019E1
		public EngineCreateEvent(Engine engine) : base(new Guid("FE5B734C-759D-4E59-AB04-F103343BDD06"), 0U)
		{
			this.Engine = engine;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000037FB File Offset: 0x000019FB
		public int GetEngine(out IDebugEngine2 engine)
		{
			engine = this.Engine;
			return 0;
		}

		// Token: 0x04000022 RID: 34
		public const string IID = "FE5B734C-759D-4E59-AB04-F103343BDD06";
	}
}
