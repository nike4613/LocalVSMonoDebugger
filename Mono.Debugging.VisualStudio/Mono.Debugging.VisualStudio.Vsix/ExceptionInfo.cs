using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200003F RID: 63
	[Serializable]
	public struct ExceptionInfo
	{
		// Token: 0x0600012F RID: 303 RVA: 0x00005679 File Offset: 0x00003879
		public ExceptionInfo(string name, ExceptionState state)
		{
			this.Name = name;
			this.State = state;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00005689 File Offset: 0x00003889
		public override string ToString()
		{
			return string.Format("ExceptionInfo ({0}:{1})", this.Name, this.State);
		}

		// Token: 0x04000092 RID: 146
		public readonly string Name;

		// Token: 0x04000093 RID: 147
		public readonly ExceptionState State;
	}
}
