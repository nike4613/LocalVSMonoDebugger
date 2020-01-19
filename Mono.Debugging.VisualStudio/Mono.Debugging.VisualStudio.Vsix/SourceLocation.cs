using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000047 RID: 71
	[Serializable]
	public class SourceLocation
	{
		// Token: 0x06000199 RID: 409 RVA: 0x0000627D File Offset: 0x0000447D
		public SourceLocation(string file, int line)
		{
			this.FileName = file;
			this.Line = line;
			this.SourceRange = null;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000629F File Offset: 0x0000449F
		public SourceLocation(string file, int line, SourceRange range)
		{
			this.FileName = file;
			this.Line = line;
			this.SourceRange = new SourceRange?(range);
		}

		// Token: 0x040000B1 RID: 177
		public readonly string FileName;

		// Token: 0x040000B2 RID: 178
		public readonly int Line;

		// Token: 0x040000B3 RID: 179
		public readonly SourceRange? SourceRange;
	}
}
