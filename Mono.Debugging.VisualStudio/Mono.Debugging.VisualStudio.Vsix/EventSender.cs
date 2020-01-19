using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200003D RID: 61
	internal class EventSender : IEventSender
	{
		// Token: 0x0600012D RID: 301 RVA: 0x00005613 File Offset: 0x00003813
		public EventSender(IDebugEventCallback2 callback, IDebugEngine2 engine)
		{
			this.callback = callback;
			this.engine = engine;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000562C File Offset: 0x0000382C
		public void SendEvent(EngineEvent e)
		{
			Guid id = e.ID;
			uint num;
			Utils.RequireOk(e.GetAttributes(out num));
			Utils.RequireOk(this.callback.Event(this.engine, e.Process, e.Process, e.Thread, e, ref id, num));
		}

		// Token: 0x0400008C RID: 140
		private IDebugEventCallback2 callback;

		// Token: 0x0400008D RID: 141
		private IDebugEngine2 engine;
	}
}
