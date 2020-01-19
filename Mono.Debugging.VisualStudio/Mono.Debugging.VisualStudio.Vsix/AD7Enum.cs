using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000011 RID: 17
	public class AD7Enum<T, I> where I : class
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00003502 File Offset: 0x00001702
		public AD7Enum(T[] data)
		{
			this.m_data = data;
			this.m_position = 0U;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003518 File Offset: 0x00001718
		public int Clone(out I ppEnum)
		{
			ppEnum = default(I);
			return -2147467263;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003526 File Offset: 0x00001726
		public int GetCount(out uint pcelt)
		{
			pcelt = (uint)this.m_data.Length;
			return 0;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003533 File Offset: 0x00001733
		public int Next(uint celt, T[] rgelt, out uint celtFetched)
		{
			return this.Move(celt, rgelt, out celtFetched);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003540 File Offset: 0x00001740
		public int Reset()
		{
			int result;
			lock (this)
			{
				this.m_position = 0U;
				result = 0;
			}
			return result;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003580 File Offset: 0x00001780
		public int Skip(uint celt)
		{
			uint num;
			return this.Move(celt, null, out num);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003598 File Offset: 0x00001798
		private int Move(uint celt, T[] rgelt, out uint celtFetched)
		{
			int result;
			lock (this)
			{
				int num = 0;
				celtFetched = (uint)(this.m_data.Length - (int)this.m_position);
				if (celt > celtFetched)
				{
					num = 1;
				}
				else if (celt < celtFetched)
				{
					celtFetched = celt;
				}
				if (rgelt != null)
				{
					int num2 = 0;
					while ((long)num2 < (long)((ulong)celtFetched))
					{
						rgelt[num2] = this.m_data[(int)(checked((IntPtr)(unchecked((ulong)this.m_position + (ulong)((long)num2)))))];
						num2++;
					}
				}
				this.m_position += celtFetched;
				result = num;
			}
			return result;
		}

		// Token: 0x0400001B RID: 27
		private readonly T[] m_data;

		// Token: 0x0400001C RID: 28
		private uint m_position;
	}
}
