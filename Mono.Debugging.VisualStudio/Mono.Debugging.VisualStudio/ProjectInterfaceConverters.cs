using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using IServiceProvider = System.IServiceProvider;
using OLE_IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000A RID: 10
	internal static class ProjectInterfaceConverters
	{
		// Token: 0x0600001A RID: 26 RVA: 0x000024A8 File Offset: 0x000006A8
		public static IVsHierarchy GetCurrentHierarchy(this IServiceProvider provider)
		{
			Project selectedProject = provider.GetSelectedProject();
			if (selectedProject == null)
			{
				return null;
			}
			return selectedProject.ToHierarchy();
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000024C8 File Offset: 0x000006C8
		public static Project GetSelectedProject(this IServiceProvider provider)
		{
			DTE dte = (DTE)provider.GetService(typeof(DTE));
			if (dte == null)
			{
				throw new InvalidOperationException("DTE not found.");
			}
			SelectedItem selectedItem = dte.SelectedItems.Item(1);
			if (selectedItem.Project != null)
			{
				return selectedItem.Project;
			}
			if (selectedItem.ProjectItem != null)
			{
				return selectedItem.ProjectItem.ContainingProject;
			}
			return null;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002530 File Offset: 0x00000730
		public static bool IsSolutionFolder(this Project item)
		{
			Guid guid = new Guid(item.Kind);
			return guid.Equals(ProjectInterfaceConverters.VsSolutionFolder);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002556 File Offset: 0x00000756
		public static IVsBuildPropertyStorage ToVsBuildPropertyStorage(this Project project)
		{
			return project.ToHierarchy() as IVsBuildPropertyStorage;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002564 File Offset: 0x00000764
		public static Guid GetProjectGuid(this Project project)
		{
			IVsHierarchy vsHierarchy = project.ToHierarchy();
			if (vsHierarchy != null)
			{
				IVsSolution2 vsSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution2;
				if (ErrorHandler.Succeeded(vsSolution.GetGuidOfProject(vsHierarchy, out var result)))
				{
					return result;
				}
			}
			return Guid.Empty;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000025A8 File Offset: 0x000007A8
		public static IVsHierarchy ToHierarchy(this Project project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			try
			{
				IVsSolution2 vsSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution2;
				if (vsSolution.GetProjectOfUniqueName(project.UniqueName, out var result) == 0)
				{
					return result;
				}
			}
			catch (NotImplementedException)
			{
				return null;
			}
			return null;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002608 File Offset: 0x00000808
		public static IVsProject3 ToVsProject(this Project project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			IVsProject3 vsProject = project.ToHierarchy() as IVsProject3;
			if (vsProject == null)
			{
				throw new ArgumentException("Project is not a VS project.");
			}
			return vsProject;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002640 File Offset: 0x00000840
		public static Project ToDteProject(this IVsHierarchy hierarchy)
		{
			if (hierarchy == null)
			{
				throw new ArgumentNullException("hierarchy");
			}
			if (hierarchy.GetProperty(4294967294U, -2027, out var obj) == 0)
			{
				return (Project)obj;
			}
			return null;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002676 File Offset: 0x00000876
		public static Project ToDteProject(this IVsProject project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			return (project as IVsHierarchy).ToDteProject();
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002691 File Offset: 0x00000891
		public static IEnumerable<Reference> GetReferences(this Project project)
		{
			VSProject vsproject = ProjectInterfaceConverters.ToVSProject(project);
			foreach (object obj in vsproject.References)
			{
				Reference reference = (Reference)obj;
				yield return reference;
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000026A4 File Offset: 0x000008A4
		public static bool References(this Project project, Project reference)
		{
			return project.GetReferences().Any((Reference r) => r.SourceProject == reference);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000026D8 File Offset: 0x000008D8
		private static VSProject ToVSProject(Project project)
		{
			VSProject vsproject = project.Object as VSProject;
			if (vsproject == null)
			{
				throw new ArgumentException("project is not a VSLangProj.VSProject");
			}
			return vsproject;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002700 File Offset: 0x00000900
		public static void AddReference(this Project project, Project toProject)
		{
			VSProject vsproject = ProjectInterfaceConverters.ToVSProject(project);
			vsproject.References.AddProject(toProject);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002724 File Offset: 0x00000924
		public static Guid ToGuid(this Project project)
		{
			IServiceProvider serviceProvider = new ServiceProvider(project.DTE as OLE_IServiceProvider);
			IVsSolution vsSolution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
			IVsHierarchy vsHierarchy = project.ToHierarchy();
			Guid result = default;
			if (vsHierarchy != null)
			{
				vsSolution.GetGuidOfProject(vsHierarchy, out result);
			}
			return result;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002778 File Offset: 0x00000978
		public static IVsHierarchy[] AllProjects(this IVsSolution solution)
		{
			List<IVsHierarchy> list = new List<IVsHierarchy>();
			if (solution == null)
			{
				return list.ToArray();
			}
			Guid empty = Guid.Empty;
			solution.GetProjectEnum(27U, ref empty, out var enumHierarchies);
			if (enumHierarchies != null)
			{
				IVsHierarchy[] array = new IVsHierarchy[1];
				while (enumHierarchies.Next(1U, array, out var _) == 0)
				{
					list.Add(array[0]);
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000027D4 File Offset: 0x000009D4
		public static Project[] GetProjects(this IVsSolution solution, string projectKind = null)
		{
			List<Project> list = new List<Project>();
			if (solution == null)
			{
				return list.ToArray();
			}
			Guid empty = Guid.Empty;
			solution.GetProjectEnum(27U, ref empty, out var enumHierarchies);
			if (enumHierarchies != null)
			{
				IVsHierarchy[] array = new IVsHierarchy[1];
				while (enumHierarchies.Next(1U, array, out var num) == 0)
				{
					array[0].GetProperty(4294967294U, -2027, out var obj);
					Project project = obj as Project;
					if (project != null)
					{
						if (string.IsNullOrEmpty(projectKind))
						{
							list.Add(project);
						}
						else if (projectKind.Equals(project.Kind, StringComparison.InvariantCultureIgnoreCase))
						{
							list.Add(project);
						}
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x0400001D RID: 29
		private static readonly Guid VsSolutionFolder = new Guid("{66A26720-8FB5-11D2-AA7E-00C04F688DDE}");
	}
}
