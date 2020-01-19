using System;
using EnvDTE;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000003 RID: 3
	internal static class EnvDTEProjectExtensions
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public static int? GetBuildAction(this ProjectItem item)
		{
			if (item == null)
			{
				return null;
			}
			Property property = EnvDTEProjectExtensions.FindProperty(item.Properties, "BuildAction");
			if (property == null)
			{
				return null;
			}
			return property.Value as int?;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020A0 File Offset: 0x000002A0
		public static string GetFullPath(this ProjectItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException();
			}
			return (string)item.Properties.Item("FullPath").Value;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020C8 File Offset: 0x000002C8
		public static Property FindProperty(Properties props, string name)
		{
			if (props != null)
			{
				foreach (object obj in props)
				{
					Property property = (Property)obj;
					if (property != null && !string.IsNullOrEmpty(property.Name) && string.Compare(name, property.Name, StringComparison.Ordinal) == 0)
					{
						return property;
					}
				}
			}
			return null;
		}
	}
}
