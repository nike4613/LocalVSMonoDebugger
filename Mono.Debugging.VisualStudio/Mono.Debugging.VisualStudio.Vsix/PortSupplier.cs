using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000045 RID: 69
	[ComVisible(true)]
	[Guid("A2C0CC70-C265-4807-901D-2E5A6378BF43")]
	public class PortSupplier : IDebugPortSupplier2
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00005A86 File Offset: 0x00003C86
		// (set) Token: 0x06000151 RID: 337 RVA: 0x00005A8D File Offset: 0x00003C8D
		public static Port MainPort { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00005A95 File Offset: 0x00003C95
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00005A9D File Offset: 0x00003C9D
		public IDebugCoreServer2 Server { get; private set; }

		// Token: 0x06000154 RID: 340 RVA: 0x00005AA6 File Offset: 0x00003CA6
		public PortSupplier()
		{
			PortSupplier.MainPort = new Port(this);
			this.ports.Add(PortSupplier.MainPort);
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00005AD4 File Offset: 0x00003CD4
		protected Guid PortSupplierClassGuid
		{
			get
			{
				return Guids.PortSupplierClassGuid;
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00005ADC File Offset: 0x00003CDC
		public int AddPort(IDebugPortRequest2 request, out IDebugPort2 debug_port)
		{
			Utils.RequireOk(request.GetPortName(out var _));
			debug_port = PortSupplier.MainPort;
			return 0;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00003244 File Offset: 0x00001444
		public int CanAddPort()
		{
			return 0;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00004B88 File Offset: 0x00002D88
		public int EnumPorts(out IEnumDebugPorts2 ppEnum)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00005B00 File Offset: 0x00003D00
		public int GetPort(ref Guid guidPort, out IDebugPort2 port)
		{
			foreach (Port port2 in this.ports)
			{
				if (port2.Guid == guidPort)
				{
					port = port2;
					return 0;
				}
			}
			port = null;
			return 1;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00005B6C File Offset: 0x00003D6C
		public int GetPortSupplierId(out Guid portSupplierGuid)
		{
			portSupplierGuid = this.PortSupplierClassGuid;
			return 0;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00005B7B File Offset: 0x00003D7B
		public int GetPortSupplierName(out string name)
		{
			name = "MonoPortSupplier";
			return 0;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00004B88 File Offset: 0x00002D88
		public int RemovePort(IDebugPort2 pPort)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040000A0 RID: 160
		private List<Port> ports = new List<Port>();

		// Token: 0x040000A3 RID: 163
		public const string PortSupplierName = "MonoPortSupplier";
	}
}
