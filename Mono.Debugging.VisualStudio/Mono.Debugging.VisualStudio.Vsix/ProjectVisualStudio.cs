using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using Microsoft.VisualStudio.Shell;
using VSLangProj;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000B RID: 11
	internal class ProjectVisualStudio
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000025F2 File Offset: 0x000007F2
		public IProjectContent CompilerSettings
		{
			get
			{
				return this.content;
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000025FC File Offset: 0x000007FC
		private static Version ConvertLanguageVersion(string ver)
		{
			if (ver == "0")
			{
				return new Version(5, 0, 0, 0);
			}
			if (ver == "1")
			{
				return new Version(1, 0, 0, 0);
			}
			if (ver == "2")
			{
				return new Version(2, 0, 0, 0);
			}
			if (ver == "3")
			{
				return new Version(3, 0, 0, 0);
			}
			if (ver == "4")
			{
				return new Version(4, 0, 0, 0);
			}
			if (!(ver == "5"))
			{
				return new Version(5, 0, 0, 0);
			}
			return new Version(5, 0, 0, 0);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000026A0 File Offset: 0x000008A0
		private CompilerSettings GetCompilerSettingsFromVSProject(VSProject vsproject)
		{
			CompilerSettings compilerSettings = new CompilerSettings();
			if (vsproject == null || vsproject.Project == null || vsproject.Project.ConfigurationManager.ActiveConfiguration == null || vsproject.Project.ConfigurationManager.ActiveConfiguration.Properties == null)
			{
				compilerSettings.AllowUnsafeBlocks = true;
				return compilerSettings;
			}
			Properties properties = vsproject.Project.ConfigurationManager.ActiveConfiguration.Properties;
			if (properties.Count == 0)
			{
				return compilerSettings;
			}
			try
			{
				string text = (string)properties.Item("DefineConstants").Value;
				bool allowUnsafeBlocks = (bool)properties.Item("AllowUnsafeBlocks").Value;
				bool flag = (bool)properties.Item("Optimize").Value;
				string ver = (string)properties.Item("LanguageVersion").Value;
				bool checkForOverflow = (bool)properties.Item("CheckForOverflowUnderflow").Value;
				int warningLevel = (int)properties.Item("WarningLevel").Value;
				bool treatWarningsAsErrors = (bool)properties.Item("TreatWarningsAsErrors").Value;
				string text2 = (string)properties.Item("NoWarn").Value;
				if (!string.IsNullOrEmpty(text))
				{
					foreach (string item in from s in text.Split(new char[]
					{
						';',
						',',
						' ',
						'\t'
					})
					where !string.IsNullOrWhiteSpace(s)
					select s)
					{
						compilerSettings.ConditionalSymbols.Add(item);
					}
				}
				compilerSettings.AllowUnsafeBlocks = allowUnsafeBlocks;
				compilerSettings.LanguageVersion = ProjectVisualStudio.ConvertLanguageVersion(ver);
				compilerSettings.CheckForOverflow = checkForOverflow;
				compilerSettings.WarningLevel = warningLevel;
				compilerSettings.TreatWarningsAsErrors = treatWarningsAsErrors;
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array = text2.Split(new char[]
					{
						';',
						',',
						' ',
						'\t'
					});
					int i = 0;
					while (i < array.Length)
					{
						string s2 = array[i];
						int item2;
						try
						{
							item2 = int.Parse(s2);
						}
						catch (Exception)
						{
							goto IL_209;
						}
						goto IL_1FC;
						IL_209:
						i++;
						continue;
						IL_1FC:
						compilerSettings.DisabledWarnings.Add(item2);
						goto IL_209;
					}
				}
			}
			catch (Exception)
			{
				return compilerSettings;
			}
			return compilerSettings;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000291C File Offset: 0x00000B1C
		private IProjectContent GetCSharpProjectContent(VSProject vsproject)
		{
			IProjectContent projectContent = new CSharpProjectContent();
			Properties properties = vsproject.Project.ConfigurationManager.ActiveConfiguration.Properties;
			string fullName = vsproject.Project.FullName;
			projectContent = projectContent.SetAssemblyName(vsproject.Project.Properties.Item("AssemblyName").Value.ToString());
			projectContent = projectContent.SetProjectFileName(Path.GetFileName(fullName));
			projectContent = projectContent.SetLocation(fullName);
			return projectContent.SetCompilerSettings(this.GetCompilerSettingsFromVSProject(vsproject));
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000299C File Offset: 0x00000B9C
		public ProjectVisualStudio(VSProject vsproject)
		{
			IAssemblyReference[] references = this.GetReferences(vsproject);
			IUnresolvedFile[] files = this.GetFiles(vsproject);
			this.content = (this.GetCSharpProjectContent(vsproject) as CSharpProjectContent);
			this.content = this.content.AddAssemblyReferences(references);
			this.content = this.content.AddOrUpdateFiles(files);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002A0C File Offset: 0x00000C0C
		private string[] GetReferencesPaths(VSProject vsproject)
		{
			List<string> list = new List<string>();
			foreach (Reference reference in vsproject.References.OfType<Reference>())
			{
				string path = reference.Path;
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					list.Add(path);
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002A84 File Offset: 0x00000C84
		private string[] GetFilePaths(VSProject vsproject)
		{
			List<string> list = new List<string>();
			foreach (ProjectItem item in vsproject.Project.ProjectItems.OfType<ProjectItem>())
			{
				int? buildAction = item.GetBuildAction();
				if (buildAction != null && !(buildAction != 1))
				{
					string fullPath = item.GetFullPath();
					if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
					{
						list.Add(fullPath);
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002B34 File Offset: 0x00000D34
		private IUnresolvedAssembly LoadAssembly(string path)
		{
			if (this.assemblies.ContainsKey(path))
			{
				return this.assemblies[path];
			}
			CecilLoader cecilLoader = new CecilLoader();
			IUnresolvedAssembly unresolvedAssembly = cecilLoader.LoadAssemblyFile(path);
			this.assemblies.Add(path, unresolvedAssembly);
			return unresolvedAssembly;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002B78 File Offset: 0x00000D78
		private IUnresolvedFile LoadFile(string path)
		{
			ProjectDocument projectDocument = this.FindOrCreateDocument(path);
			return projectDocument.ParsedDocument.ParsedFile;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002B98 File Offset: 0x00000D98
		private IUnresolvedFile[] GetFiles(VSProject vsproject)
		{
			string[] files = this.GetFilePaths(vsproject);
			IUnresolvedFile[] projectFiles = new IUnresolvedFile[files.Length];
			ParallelOptions parallelOptions = new ParallelOptions
			{
				TaskScheduler = TaskScheduler.Default
			};
			Parallel.For(0, files.Length, parallelOptions, delegate(int i)
			{
				projectFiles[i] = this.LoadFile(files[i]);
			});
			return projectFiles;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002C08 File Offset: 0x00000E08
		private IAssemblyReference[] GetReferences(VSProject vsproject)
		{
			string[] assemblies = this.GetReferencesPaths(vsproject);
			IUnresolvedAssembly[] projectContents = new IUnresolvedAssembly[assemblies.Length];
			ParallelOptions parallelOptions = new ParallelOptions
			{
				TaskScheduler = TaskScheduler.Default
			};
			Parallel.For(0, assemblies.Length, parallelOptions, delegate(int i)
			{
				projectContents[i] = this.LoadAssembly(assemblies[i]);
			});
			return projectContents;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002C78 File Offset: 0x00000E78
		public static VSProject FindVSProjectBySource(string source)
		{
			DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
			Solution solution = dte.Solution;
			if (solution == null)
			{
				return null;
			}
			ProjectItem projectItem = solution.FindProjectItem(source);
			if (projectItem == null)
			{
				return null;
			}
			Project containingProject = projectItem.ContainingProject;
			if (containingProject == null)
			{
				return null;
			}
			VSProject vsproject = containingProject.Object as VSProject;
			if (vsproject == null)
			{
				return null;
			}
			return vsproject;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002CD4 File Offset: 0x00000ED4
		public ProjectDocument FindOrCreateDocument(string source)
		{
			if (this.documents.ContainsKey(source))
			{
				return this.documents[source];
			}
			ProjectDocument projectDocument = new ProjectDocument(this, source);
			this.documents[source] = projectDocument;
			return projectDocument;
		}

		// Token: 0x0400000C RID: 12
		private Dictionary<string, ProjectDocument> documents = new Dictionary<string, ProjectDocument>();

		// Token: 0x0400000D RID: 13
		private Dictionary<string, IUnresolvedAssembly> assemblies = new Dictionary<string, IUnresolvedAssembly>();

		// Token: 0x0400000E RID: 14
		private IProjectContent content;
	}
}
