using System;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000049 RID: 73
	[Serializable]
	public struct TargetAddress
	{
		// Token: 0x0600019C RID: 412 RVA: 0x000062E0 File Offset: 0x000044E0
		public TargetAddress(ulong address)
		{
			this.Address = address;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x000062E0 File Offset: 0x000044E0
		public TargetAddress(long address)
		{
			this.Address = (ulong)address;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000062EC File Offset: 0x000044EC
		public override string ToString()
		{
			long address = (long)this.Address;
			return string.Format("0x{0}", TargetAddress.FormatAddress(address));
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00006310 File Offset: 0x00004510
		public static string FormatAddress(long address)
		{
			int num = 8;
			string text = address.ToString("x");
			for (int i = text.Length; i < num; i++)
			{
				text = "0" + text;
			}
			return text;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000634C File Offset: 0x0000454C
		public int CompareTo(object obj)
		{
			TargetAddress targetAddress = (TargetAddress)obj;
			if (this.Address < targetAddress.Address)
			{
				return -1;
			}
			if (this.Address > targetAddress.Address)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00006384 File Offset: 0x00004584
		public override bool Equals(object o)
		{
			if (o == null || !(o is TargetAddress))
			{
				return false;
			}
			TargetAddress targetAddress = (TargetAddress)o;
			return this.Address == targetAddress.Address;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x000063B3 File Offset: 0x000045B3
		public override int GetHashCode()
		{
			return (int)this.Address;
		}

		// Token: 0x040000B8 RID: 184
		public readonly ulong Address;
	}
}
