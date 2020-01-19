using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Clide;
using Clide.Solution;
using EnvDTE;
using Microsoft.CSharp.RuntimeBinder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.CompilerServices.SymbolWriter;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;
using VSLangProj;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000D RID: 13
	public class StartInfo : SoftDebuggerStartInfo, IStartInfo
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000029DC File Offset: 0x00000BDC
		// (set) Token: 0x06000035 RID: 53 RVA: 0x000029E4 File Offset: 0x00000BE4
		public DebuggingOptions Options { get; protected set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000036 RID: 54 RVA: 0x000029ED File Offset: 0x00000BED
		// (set) Token: 0x06000037 RID: 55 RVA: 0x000029F5 File Offset: 0x00000BF5
		public DebuggerSessionOptions SessionOptions { get; protected set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000038 RID: 56 RVA: 0x000029FE File Offset: 0x00000BFE
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002A06 File Offset: 0x00000C06
		public Project StartupProject { get; private set; }

		// Token: 0x0600003A RID: 58 RVA: 0x00002A0F File Offset: 0x00000C0F
		public StartInfo(SoftDebuggerStartArgs start_args, DebuggingOptions options, Project startupProject) : base(start_args)
		{
			this.StartupProject = startupProject;
			this.Options = options;
			this.SessionOptions = this.CreateDebuggerSessionOptions();
			this.GetUserAssemblyNamesAndMaps();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002A38 File Offset: 0x00000C38
		protected void GetUserAssemblyNamesAndMaps()
		{
			base.UserAssemblyNames = new List<AssemblyName>();
			base.AssemblyPathMap = new Dictionary<string, string>();
			IProjectNode projectNode = this.StartupProject.Adapt().AsProjectNode();
			this.AddProjectToPathMap(projectNode);
			this.AddReferencesToPathMap(this.StartupProject);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002A80 File Offset: 0x00000C80
		private bool AddProjectToPathMap(IProjectNode projectNode)
		{
			var configProperties = projectNode.PropertiesFor(projectNode.Configuration.ActiveConfigurationName);
			var assemblyPath = Path.Combine(configProperties.targetDir, configProperties.TargetFileName);
			if (File.Exists(assemblyPath))
				return AddAssemblyToPathMap(assemblyPath, true);
			return true;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002CF0 File Offset: 0x00000EF0
		private void AddReferencesToPathMap(Project project)
		{
			foreach (Reference reference in project.GetReferences())
			{
				try
				{
					if (reference.SourceProject != null)
					{
						IProjectNode projectNode = reference.SourceProject.Adapt().AsProjectNode();
						if (this.AddProjectToPathMap(projectNode))
						{
							this.AddReferencesToPathMap(reference.SourceProject);
						}
					}
					else
					{
						this.AddAssemblyToPathMap(reference.Path, this.IsMyCode(reference.Path));
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002D90 File Offset: 0x00000F90
		private bool IsMyCode(string path)
		{
			string text = Path.ChangeExtension(path, "pdb");
			if (File.Exists(text))
			{
				if (File.Exists(path + ".mdb"))
				{
					MonoSymbolFile monoSymbolFile = MonoSymbolFile.ReadSymbolFile(path + ".mdb");
					return File.Exists(monoSymbolFile.Sources.First<SourceFileEntry>().FileName);
				}
				if (StartInfo.IsPortablePdb(text))
				{
					ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(path);
					DefaultSymbolReaderProvider defaultSymbolReaderProvider = new DefaultSymbolReaderProvider(false);
					moduleDefinition.ReadSymbols(defaultSymbolReaderProvider.GetSymbolReader(moduleDefinition, text));
					return File.Exists((from t in moduleDefinition.Types
					where t.HasMethods
					select t).First<TypeDefinition>().Methods.First<MethodDefinition>().DebugInformation.SequencePoints.First<SequencePoint>().Document.Url);
				}
			}
			return false;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002E6C File Offset: 0x0000106C
		public static bool IsPortablePdb(string filename)
		{
			bool result;
			try
			{
				using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						result = (binaryReader.ReadUInt32() == 1112167234U);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002EDC File Offset: 0x000010DC
		private bool AddAssemblyToPathMap(string fileName, bool userAssembly)
		{
			if (!base.AssemblyPathMap.ContainsValue(fileName))
			{
				AssemblyName assemblyName = new AssemblyName(CecilHelper.GetAssemblyName(fileName));
				if (userAssembly)
				{
					base.UserAssemblyNames.Add(assemblyName);
				}
				base.AssemblyPathMap.Add(assemblyName.FullName, fileName);
				return true;
			}
			return false;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002F28 File Offset: 0x00001128
		protected DebuggingOptions CreateDefaultDebuggingOptions(bool longWait = true)
		{
			if (longWait)
			{
				return new DebuggingOptions
				{
					EvaluationTimeout = new int?(60000),
					MemberEvaluationTimeout = new int?(180000),
					ModificationTimeout = new int?(120000),
					SocketTimeout = 0
				};
			}
			return new DebuggingOptions
			{
				EvaluationTimeout = new int?(10000),
				MemberEvaluationTimeout = new int?(15000),
				ModificationTimeout = new int?(10000),
				SocketTimeout = 0
			};
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002FB4 File Offset: 0x000011B4
		protected DebuggerSessionOptions CreateDebuggerSessionOptions()
		{
			EvaluationOptions defaultOptions = EvaluationOptions.DefaultOptions;
			defaultOptions.GroupPrivateMembers = true;
			defaultOptions.GroupStaticMembers = true;
			defaultOptions.FlattenHierarchy = false;
			defaultOptions.AllowToStringCalls = false;
			return new DebuggerSessionOptions
			{
				EvaluationOptions = defaultOptions,
				ProjectAssembliesOnly = true
			};
		}

		// Token: 0x04000026 RID: 38
		private const uint ppdb_signature = 1112167234U;
	}
}
