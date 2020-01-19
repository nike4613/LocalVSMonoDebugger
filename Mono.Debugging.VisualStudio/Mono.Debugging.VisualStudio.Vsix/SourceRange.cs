using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000048 RID: 72
	[Serializable]
	public struct SourceRange
	{
		// Token: 0x0600019B RID: 411 RVA: 0x000062C1 File Offset: 0x000044C1
		public SourceRange(int start_line, int end_line, int start_col, int end_col)
		{
			this.StartLine = start_line;
			this.EndLine = end_line;
			this.StartColumn = start_col;
			this.EndColumn = end_col;
		}

		// Token: 0x040000B4 RID: 180
		public readonly int StartLine;

		// Token: 0x040000B5 RID: 181
		public readonly int EndLine;

		// Token: 0x040000B6 RID: 182
		public readonly int StartColumn;

		// Token: 0x040000B7 RID: 183
		public readonly int EndColumn;
	}
}
