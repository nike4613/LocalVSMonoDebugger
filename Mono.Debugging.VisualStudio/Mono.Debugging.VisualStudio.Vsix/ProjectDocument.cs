using System;
using System.IO;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000A RID: 10
	internal class ProjectDocument
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00002535 File Offset: 0x00000735
		public ProjectDocument(ProjectVisualStudio project, string source)
		{
			this.project = project;
			this.parsedDocument = this.Parse(source, project);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002552 File Offset: 0x00000752
		private CompilerSettings GetCompilerArguments(ProjectVisualStudio project)
		{
			return project.CompilerSettings as CompilerSettings;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002560 File Offset: 0x00000760
		private ParsedDocument Parse(TextReader content, string fileName, ProjectVisualStudio project)
		{
			CSharpParser csharpParser = new CSharpParser(this.GetCompilerArguments(project));
			SyntaxTree syntaxTree = csharpParser.Parse(content, fileName);
			syntaxTree.Freeze();
			CSharpUnresolvedFile parsedFile = syntaxTree.ToTypeSystem();
			return new ParsedDocument(syntaxTree, parsedFile);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000259C File Offset: 0x0000079C
		private ParsedDocument Parse(string source, ProjectVisualStudio project)
		{
			ParsedDocument result;
			using (TextReader textReader = File.OpenText(source))
			{
				result = this.Parse(textReader, source, project);
			}
			return result;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000025D8 File Offset: 0x000007D8
		public ParsedDocument ParsedDocument
		{
			get
			{
				return this.parsedDocument;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001D RID: 29 RVA: 0x000025E0 File Offset: 0x000007E0
		public ICompilation Compilation
		{
			get
			{
				return this.project.CompilerSettings.CreateCompilation();
			}
		}

		// Token: 0x0400000A RID: 10
		private ParsedDocument parsedDocument;

		// Token: 0x0400000B RID: 11
		private ProjectVisualStudio project;
	}
}
