using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000003 RID: 3
	internal static class CecilHelper
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public static List<string> GetDependencies(string filename, bool fullname)
		{
			List<string> list = new List<string>();
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(filename);
			foreach (AssemblyNameReference assemblyNameReference in assemblyDefinition.MainModule.AssemblyReferences)
			{
				list.Add(fullname ? assemblyNameReference.FullName : assemblyNameReference.Name);
			}
			return list;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020D0 File Offset: 0x000002D0
		public static string GetAssemblyName(string filename)
		{
			AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(filename);
			return assemblyDefinition.Name.FullName;
		}
	}
}
