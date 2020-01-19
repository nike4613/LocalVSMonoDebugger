using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugger.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000033 RID: 51
	internal class AD7Module : IDebugModule2
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x00003D10 File Offset: 0x00001F10
		public AD7Module(AssemblyMirror assembly, StartInfo startInfo)
		{
			this.assembly = assembly;
			this.startInfo = startInfo;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00003D28 File Offset: 0x00001F28
		public int GetInfo(enum_MODULE_INFO_FIELDS dwFields, MODULE_INFO[] pinfo)
		{
			int result;
			try
			{
				MODULE_INFO module_INFO = default(MODULE_INFO);
				if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_NAME) != 0)
				{
					module_INFO.m_bstrName = Path.GetFileName(this.assembly.Location);
					module_INFO.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_NAME;
				}
				bool flag = false;
				if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_URL) != 0)
				{
					module_INFO.m_bstrUrl = this.FindLocalLocation(this.assembly);
					if (!string.IsNullOrEmpty(module_INFO.m_bstrUrl))
					{
						flag = true;
					}
					module_INFO.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_URL;
				}
				if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_VERSION) != 0)
				{
					module_INFO.m_bstrVersion = this.assembly.GetName().Version.ToString();
					module_INFO.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_VERSION;
				}
				if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_LOADADDRESS) != 0)
				{
					module_INFO.m_addrLoadAddress = (ulong)this.assembly.GetAssemblyObject().Address;
					module_INFO.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_LOADADDRESS;
					if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_PREFFEREDADDRESS) != 0)
					{
						module_INFO.m_addrPreferredLoadAddress = module_INFO.m_addrLoadAddress;
						module_INFO.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_PREFFEREDADDRESS;
					}
				}
				string text = this.FindSymbolLocation(this.assembly);
				if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_URLSYMBOLLOCATION) != 0 && text != null)
				{
					module_INFO.m_bstrUrlSymbolLocation = text;
					module_INFO.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_URLSYMBOLLOCATION;
				}
				if ((dwFields & enum_MODULE_INFO_FIELDS.MIF_FLAGS) != 0)
				{
					module_INFO.m_dwModuleFlags = 0;
					if (text != null)
					{
						module_INFO.m_dwModuleFlags |= enum_MODULE_FLAGS.MODULE_FLAG_SYMBOLS;
					}
					else if (flag && (dwFields & enum_MODULE_INFO_FIELDS.MIF_DEBUGMESSAGE) != 0)
					{
						module_INFO.m_bstrDebugMessage = "Symbols loaded in VM";
						module_INFO.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_DEBUGMESSAGE;
					}
					if (this.assembly.GetName().ProcessorArchitecture == ProcessorArchitecture.IA64)
					{
						module_INFO.m_dwModuleFlags |= enum_MODULE_FLAGS.MODULE_FLAG_64BIT;
					}
					module_INFO.dwValidFields |= enum_MODULE_INFO_FIELDS.MIF_FLAGS;
				}
				pinfo[0] = module_INFO;
				result = 0;
			}
			catch (Exception ex)
			{
				result = ex.HResult;
			}
			return result;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00003EE8 File Offset: 0x000020E8
		private string FindLocalLocation(AssemblyMirror assembly)
		{
			AssemblyName name = assembly.GetName();
			if (name != null)
			{
				string fullName = name.FullName;
				if (fullName != null && this.startInfo.AssemblyPathMap.ContainsKey(fullName))
				{
					string value = this.startInfo.AssemblyPathMap.FirstOrDefault((KeyValuePair<string, string> a) => a.Key == assembly.GetName().FullName).Value;
					if (File.Exists(value))
					{
						return value;
					}
				}
			}
			return null;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00003F60 File Offset: 0x00002160
		private string FindSymbolLocation(AssemblyMirror assembly)
		{
			AssemblyName name = assembly.GetName();
			if (name != null)
			{
				string fullName = name.FullName;
				if (fullName != null && this.startInfo.AssemblyPathMap.ContainsKey(fullName))
				{
					string text = this.startInfo.AssemblyPathMap.FirstOrDefault((KeyValuePair<string, string> a) => a.Key == assembly.GetName().FullName).Value + ".mdb";
					if (File.Exists(text))
					{
						return text;
					}
				}
			}
			return null;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000030EC File Offset: 0x000012EC
		public int ReloadSymbols_Deprecated(string pszUrlToSymbols, out string pbstrDebugMessage)
		{
			pbstrDebugMessage = null;
			return -2147467263;
		}

		// Token: 0x04000041 RID: 65
		private AssemblyMirror assembly;

		// Token: 0x04000042 RID: 66
		private StartInfo startInfo;
	}
}
