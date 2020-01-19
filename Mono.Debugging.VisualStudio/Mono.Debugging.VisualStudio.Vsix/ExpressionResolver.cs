using System;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000006 RID: 6
	internal class ExpressionResolver
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002247 File Offset: 0x00000447
		public ExpressionResolver(object workspace)
		{
			if (workspace != null)
			{
				this.defaultResolver = new RoslynExpressionResolver(workspace);
			}
			this.fallbackResolver = new NrefactoryExpressionResolver();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000226C File Offset: 0x0000046C
		internal string ResolveType(string identifier, Client.SourceLocation location)
		{
			string empty = string.Empty;
			if ((this.defaultResolver != null && this.defaultResolver.TryResolveType(identifier, location, out empty)) || this.fallbackResolver.TryResolveType(identifier, location, out empty))
			{
				return empty;
			}
			return null;
		}

		// Token: 0x04000002 RID: 2
		private RoslynExpressionResolver defaultResolver;

		// Token: 0x04000003 RID: 3
		private NrefactoryExpressionResolver fallbackResolver;
	}
}
