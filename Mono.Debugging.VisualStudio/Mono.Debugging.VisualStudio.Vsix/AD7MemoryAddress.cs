using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000032 RID: 50
	public class AD7MemoryAddress : IDebugCodeContext2, IDebugMemoryContext2
	{
		// Token: 0x060000AD RID: 173 RVA: 0x00003AE8 File Offset: 0x00001CE8
		public AD7MemoryAddress(Engine engine, ulong address)
		{
			this.m_engine = engine;
			this.m_address = address;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00003AFE File Offset: 0x00001CFE
		public ulong Address
		{
			get
			{
				return this.m_address;
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003B06 File Offset: 0x00001D06
		public void SetDocumentContext(IDebugDocumentContext2 docContext)
		{
			this.m_documentContext = docContext;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00003B0F File Offset: 0x00001D0F
		public int Add(ulong dwCount, out IDebugMemoryContext2 newAddress)
		{
			newAddress = new AD7MemoryAddress(this.m_engine, (ulong)((uint)dwCount) + this.m_address);
			return 0;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00003B2C File Offset: 0x00001D2C
		public int Compare(enum_CONTEXT_COMPARE contextCompare, IDebugMemoryContext2[] compareToItems, uint compareToLength, out uint foundIndex)
		{
			foundIndex = uint.MaxValue;
			int result;
			try
			{
				for (uint num = 0U; num < compareToLength; num += 1U)
				{
					AD7MemoryAddress ad7MemoryAddress = compareToItems[(int)num] as AD7MemoryAddress;
					if (ad7MemoryAddress != null && this.m_engine == ad7MemoryAddress.m_engine)
					{
						bool flag;
						switch (contextCompare)
						{
						case enum_CONTEXT_COMPARE.CONTEXT_EQUAL:
							flag = (this.m_address == ad7MemoryAddress.m_address);
							break;
						case enum_CONTEXT_COMPARE.CONTEXT_LESS_THAN:
							flag = (this.m_address < ad7MemoryAddress.m_address);
							break;
						case enum_CONTEXT_COMPARE.CONTEXT_GREATER_THAN:
							flag = (this.m_address > ad7MemoryAddress.m_address);
							break;
						case enum_CONTEXT_COMPARE.CONTEXT_LESS_THAN_OR_EQUAL:
							flag = (this.m_address <= ad7MemoryAddress.m_address);
							break;
						case enum_CONTEXT_COMPARE.CONTEXT_GREATER_THAN_OR_EQUAL:
							flag = (this.m_address >= ad7MemoryAddress.m_address);
							break;
						case enum_CONTEXT_COMPARE.CONTEXT_SAME_SCOPE:
						case enum_CONTEXT_COMPARE.CONTEXT_SAME_FUNCTION:
							flag = (this.m_address == ad7MemoryAddress.m_address);
							break;
						case enum_CONTEXT_COMPARE.CONTEXT_SAME_MODULE:
							goto IL_C9;
						case enum_CONTEXT_COMPARE.CONTEXT_SAME_PROCESS:
							flag = true;
							break;
						default:
							goto IL_C9;
						}
						if (flag)
						{
							foundIndex = num;
							return 0;
						}
						goto IL_DC;
						IL_C9:
						return -2147467263;
					}
					IL_DC:;
				}
				result = 1;
			}
			catch (Exception e)
			{
				result = Utils.UnexpectedException(e);
			}
			return result;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00003C44 File Offset: 0x00001E44
		public int GetInfo(enum_CONTEXT_INFO_FIELDS dwFields, CONTEXT_INFO[] pinfo)
		{
			int result;
			try
			{
				pinfo[0].dwFields = 0;
				if ((dwFields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESS) != 0)
				{
					pinfo[0].bstrAddress = this.m_address.ToString();
					pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_ADDRESS;
				}
				result = 0;
			}
			catch (Exception e)
			{
				result = Utils.UnexpectedException(e);
			}
			return result;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00003CC4 File Offset: 0x00001EC4
		public int GetName(out string pbstrName)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003CD0 File Offset: 0x00001ED0
		public int Subtract(ulong dwCount, out IDebugMemoryContext2 ppMemCxt)
		{
			ppMemCxt = new AD7MemoryAddress(this.m_engine, (ulong)((uint)dwCount) - this.m_address);
			return 0;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003CEA File Offset: 0x00001EEA
		public int GetDocumentContext(out IDebugDocumentContext2 ppSrcCxt)
		{
			ppSrcCxt = this.m_documentContext;
			return 0;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00003CF5 File Offset: 0x00001EF5
		public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
		{
			if (this.m_documentContext != null)
			{
				this.m_documentContext.GetLanguageInfo(ref pbstrLanguage, ref pguidLanguage);
				return 0;
			}
			return 1;
		}

		// Token: 0x0400003E RID: 62
		private readonly Engine m_engine;

		// Token: 0x0400003F RID: 63
		private readonly ulong m_address;

		// Token: 0x04000040 RID: 64
		private IDebugDocumentContext2 m_documentContext;
	}
}
