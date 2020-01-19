using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200003C RID: 60
	internal interface IEventSender
	{
		// Token: 0x0600012C RID: 300
		void SendEvent(EngineEvent e);
	}
}
