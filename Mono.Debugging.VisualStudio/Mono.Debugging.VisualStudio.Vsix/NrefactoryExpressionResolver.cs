using System;
using System.Collections.Generic;
using System.Threading;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Debugging.Client;
using VSLangProj;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000008 RID: 8
	internal class NrefactoryExpressionResolver : IExpressionResolver
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002380 File Offset: 0x00000580
		private ProjectDocument FindOrCreateDocument(string source)
		{
			VSProject vsproject = ProjectVisualStudio.FindVSProjectBySource(source);
			if (vsproject == null)
			{
				return null;
			}
			ProjectVisualStudio projectVisualStudio;
			if (this.projects.ContainsKey(vsproject))
			{
				projectVisualStudio = this.projects[vsproject];
			}
			else
			{
				projectVisualStudio = new ProjectVisualStudio(vsproject);
				this.projects[vsproject] = projectVisualStudio;
			}
			return projectVisualStudio.FindOrCreateDocument(source);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000023D3 File Offset: 0x000005D3
		private ProjectDocument GetDocument(string path)
		{
			return this.FindOrCreateDocument(path);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000023DC File Offset: 0x000005DC
		public bool TryResolveType(string identifier, Client.SourceLocation location, out string resolvedType)
		{
			resolvedType = null;
			ProjectDocument document = this.GetDocument(location.FileName);
			if (document != null)
			{
				ResolveResult languageItem = this.GetLanguageItem(document, identifier, location);
				NamespaceResolveResult namespaceResolveResult = languageItem as NamespaceResolveResult;
				if (namespaceResolveResult != null)
				{
					resolvedType = namespaceResolveResult.NamespaceName;
					return true;
				}
				TypeResolveResult typeResolveResult = languageItem as TypeResolveResult;
				if (typeResolveResult != null && !typeResolveResult.IsError)
				{
					resolvedType = typeResolveResult.Type.FullName;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000243C File Offset: 0x0000063C
		private ResolveResult GetLanguageItem(ProjectDocument doc, string expression, Client.SourceLocation location)
		{
			ParsedDocument parsedDocument = doc.ParsedDocument;
			if (parsedDocument == null)
			{
				return null;
			}
			SyntaxTree ast = parsedDocument.GetAst<SyntaxTree>();
			CSharpUnresolvedFile csharpUnresolvedFile = parsedDocument.ParsedFile as CSharpUnresolvedFile;
			if (ast == null || csharpUnresolvedFile == null)
			{
				return null;
			}
			AstNode nodeAt = ast.GetNodeAt(location.Line, location.Column, null);
			if (nodeAt == null)
			{
				return null;
			}
			CSharpTypeResolveContext typeResolveContext = csharpUnresolvedFile.GetTypeResolveContext(doc.Compilation, new TextLocation(location.Line, location.Column));
			CSharpResolver resolver = new CSharpResolver(typeResolveContext);
			CSharpAstResolver csharpAstResolver = new CSharpAstResolver(resolver, ast, csharpUnresolvedFile);
			csharpAstResolver.ApplyNavigator(new NodeListResolveVisitorNavigator(new AstNode[]
			{
				nodeAt
			}), CancellationToken.None);
			CSharpResolver resolverStateBefore = csharpAstResolver.GetResolverStateBefore(nodeAt, CancellationToken.None);
			return resolverStateBefore.LookupSimpleNameOrTypeName(expression, new List<IType>(), NameLookupMode.Expression);
		}

		// Token: 0x04000007 RID: 7
		private Dictionary<VSProject, ProjectVisualStudio> projects = new Dictionary<VSProject, ProjectVisualStudio>();
	}
}
