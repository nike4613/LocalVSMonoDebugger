using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000017 RID: 23
	public class AD7PropertyEnum : AD7Enum<DEBUG_PROPERTY_INFO, IEnumDebugPropertyInfo2>, IEnumDebugPropertyInfo2
	{
		// Token: 0x0600006F RID: 111 RVA: 0x0000365C File Offset: 0x0000185C
		public AD7PropertyEnum(DEBUG_PROPERTY_INFO[] properties) : base(properties)
		{
		}
	}
}
