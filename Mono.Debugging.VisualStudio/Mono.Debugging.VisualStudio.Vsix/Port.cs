using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000041 RID: 65
	public class Port : IDebugPort2, IDebugPortNotify2, IConnectionPointContainer
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00005978 File Offset: 0x00003B78
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00005980 File Offset: 0x00003B80
		public Port.DebugPortEvents2ConnectionPoint PortEventsCP { get; private set; }

		// Token: 0x0600013D RID: 317 RVA: 0x00005989 File Offset: 0x00003B89
		public Port(PortSupplier supplier)
		{
			this.PortSupplier = supplier;
			this.Guid = Guid.NewGuid();
			this.PortEventsCP = new Port.DebugPortEvents2ConnectionPoint(this);
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600013E RID: 318 RVA: 0x000059BA File Offset: 0x00003BBA
		// (set) Token: 0x0600013F RID: 319 RVA: 0x000059C2 File Offset: 0x00003BC2
		public PortSupplier PortSupplier { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000140 RID: 320 RVA: 0x000059CB File Offset: 0x00003BCB
		// (set) Token: 0x06000141 RID: 321 RVA: 0x000059D3 File Offset: 0x00003BD3
		public Guid Guid { get; private set; }

		// Token: 0x06000142 RID: 322 RVA: 0x00004B88 File Offset: 0x00002D88
		public int EnumProcesses(out IEnumDebugProcesses2 ppEnum)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000143 RID: 323 RVA: 0x000059DC File Offset: 0x00003BDC
		public int GetPortId(out Guid guidport)
		{
			guidport = this.Guid;
			return 0;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000059EB File Offset: 0x00003BEB
		public int GetPortName(out string name)
		{
			name = "Mono";
			return 0;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetPortRequest(out IDebugPortRequest2 ppRequest)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000146 RID: 326 RVA: 0x000059F5 File Offset: 0x00003BF5
		public int GetPortSupplier(out IDebugPortSupplier2 portSupplier)
		{
			portSupplier = this.PortSupplier;
			return 0;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetProcess(AD_PROCESS_ID ProcessId, out IDebugProcess2 ppProcess)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00005A00 File Offset: 0x00003C00
		public int AddProgramNode(IDebugProgramNode2 node)
		{
			this.nodes.Add(node);
			return 0;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00005A0F File Offset: 0x00003C0F
		public int RemoveProgramNode(IDebugProgramNode2 node)
		{
			this.nodes.Remove(node);
			return 0;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00004B88 File Offset: 0x00002D88
		public void EnumConnectionPoints(out IEnumConnectionPoints ppEnum)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00005A1F File Offset: 0x00003C1F
		public void FindConnectionPoint(ref Guid riid, out IConnectionPoint ppCP)
		{
			ppCP = null;
			if (riid == typeof(IDebugPortEvents2).GUID)
			{
				ppCP = this.PortEventsCP;
				return;
			}
			throw new NotImplementedException();
		}

		// Token: 0x04000098 RID: 152
		private List<IDebugProgramNode2> nodes = new List<IDebugProgramNode2>();

		// Token: 0x0400009A RID: 154
		private const string MonoPortName = "Mono";

		// Token: 0x0200005E RID: 94
		public class DebugPortEvents2ConnectionPoint : IConnectionPoint
		{
			// Token: 0x06000205 RID: 517 RVA: 0x00007091 File Offset: 0x00005291
			public DebugPortEvents2ConnectionPoint(IConnectionPointContainer container)
			{
				this.container = container;
			}

			// Token: 0x06000206 RID: 518 RVA: 0x000070AC File Offset: 0x000052AC
			public void Event(IDebugCoreServer2 pServer, IDebugPort2 pPort, IDebugProcess2 pProcess, IDebugProgram2 pProgram, IDebugEvent2 pEvent, ref Guid riidEvent)
			{
				IDebugPortEvents2[] array = this.sinks.ToArray();
				foreach (IDebugPortEvents2 debugPortEvents in array)
				{
					if (debugPortEvents != null)
					{
						debugPortEvents.Event(pServer, pPort, pProcess, pProgram, pEvent, ref riidEvent);
					}
				}
			}

			// Token: 0x06000207 RID: 519 RVA: 0x000070EC File Offset: 0x000052EC
			public void Advise(object pUnkSink, out int pdwCookie)
			{
				IDebugPortEvents2 debugPortEvents = pUnkSink as IDebugPortEvents2;
				if (debugPortEvents != null)
				{
					this.sinks.Add(debugPortEvents);
					pdwCookie = this.sinks.Count;
					return;
				}
				throw new ArgumentException();
			}

			// Token: 0x06000208 RID: 520 RVA: 0x00004B88 File Offset: 0x00002D88
			public void EnumConnections(out IEnumConnections ppEnum)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06000209 RID: 521 RVA: 0x00007122 File Offset: 0x00005322
			public void GetConnectionInterface(out Guid pIID)
			{
				pIID = typeof(IDebugPortEvents2).GUID;
			}

			// Token: 0x0600020A RID: 522 RVA: 0x00007139 File Offset: 0x00005339
			public void GetConnectionPointContainer(out IConnectionPointContainer ppCPC)
			{
				ppCPC = this.container;
			}

			// Token: 0x0600020B RID: 523 RVA: 0x00007143 File Offset: 0x00005343
			public void Unadvise(int dwCookie)
			{
				if (dwCookie > 0 && dwCookie <= this.sinks.Count)
				{
					this.sinks[dwCookie - 1] = null;
				}
			}

			// Token: 0x040000E9 RID: 233
			private IConnectionPointContainer container;

			// Token: 0x040000EA RID: 234
			private List<IDebugPortEvents2> sinks = new List<IDebugPortEvents2>();
		}
	}
}
