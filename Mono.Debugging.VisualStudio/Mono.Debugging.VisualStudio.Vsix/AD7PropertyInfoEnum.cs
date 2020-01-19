using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000014 RID: 20
	public class AD7PropertyInfoEnum : AD7Enum<DEBUG_PROPERTY_INFO, IEnumDebugPropertyInfo2>, IEnumDebugPropertyInfo2
	{
		// Token: 0x06000069 RID: 105 RVA: 0x0000365C File Offset: 0x0000185C
		public AD7PropertyInfoEnum(DEBUG_PROPERTY_INFO[] data) : base(data)
		{
		}
	}
}
