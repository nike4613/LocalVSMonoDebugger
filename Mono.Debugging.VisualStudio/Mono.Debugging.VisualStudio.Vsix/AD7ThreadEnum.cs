using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000015 RID: 21
	public class AD7ThreadEnum : AD7Enum<IDebugThread2, IEnumDebugThreads2>, IEnumDebugThreads2
	{
		// Token: 0x0600006A RID: 106 RVA: 0x00003665 File Offset: 0x00001865
		public AD7ThreadEnum(IDebugThread2[] threads) : base(threads)
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000366E File Offset: 0x0000186E
		public AD7ThreadEnum(IEnumerable<IDebugThread2> threads) : base(threads.ToArray<IDebugThread2>())
		{
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000367C File Offset: 0x0000187C
		public new int Next(uint celt, IDebugThread2[] rgelt, ref uint celtFetched)
		{
			return base.Next(celt, rgelt, out celtFetched);
		}
	}
}
