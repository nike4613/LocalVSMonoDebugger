using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000009 RID: 9
	internal class ParsedDocument
	{
		// Token: 0x06000013 RID: 19 RVA: 0x000024F4 File Offset: 0x000006F4
		public ParsedDocument(SyntaxTree ast, IUnresolvedFile parsedFile)
		{
			this.parsedFile = parsedFile;
			this.Ast = ast;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000250A File Offset: 0x0000070A
		// (set) Token: 0x06000015 RID: 21 RVA: 0x00002512 File Offset: 0x00000712
		public object Ast { get; private set; }

		// Token: 0x06000016 RID: 22 RVA: 0x0000251B File Offset: 0x0000071B
		public T GetAst<T>() where T : class
		{
			return this.Ast as T;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000017 RID: 23 RVA: 0x0000252D File Offset: 0x0000072D
		public IUnresolvedFile ParsedFile
		{
			get
			{
				return this.parsedFile;
			}
		}

		// Token: 0x04000008 RID: 8
		private IUnresolvedFile parsedFile;
	}
}
