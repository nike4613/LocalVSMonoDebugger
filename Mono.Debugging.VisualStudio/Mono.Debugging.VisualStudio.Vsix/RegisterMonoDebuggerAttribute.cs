using System;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200004D RID: 77
	public class RegisterMonoDebuggerAttribute : RegistrationAttribute
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x000065B7 File Offset: 0x000047B7
		protected string DebuggerName
		{
			get
			{
				return "Mono.Debugger";
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x000065BE File Offset: 0x000047BE
		protected string EngineAssemblyFileName
		{
			get
			{
				return Path.GetFileName(typeof(Engine).Assembly.Location);
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x000065D9 File Offset: 0x000047D9
		protected string EngineAssemblyName
		{
			get
			{
				return typeof(Engine).Assembly.FullName;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x000065EF File Offset: 0x000047EF
		protected string EngineGuid
		{
			get
			{
				return "9E1626AE-7DB7-4138-AC41-641D55CF9A4A";
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x000065F6 File Offset: 0x000047F6
		protected string EngineClassGuid
		{
			get
			{
				return "C094C059-1786-49CD-8EB9-9C0EF6CA5454";
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x000065FD File Offset: 0x000047FD
		protected string EngineClassName
		{
			get
			{
				return typeof(Engine).FullName;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000660E File Offset: 0x0000480E
		protected string PortSupplierGuid
		{
			get
			{
				return "FCEE747D-E866-4221-BD96-99C30E4B67B9";
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x00006615 File Offset: 0x00004815
		protected string PortSupplierClassGuid
		{
			get
			{
				return "A2C0CC70-C265-4807-901D-2E5A6378BF43";
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x0000661C File Offset: 0x0000481C
		protected string PortSupplierClassName
		{
			get
			{
				return typeof(PortSupplier).FullName;
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000662D File Offset: 0x0000482D
		public override void Register(RegistrationAttribute.RegistrationContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.RegisterCommon(context);
			this.RegisterSoftEngine(context);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000664B File Offset: 0x0000484B
		public override void Unregister(RegistrationAttribute.RegistrationContext context)
		{
			if (context == null)
			{
				return;
			}
			this.UnregisterCommon(context);
			this.UnregisterSoft(context);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00006660 File Offset: 0x00004860
		private void RegisterCommon(RegistrationAttribute.RegistrationContext context)
		{
			string inprocServerPath = context.InprocServerPath;
			string text = Path.Combine(context.ComponentPath, this.EngineAssemblyFileName);
			using (RegistrationAttribute.Key key = context.CreateKey(string.Format("AD7Metrics\\PortSupplier\\{0}", RegisterMonoDebuggerAttribute.FormatGuid(this.PortSupplierGuid))))
			{
				key.SetValue("CLSID", RegisterMonoDebuggerAttribute.FormatGuid(this.PortSupplierClassGuid));
				key.SetValue("Name", this.DebuggerName);
			}
			using (RegistrationAttribute.Key key2 = context.CreateKey(string.Format("CLSID\\{0}", RegisterMonoDebuggerAttribute.FormatGuid(this.PortSupplierClassGuid))))
			{
				key2.SetValue("InprocServer32", inprocServerPath);
				key2.SetValue("Class", this.PortSupplierClassName);
				key2.SetValue("CodeBase", text);
				key2.SetValue("Assembly", this.EngineAssemblyName);
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00006754 File Offset: 0x00004954
		private void UnregisterCommon(RegistrationAttribute.RegistrationContext context)
		{
			context.RemoveKey(string.Format("AD7Metrics\\PortSupplier\\{0}", this.PortSupplierGuid));
			context.RemoveKey(string.Format("CLSID\\{0}", this.PortSupplierClassGuid));
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00006784 File Offset: 0x00004984
		private void RegisterSoftEngine(RegistrationAttribute.RegistrationContext context)
		{
			string inprocServerPath = context.InprocServerPath;
			string text = Path.Combine(context.ComponentPath, this.EngineAssemblyFileName);
			using (RegistrationAttribute.Key key = context.CreateKey(string.Format("AD7Metrics\\Engine\\{0}", RegisterMonoDebuggerAttribute.FormatGuid(this.EngineGuid))))
			{
				key.SetValue("CLSID", RegisterMonoDebuggerAttribute.FormatGuid(this.EngineClassGuid));
				key.SetValue("Name", this.EngineClassName);
				key.SetValue("PortSupplier", RegisterMonoDebuggerAttribute.FormatGuid(this.PortSupplierClassGuid));
				key.SetValue("CallstackBP", 1);
				key.SetValue("AutoselectPriority", 4);
				key.SetValue("Attach", 0);
				key.SetValue("AddressBP", 0);
				key.SetValue("Disassembly", 0);
				key.SetValue("RemotingDebugging", 0);
				key.SetValue("Exceptions", 1);
			}
			using (RegistrationAttribute.Key key2 = context.CreateKey(string.Format("CLSID\\{0}", RegisterMonoDebuggerAttribute.FormatGuid(this.EngineClassGuid))))
			{
				key2.SetValue("InprocServer32", inprocServerPath);
				key2.SetValue("Class", this.EngineClassName);
				key2.SetValue("CodeBase", text);
				key2.SetValue("Assembly", this.EngineAssemblyName);
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00006904 File Offset: 0x00004B04
		private void UnregisterSoft(RegistrationAttribute.RegistrationContext context)
		{
			context.RemoveKey(string.Format("AD7Metrics\\Engine\\{0}", this.EngineGuid));
			context.RemoveKey(string.Format("CLSID\\{0}", this.EngineClassGuid));
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00006934 File Offset: 0x00004B34
		private static string FormatGuid(string guid)
		{
			Guid guid2 = new Guid(guid);
			return string.Format("{{{0}}}", guid2.ToString());
		}
	}
}
