using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000034 RID: 52
	public class PendingBreakpointRequest
	{
		// Token: 0x060000BC RID: 188 RVA: 0x00003FE4 File Offset: 0x000021E4
		public PendingBreakpointRequest(IDebugBreakpointRequest2 request)
		{
			this.Request = request;
			BP_REQUEST_INFO[] array = new BP_REQUEST_INFO[1];
			Utils.RequireOk(request.GetRequestInfo(enum_BPREQI_FIELDS.BPREQI_CONDITION|enum_BPREQI_FIELDS.BPREQI_PASSCOUNT|enum_BPREQI_FIELDS.BPREQI_BPLOCATION, array));
			this.RequestInfo = array[0];
			enum_BP_LOCATION_TYPE[] array2 = new enum_BP_LOCATION_TYPE[1];
			Utils.RequireOk(request.GetLocationType(array2));
			this.LocationType = array2[0];
		}

		// Token: 0x04000043 RID: 67
		public readonly IDebugBreakpointRequest2 Request;

		// Token: 0x04000044 RID: 68
		public readonly BP_REQUEST_INFO RequestInfo;

		// Token: 0x04000045 RID: 69
		public readonly enum_BP_LOCATION_TYPE LocationType;
	}
}
