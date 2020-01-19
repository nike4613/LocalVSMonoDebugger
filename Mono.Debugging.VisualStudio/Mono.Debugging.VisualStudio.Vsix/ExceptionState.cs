using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200003E RID: 62
	[Flags]
	[Serializable]
	public enum ExceptionState
	{
		// Token: 0x0400008F RID: 143
		None = 0,
		// Token: 0x04000090 RID: 144
		StopWhenThrown = 1,
		// Token: 0x04000091 RID: 145
		StopIfUnhandled = 2
	}
}
