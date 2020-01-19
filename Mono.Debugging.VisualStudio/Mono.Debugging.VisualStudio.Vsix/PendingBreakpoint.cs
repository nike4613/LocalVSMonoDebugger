using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000035 RID: 53
	public class PendingBreakpoint : IDebugPendingBreakpoint2
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000BD RID: 189 RVA: 0x0000403F File Offset: 0x0000223F
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00004047 File Offset: 0x00002247
		internal IBreakpointsAdapter Engine { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00004050 File Offset: 0x00002250
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x00004058 File Offset: 0x00002258
		public PendingBreakpointRequest Request { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00004061 File Offset: 0x00002261
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00004069 File Offset: 0x00002269
		public int HitCount { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00004072 File Offset: 0x00002272
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x0000407A File Offset: 0x0000227A
		public HitCountMode HitCountMode { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004083 File Offset: 0x00002283
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x0000408B File Offset: 0x0000228B
		public bool BreakIfConditionChanges { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00004094 File Offset: 0x00002294
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x0000409C File Offset: 0x0000229C
		public string ConditionExpression { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x000040A5 File Offset: 0x000022A5
		// (set) Token: 0x060000CA RID: 202 RVA: 0x000040AD File Offset: 0x000022AD
		internal IList<AD7BoundBreakpoint> BoundBreakpoints { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000CB RID: 203 RVA: 0x000040B6 File Offset: 0x000022B6
		// (set) Token: 0x060000CC RID: 204 RVA: 0x000040BE File Offset: 0x000022BE
		internal Breakpoint Handle { get; private set; }

		// Token: 0x060000CD RID: 205 RVA: 0x000040C7 File Offset: 0x000022C7
		internal PendingBreakpoint(IBreakpointsAdapter engine, IEventSender sender, Process process, PendingBreakpointRequest request)
		{
			this.Engine = engine;
			this.Request = request;
			this.eventSender = sender;
			this.process = process;
			this.BoundBreakpoints = new List<AD7BoundBreakpoint>();
			this.is_enabled = false;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00004100 File Offset: 0x00002300
		protected bool DoBind()
		{
			if (this.Request.LocationType == 65537)
			{
				if ((this.Request.RequestInfo.dwFields & 1) == null)
				{
					this.OnBreakpointError("Breakpoints of this type are not supported.");
					return false;
				}
				IDebugDocumentPosition2 debugDocumentPosition = (IDebugDocumentPosition2)Marshal.GetObjectForIUnknown(this.Request.RequestInfo.bpLocation.unionmember2);
				string file;
				Utils.RequireOk(debugDocumentPosition.GetFileName(ref file));
				TEXT_POSITION[] array = new TEXT_POSITION[1];
				TEXT_POSITION[] array2 = new TEXT_POSITION[1];
				Utils.RequireOk(debugDocumentPosition.GetRange(array, array2));
				SourceRange range = new SourceRange((int)(array[0].dwLine + 1U), (int)(array2[0].dwLine + 1U), (int)(array[0].dwColumn + 1U), (int)(array2[0].dwColumn + 1U));
				SourceLocation location = new SourceLocation(file, (int)(array[0].dwLine + 1U), range);
				if ((this.Request.RequestInfo.dwFields & 64) == 64)
				{
					this.HitCount = (int)this.Request.RequestInfo.bpPassCount.dwPassCount;
					switch (this.Request.RequestInfo.bpPassCount.stylePassCount)
					{
					case 1:
						this.HitCountMode = HitCountMode.EqualTo;
						break;
					case 2:
						this.HitCountMode = HitCountMode.GreaterThanOrEqualTo;
						break;
					case 3:
						this.HitCountMode = HitCountMode.MultipleOf;
						break;
					default:
						this.HitCountMode = HitCountMode.None;
						break;
					}
				}
				if ((this.Request.RequestInfo.dwFields & 128) == 128)
				{
					this.ConditionExpression = this.Request.RequestInfo.bpCondition.bstrCondition;
					this.BreakIfConditionChanges = (this.Request.RequestInfo.bpCondition.styleCondition == 2);
				}
				this.Handle = this.Engine.BindBreakpoint(this, location);
				if (this.Handle == null)
				{
					return false;
				}
				this.Handle.Enabled = this.is_enabled;
				return true;
			}
			else
			{
				if (this.Request.LocationType == 131073)
				{
					this.OnBreakpointError("Function breakpoints are not supported yet.");
					return false;
				}
				this.OnBreakpointError("Breakpoints of this type are not supported.");
				return false;
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000431A File Offset: 0x0000251A
		internal void OnBreakpointBound(AD7BoundBreakpoint bound)
		{
			this.BoundBreakpoints.Add(bound);
			this.eventSender.SendEvent(new BreakpointBoundEvent(bound));
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000433C File Offset: 0x0000253C
		internal void OnBreakpointError(string error_msg)
		{
			if (this.error_breakpoint != null)
			{
				return;
			}
			ErrorBreakpointResolution error = new ErrorBreakpointResolution(this.process, error_msg);
			this.error_breakpoint = new ErrorBreakpoint(this, error);
			this.eventSender.SendEvent(new BreakpointErrorEvent(this.error_breakpoint));
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00004382 File Offset: 0x00002582
		internal void OnBreakpointUnbound(AD7BoundBreakpoint bound)
		{
			this.BoundBreakpoints.Remove(bound);
			this.eventSender.SendEvent(new BreakpointUnboundEvent(bound));
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000043A4 File Offset: 0x000025A4
		public int CanBind(out IEnumDebugErrorBreakpoints2 error_breakpoints)
		{
			if (this.error_breakpoint != null)
			{
				error_breakpoints = new AD7ErrorBreakpointsEnum(new IDebugErrorBreakpoint2[]
				{
					this.error_breakpoint
				});
				return 1;
			}
			if (this.Request.LocationType == 65537)
			{
				if ((this.Request.RequestInfo.dwFields & 1) == null)
				{
					this.error_breakpoint = new ErrorBreakpoint(this, this.process, "Breakpoints of this type are not supported.");
					error_breakpoints = new AD7ErrorBreakpointsEnum(new IDebugErrorBreakpoint2[]
					{
						this.error_breakpoint
					});
					return 1;
				}
				error_breakpoints = null;
				return 0;
			}
			else
			{
				if (this.Request.LocationType == 131073)
				{
					this.error_breakpoint = new ErrorBreakpoint(this, this.process, "Function breakpoints are not supported yet, see bug #673920.");
					error_breakpoints = new AD7ErrorBreakpointsEnum(new IDebugErrorBreakpoint2[]
					{
						this.error_breakpoint
					});
					return 1;
				}
				error_breakpoints = null;
				return 0;
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00004470 File Offset: 0x00002670
		public int Bind()
		{
			int result;
			try
			{
				result = (this.DoBind() ? 0 : 1);
			}
			catch (Exception ex)
			{
				Utils.Message("Exception in Bind: {0}", new object[]
				{
					ex
				});
				result = 1;
			}
			return result;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000044B8 File Offset: 0x000026B8
		public int Delete()
		{
			int result;
			try
			{
				if (this.Handle != null)
				{
					this.Engine.DeletePendingBreakpoint(this);
					this.Handle = null;
				}
				result = 0;
			}
			catch (Exception ex)
			{
				Utils.Message("Exception in Delete: {0}", new object[]
				{
					ex
				});
				result = 1;
			}
			return result;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00004510 File Offset: 0x00002710
		public int Enable(int enable)
		{
			bool flag = enable != 0;
			if (flag != this.is_enabled)
			{
				this.is_enabled = flag;
				if (this.Handle == null)
				{
					return 0;
				}
				this.Handle.Enabled = flag;
			}
			return 0;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000454C File Offset: 0x0000274C
		public int EnumBoundBreakpoints(out IEnumDebugBoundBreakpoints2 ppEnum)
		{
			ppEnum = new AD7BoundBreakpointsEnum(this.BoundBreakpoints.Cast<IDebugBoundBreakpoint2>().ToArray<IDebugBoundBreakpoint2>());
			return 0;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00004566 File Offset: 0x00002766
		public int EnumErrorBreakpoints(enum_BP_ERROR_TYPE bpErrorType, out IEnumDebugErrorBreakpoints2 ppEnum)
		{
			if (this.error_breakpoint != null)
			{
				ppEnum = new AD7ErrorBreakpointsEnum(new IDebugErrorBreakpoint2[]
				{
					this.error_breakpoint
				});
			}
			else
			{
				ppEnum = new AD7ErrorBreakpointsEnum(new IDebugErrorBreakpoint2[0]);
			}
			return 0;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00004596 File Offset: 0x00002796
		public int GetBreakpointRequest(out IDebugBreakpointRequest2 ppBPRequest)
		{
			ppBPRequest = this.Request.Request;
			return 0;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000045A8 File Offset: 0x000027A8
		public int GetState(PENDING_BP_STATE_INFO[] pState)
		{
			enum_PENDING_BP_STATE state;
			if (this.Handle == null)
			{
				state = 1;
			}
			else if (this.Handle.Enabled)
			{
				state = 3;
			}
			else
			{
				state = 2;
			}
			pState[0].state = state;
			pState[0].Flags = 0;
			return 0;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000045F1 File Offset: 0x000027F1
		public virtual int SetCondition(BP_CONDITION bpCondition)
		{
			return -2147467263;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000045F1 File Offset: 0x000027F1
		public virtual int SetPassCount(BP_PASSCOUNT bpPassCount)
		{
			return -2147467263;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000045F1 File Offset: 0x000027F1
		public virtual int Virtualize(int fVirtualize)
		{
			return -2147467263;
		}

		// Token: 0x0400004E RID: 78
		private ErrorBreakpoint error_breakpoint;

		// Token: 0x0400004F RID: 79
		private bool is_enabled;

		// Token: 0x04000050 RID: 80
		private IEventSender eventSender;

		// Token: 0x04000051 RID: 81
		private Process process;
	}
}
