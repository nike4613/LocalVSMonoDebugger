using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200000F RID: 15
	public class AD7DocumentContext : IDebugDocumentContext2
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00002FB0 File Offset: 0x000011B0
		public AD7DocumentContext(string fileName, TEXT_POSITION begPos, TEXT_POSITION endPos, AD7MemoryAddress codeContext)
		{
			this.m_fileName = fileName;
			this.m_begPos = begPos;
			this.m_endPos = endPos;
			this.m_codeContext = codeContext;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002FD5 File Offset: 0x000011D5
		int IDebugDocumentContext2.Compare(enum_DOCCONTEXT_COMPARE Compare, IDebugDocumentContext2[] rgpDocContextSet, uint dwDocContextSetLen, out uint pdwDocContext)
		{
			pdwDocContext = 0U;
			return -2147467263;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002FE4 File Offset: 0x000011E4
		int IDebugDocumentContext2.EnumCodeContexts(out IEnumDebugCodeContexts2 ppEnumCodeCxts)
		{
			ppEnumCodeCxts = null;
			int result;
			try
			{
				ppEnumCodeCxts = new AD7CodeContextEnum(new AD7MemoryAddress[]
				{
					this.m_codeContext
				});
				result = 0;
			}
			catch (Exception e)
			{
				result = Utils.UnexpectedException(e);
			}
			return result;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000302C File Offset: 0x0000122C
		int IDebugDocumentContext2.GetDocument(out IDebugDocument2 ppDocument)
		{
			ppDocument = null;
			return -2147467259;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003036 File Offset: 0x00001236
		int IDebugDocumentContext2.GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
		{
			pbstrLanguage = "C++";
			pguidLanguage = Guids.guidLanguageCpp;
			return 0;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000304B File Offset: 0x0000124B
		int IDebugDocumentContext2.GetName(enum_GETNAME_TYPE gnType, out string pbstrFileName)
		{
			pbstrFileName = this.m_fileName;
			return 0;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003056 File Offset: 0x00001256
		int IDebugDocumentContext2.GetSourceRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
		{
			throw new NotImplementedException("This method is not implemented");
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003064 File Offset: 0x00001264
		int IDebugDocumentContext2.GetStatementRange(TEXT_POSITION[] pBegPosition, TEXT_POSITION[] pEndPosition)
		{
			try
			{
				pBegPosition[0].dwColumn = this.m_begPos.dwColumn;
				pBegPosition[0].dwLine = this.m_begPos.dwLine;
				pEndPosition[0].dwColumn = this.m_endPos.dwColumn;
				pEndPosition[0].dwLine = this.m_endPos.dwLine;
			}
			catch (Exception e)
			{
				return Utils.UnexpectedException(e);
			}
			return 0;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000030EC File Offset: 0x000012EC
		int IDebugDocumentContext2.Seek(int nCount, out IDebugDocumentContext2 ppDocContext)
		{
			ppDocContext = null;
			return -2147467263;
		}

		// Token: 0x04000014 RID: 20
		private string m_fileName;

		// Token: 0x04000015 RID: 21
		private TEXT_POSITION m_begPos;

		// Token: 0x04000016 RID: 22
		private TEXT_POSITION m_endPos;

		// Token: 0x04000017 RID: 23
		private AD7MemoryAddress m_codeContext;
	}
}
