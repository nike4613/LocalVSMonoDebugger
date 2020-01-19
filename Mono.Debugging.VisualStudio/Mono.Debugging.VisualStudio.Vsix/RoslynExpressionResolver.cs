using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000005 RID: 5
	internal class RoslynExpressionResolver : IExpressionResolver
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002140 File Offset: 0x00000340
		public RoslynExpressionResolver(object workspace)
		{
			this.workspace = (workspace as Workspace);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002154 File Offset: 0x00000354
		public bool TryResolveType(string identifier, Client.SourceLocation location, out string resolvedType)
		{
			resolvedType = null;
			Document document = this.workspace.CurrentSolution.Projects.SelectMany((Project p) => p.Documents).FirstOrDefault((Document d) => string.Equals(d.FilePath, location.FileName, StringComparison.InvariantCultureIgnoreCase));
			if (document != null)
			{
				SemanticModel result = document.GetSemanticModelAsync(default(CancellationToken)).Result;
				if (result != null)
				{
					IEnumerable<ITypeSymbol> source = result.LookupSymbols(document.GetTextAsync(default(CancellationToken)).Result.Lines[location.Line - 1].Start + location.Column - 1, null, identifier, false).OfType<ITypeSymbol>();
					if (source.Count<ITypeSymbol>() == 1)
					{
						resolvedType = source.First<ITypeSymbol>().ToDisplayString(null);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04000001 RID: 1
		private Workspace workspace;
	}
}
