using System;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000004 RID: 4
	internal interface IExpressionResolver
	{
		// Token: 0x06000005 RID: 5
		bool TryResolveType(string identifier, Client.SourceLocation location, out string resolvedType);
	}
}
