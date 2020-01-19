using System;
using System.ComponentModel.Composition.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000007 RID: 7
	internal class TypeResolverAdapter
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000022AC File Offset: 0x000004AC
		internal ExpressionResolver TypeResolver
		{
			get
			{
				return this.resolver;
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000022B4 File Offset: 0x000004B4
		public TypeResolverAdapter(SoftDebuggerSession session)
		{
			this.session = session;
			session.TypeResolverHandler = (TypeResolverHandler)Delegate.Combine(session.TypeResolverHandler, new TypeResolverHandler(this.OnResolveType));
			this.resolver = new ExpressionResolver(this.GetWorkspace());
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002304 File Offset: 0x00000504
		private Workspace GetWorkspace()
		{
			IComponentModel componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
			if (componentModel != null)
			{
				try
				{
					ExportProvider defaultExportProvider = componentModel.DefaultExportProvider;
					return (defaultExportProvider != null) ? defaultExportProvider.GetExportedValue<Workspace>("VisualStudioWorkspace") : null;
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000235C File Offset: 0x0000055C
		private string OnResolveType(string identifier, Client.SourceLocation location)
		{
			return this.TypeResolver.ResolveType(identifier, location);
		}

		// Token: 0x04000004 RID: 4
		private const string WorkspaceContractName = "VisualStudioWorkspace";

		// Token: 0x04000005 RID: 5
		private SoftDebuggerSession session;

		// Token: 0x04000006 RID: 6
		private ExpressionResolver resolver;
	}
}
