using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.Debugger.Interop;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000036 RID: 54
	public class StackFrame : IDebugStackFrame2
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000DD RID: 221 RVA: 0x000045F8 File Offset: 0x000027F8
		// (set) Token: 0x060000DE RID: 222 RVA: 0x00004600 File Offset: 0x00002800
		public Thread Thread { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00004609 File Offset: 0x00002809
		internal SoftDebuggerSession Session
		{
			get
			{
				return this.Thread.Process.Session;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x0000461B File Offset: 0x0000281B
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00004623 File Offset: 0x00002823
		public Client.StackFrame Frame { get; private set; }

		// Token: 0x060000E2 RID: 226 RVA: 0x0000462C File Offset: 0x0000282C
		internal StackFrame(Thread thread, Client.StackFrame frame, IEventSender eventSender)
		{
			this.Thread = thread;
			this.Frame = frame;
			this.eventSender = eventSender;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00004649 File Offset: 0x00002849
		public void SendEvent(EngineEvent e)
		{
			this.eventSender.SendEvent(e);
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00004658 File Offset: 0x00002858
		public static EvaluationOptions DefaultOptions
		{
			get
			{
				EvaluationOptions defaultOptions = EvaluationOptions.DefaultOptions;
				defaultOptions.GroupPrivateMembers = true;
				defaultOptions.GroupStaticMembers = true;
				defaultOptions.FlattenHierarchy = false;
				return defaultOptions;
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00004684 File Offset: 0x00002884
		private static bool WaitAll(WaitHandle[] handles, uint timeout)
		{
			DateTime now = DateTime.Now;
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeout);
			for (int i = 0; i < handles.Length; i++)
			{
				if (!handles[i].WaitOne(timeSpan))
				{
					return false;
				}
				TimeSpan timeSpan2 = DateTime.Now - now;
				if (timeSpan2 >= timeSpan)
				{
					return false;
				}
				timeSpan -= timeSpan2;
			}
			return true;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000046DC File Offset: 0x000028DC
		public int EnumProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, uint nRadix, ref Guid guidFilter, uint dwTimeout, out uint pcelt, out IEnumDebugPropertyInfo2 ppEnum)
		{
			EvaluationOptions evaluationOptions = this.Session.EvaluationOptions;
			evaluationOptions.EvaluationTimeout = (int)dwTimeout;
			evaluationOptions.MemberEvaluationTimeout = (int)dwTimeout;
			EvaluationOptions evaluationOptions2 = this.Session.EvaluationOptions;
			if (evaluationOptions2 != null)
			{
				evaluationOptions.EvaluationTimeout = evaluationOptions2.EvaluationTimeout;
				evaluationOptions.MemberEvaluationTimeout = evaluationOptions2.MemberEvaluationTimeout;
			}
			if (nRadix == 16U)
			{
				evaluationOptions.IntegerDisplayFormat = IntegerDisplayFormat.Hexadecimal;
			}
			else
			{
				evaluationOptions.IntegerDisplayFormat = IntegerDisplayFormat.Decimal;
			}
			List<ObjectValue> list = new List<ObjectValue>();
			var exception = this.Frame.GetException(evaluationOptions);
			if (exception != null)
			{
				list.Add(exception.Instance);
			}
			ObjectValue thisReference = this.Frame.GetThisReference(evaluationOptions);
			if (thisReference != null)
			{
				list.Add(thisReference);
			}
			if (guidFilter == Guids.guidFilterLocals)
			{
				list.AddRange(this.Frame.GetLocalVariables(evaluationOptions));
			}
			else if (guidFilter == Guids.guidFilterArgs)
			{
				list.AddRange(this.Frame.GetParameters(evaluationOptions));
			}
			else if (guidFilter == Guids.guidFilterLocalsPlusArgs)
			{
				list.AddRange(this.Frame.GetParameters(evaluationOptions));
				list.AddRange(this.Frame.GetLocalVariables(evaluationOptions));
			}
			if (list.Count == 0)
			{
				ppEnum = new AD7PropertyEnum(new DEBUG_PROPERTY_INFO[0]);
				pcelt = 0U;
				return 1;
			}
			StackFrame.WaitAll((from x in list
			select x.WaitHandle).ToArray<WaitHandle>(), (uint)evaluationOptions.EvaluationTimeout);
			DEBUG_PROPERTY_INFO[] array = new DEBUG_PROPERTY_INFO[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				this.CreateProperty(ref array[i], null, list[i]);
			}
			ppEnum = new AD7PropertyInfoEnum(array);
			pcelt = (uint)array.Length;
			return 0;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000048A0 File Offset: 0x00002AA0
		protected void CreateProperty(ref DEBUG_PROPERTY_INFO info, string path, ObjectValue value)
		{
			if (path != null)
			{
				info.bstrFullName = path + value.ChildSelector;
			}
			else
			{
				info.bstrFullName = (value.Name ?? string.Empty);
			}
			info.bstrName = (value.Name ?? string.Empty);
			info.bstrType = (value.TypeName ?? string.Empty);
			info.bstrValue = (value.Value ?? string.Empty);
			info.dwAttrib = 0L;
			if ((value.Flags & ObjectValueFlags.Property) != ObjectValueFlags.None)
			{
				info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_PROPERTY;
			}
			if ((value.Flags & ObjectValueFlags.Protected) != ObjectValueFlags.None)
			{
				info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PROTECTED;
			}
			if ((value.Flags & ObjectValueFlags.Public) != ObjectValueFlags.None)
			{
				info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PUBLIC;
			}
			if ((value.Flags & ObjectValueFlags.Private) != ObjectValueFlags.None)
			{
				info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_PRIVATE;
			}
			if ((value.Flags & ObjectValueFlags.Global) != ObjectValueFlags.None)
			{
				info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_STORAGE_STATIC;
			}
			if (value.IsEvaluating)
			{
				info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_TIMEOUT | enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_ERROR;
				if (string.IsNullOrEmpty(info.bstrValue))
				{
					info.bstrValue = "Function evaluation timeout.";
				}
			}
			info.dwFields = enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_STANDARD | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME;
			info.pProperty = new StackFrame.Property(this, info.bstrFullName, value);
			if (value.TypeName == "string")
			{
				info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_RAW_STRING;
			}
			if (value.HasChildren)
			{
				info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_OBJ_IS_EXPANDABLE;
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00004A3A File Offset: 0x00002C3A
		protected void CreateProperty(ref DEBUG_PROPERTY_INFO info, string error)
		{
			info.dwFields = enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE;
			info.dwAttrib = enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_ERROR;
			info.bstrValue = error;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00004A54 File Offset: 0x00002C54
		public int GetDocumentContext(out IDebugDocumentContext2 context)
		{
			context = null;
			if (this.Frame.IsExternalCode || !this.Frame.HasDebugInfo)
			{
				return 1;
			}
			if (this.Frame.SourceLocation == null)
			{
				return 1;
			}
			TEXT_POSITION begPos;
			TEXT_POSITION endPos;
			if (this.Frame.SourceLocation.EndLine == -1 || this.Frame.SourceLocation.EndColumn == -1)
			{
				begPos.dwLine = (uint)(this.Frame.SourceLocation.Line - 1);
				begPos.dwColumn = 0U;
				endPos.dwLine = (uint)this.Frame.SourceLocation.Line;
				endPos.dwColumn = 0U;
			}
			else
			{
				begPos.dwLine = (uint)(this.Frame.SourceLocation.Line - 1);
				begPos.dwColumn = (uint)(this.Frame.SourceLocation.Column - 1);
				endPos.dwLine = (uint)(this.Frame.SourceLocation.EndLine - 1);
				endPos.dwColumn = (uint)(this.Frame.SourceLocation.EndColumn - 1);
			}
			string fileName = this.Frame.SourceLocation.FileName;
			context = new AD7DocumentContext(fileName, begPos, endPos, this.GetCodeContext());
			return 0;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00004B7D File Offset: 0x00002D7D
		public int GetExpressionContext(out IDebugExpressionContext2 ppExprCxt)
		{
			ppExprCxt = new StackFrame.ExpressionContext(this);
			return 0;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00004B88 File Offset: 0x00002D88
		public int GetPhysicalStackRange(out ulong paddrMin, out ulong paddrMax)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00004B90 File Offset: 0x00002D90
		public void SetFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, out FRAMEINFO frameInfo)
		{
			frameInfo = default(FRAMEINFO);
			if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME) != 0)
			{
				frameInfo.m_bstrFuncName = this.Frame.ToString();
				frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FUNCNAME;
			}
			if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_MODULE) != 0)
			{
				try
				{
					frameInfo.m_bstrModule = Path.GetFileName(this.Frame.FullModuleName);
				}
				catch
				{
					frameInfo.m_bstrModule = "<Unknown>";
				}
				frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_MODULE;
			}
			if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_LANGUAGE) != 0)
			{
				frameInfo.m_bstrLanguage = "C#";
				frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_LANGUAGE;
			}
			if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FRAME) != 0)
			{
				frameInfo.m_pFrame = this;
				frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FRAME;
			}
			if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO) != 0)
			{
				frameInfo.m_fHasDebugInfo = (this.Frame.HasDebugInfo ? 1 : 0);
				frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO;
			}
			if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_STALECODE) != 0)
			{
				frameInfo.m_fStaleCode = 0;
				frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_STALECODE;
			}
			frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FLAGS;
			frameInfo.m_dwFlags = 0U;
			if (this.Frame.IsExternalCode || !this.Frame.HasDebugInfo)
			{
				frameInfo.m_dwFlags |= 2U;
				frameInfo.m_dwFlags |= 1U;
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00004CD8 File Offset: 0x00002ED8
		public int GetDebugProperty(out IDebugProperty2 ppProperty)
		{
			ppProperty = null;
			return -2147467263;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00004CE2 File Offset: 0x00002EE2
		private AD7MemoryAddress GetCodeContext()
		{
			return new AD7MemoryAddress(this.Thread.Process.Engine, 0UL);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00004CFB File Offset: 0x00002EFB
		public int GetCodeContext(out IDebugCodeContext2 ppCodeCxt)
		{
			ppCodeCxt = this.GetCodeContext();
			return 0;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00004D06 File Offset: 0x00002F06
		public int GetInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, FRAMEINFO[] pFrameInfo)
		{
			this.SetFrameInfo(dwFieldSpec, out pFrameInfo[0]);
			return 0;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00004D17 File Offset: 0x00002F17
		public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
		{
			pbstrLanguage = "C#";
			pguidLanguage = Guids.guidLanguageCpp;
			return 0;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00004D2C File Offset: 0x00002F2C
		public int GetName(out string pbstrName)
		{
			pbstrName = this.Frame.ToString();
			return 0;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00004D3C File Offset: 0x00002F3C
		public int GetThread(out IDebugThread2 ppThread)
		{
			ppThread = this.Thread;
			return 0;
		}

		// Token: 0x04000052 RID: 82
		private const int E_EVALUATE_TIMEOUT = -2147221455;

		// Token: 0x04000053 RID: 83
		public const ulong DBG_ATTRIB_OBJ_IS_EXPANDABLE = 1UL;

		// Token: 0x04000054 RID: 84
		public const ulong DBG_ATTRIB_OBJ_HAS_ID = 2UL;

		// Token: 0x04000055 RID: 85
		public const ulong DBG_ATTRIB_OBJ_CAN_HAVE_ID = 4UL;

		// Token: 0x04000056 RID: 86
		public const ulong DBG_ATTRIB_VALUE_READONLY = 16UL;

		// Token: 0x04000057 RID: 87
		public const ulong DBG_ATTRIB_VALUE_ERROR = 32UL;

		// Token: 0x04000058 RID: 88
		public const ulong DBG_ATTRIB_VALUE_SIDE_EFFECT = 64UL;

		// Token: 0x04000059 RID: 89
		public const ulong DBG_ATTRIB_OVERLOADED_CONTAINER = 128UL;

		// Token: 0x0400005A RID: 90
		public const ulong DBG_ATTRIB_VALUE_BOOLEAN = 256UL;

		// Token: 0x0400005B RID: 91
		public const ulong DBG_ATTRIB_VALUE_BOOLEAN_TRUE = 512UL;

		// Token: 0x0400005C RID: 92
		public const ulong DBG_ATTRIB_VALUE_INVALID = 1024UL;

		// Token: 0x0400005D RID: 93
		public const ulong DBG_ATTRIB_VALUE_NAT = 2048UL;

		// Token: 0x0400005E RID: 94
		public const ulong DBG_ATTRIB_VALUE_AUTOEXPANDED = 4096UL;

		// Token: 0x0400005F RID: 95
		public const ulong DBG_ATTRIB_VALUE_TIMEOUT = 8192UL;

		// Token: 0x04000060 RID: 96
		public const ulong DBG_ATTRIB_VALUE_RAW_STRING = 16384UL;

		// Token: 0x04000061 RID: 97
		public const ulong DBG_ATTRIB_VALUE_CUSTOM_VIEWER = 32768UL;

		// Token: 0x04000062 RID: 98
		public const ulong DBG_ATTRIB_ACCESS_NONE = 65536UL;

		// Token: 0x04000063 RID: 99
		public const ulong DBG_ATTRIB_ACCESS_PUBLIC = 131072UL;

		// Token: 0x04000064 RID: 100
		public const ulong DBG_ATTRIB_ACCESS_PRIVATE = 262144UL;

		// Token: 0x04000065 RID: 101
		public const ulong DBG_ATTRIB_ACCESS_PROTECTED = 524288UL;

		// Token: 0x04000066 RID: 102
		public const ulong DBG_ATTRIB_ACCESS_FINAL = 1048576UL;

		// Token: 0x04000067 RID: 103
		public const ulong DBG_ATTRIB_ACCESS_ALL = 2031616UL;

		// Token: 0x04000068 RID: 104
		public const ulong DBG_ATTRIB_STORAGE_NONE = 16777216UL;

		// Token: 0x04000069 RID: 105
		public const ulong DBG_ATTRIB_STORAGE_GLOBAL = 33554432UL;

		// Token: 0x0400006A RID: 106
		public const ulong DBG_ATTRIB_STORAGE_STATIC = 67108864UL;

		// Token: 0x0400006B RID: 107
		public const ulong DBG_ATTRIB_STORAGE_REGISTER = 134217728UL;

		// Token: 0x0400006C RID: 108
		public const ulong DBG_ATTRIB_STORAGE_ALL = 251658240UL;

		// Token: 0x0400006D RID: 109
		public const ulong DBG_ATTRIB_TYPE_NONE = 4294967296UL;

		// Token: 0x0400006E RID: 110
		public const ulong DBG_ATTRIB_TYPE_VIRTUAL = 8589934592UL;

		// Token: 0x0400006F RID: 111
		public const ulong DBG_ATTRIB_TYPE_CONSTANT = 17179869184UL;

		// Token: 0x04000070 RID: 112
		public const ulong DBG_ATTRIB_TYPE_SYNCHRONIZED = 34359738368UL;

		// Token: 0x04000071 RID: 113
		public const ulong DBG_ATTRIB_TYPE_VOLATILE = 68719476736UL;

		// Token: 0x04000072 RID: 114
		public const ulong DBG_ATTRIB_TYPE_ALL = 133143986176UL;

		// Token: 0x04000073 RID: 115
		public const ulong DBG_ATTRIB_DATA = 1099511627776UL;

		// Token: 0x04000074 RID: 116
		public const ulong DBG_ATTRIB_METHOD = 2199023255552UL;

		// Token: 0x04000075 RID: 117
		public const ulong DBG_ATTRIB_PROPERTY = 4398046511104UL;

		// Token: 0x04000076 RID: 118
		public const ulong DBG_ATTRIB_CLASS = 8796093022208UL;

		// Token: 0x04000077 RID: 119
		public const ulong DBG_ATTRIB_BASECLASS = 17592186044416UL;

		// Token: 0x04000078 RID: 120
		public const ulong DBG_ATTRIB_INTERFACE = 35184372088832UL;

		// Token: 0x04000079 RID: 121
		public const ulong DBG_ATTRIB_INNERCLASS = 70368744177664UL;

		// Token: 0x0400007A RID: 122
		public const ulong DBG_ATTRIB_MOSTDERIVED = 140737488355328UL;

		// Token: 0x0400007B RID: 123
		public const ulong DBG_ATTRIB_CHILD_ALL = 280375465082880UL;

		// Token: 0x0400007C RID: 124
		private IEventSender eventSender;

		// Token: 0x0200005A RID: 90
		protected class Property : IDebugProperty3, IDebugProperty2
		{
			// Token: 0x1700003C RID: 60
			// (get) Token: 0x060001D5 RID: 469 RVA: 0x00006A2F File Offset: 0x00004C2F
			// (set) Token: 0x060001D6 RID: 470 RVA: 0x00006A37 File Offset: 0x00004C37
			public StackFrame Frame { get; private set; }

			// Token: 0x1700003D RID: 61
			// (get) Token: 0x060001D7 RID: 471 RVA: 0x00006A40 File Offset: 0x00004C40
			// (set) Token: 0x060001D8 RID: 472 RVA: 0x00006A48 File Offset: 0x00004C48
			public ObjectValue Value { get; private set; }

			// Token: 0x1700003E RID: 62
			// (get) Token: 0x060001D9 RID: 473 RVA: 0x00006A51 File Offset: 0x00004C51
			// (set) Token: 0x060001DA RID: 474 RVA: 0x00006A59 File Offset: 0x00004C59
			public string ObjectPath { get; private set; }

			// Token: 0x1700003F RID: 63
			// (get) Token: 0x060001DB RID: 475 RVA: 0x00006A62 File Offset: 0x00004C62
			// (set) Token: 0x060001DC RID: 476 RVA: 0x00006A6A File Offset: 0x00004C6A
			public string ErrorMessage { get; private set; }

			// Token: 0x060001DD RID: 477 RVA: 0x00006A73 File Offset: 0x00004C73
			public Property(StackFrame frame, string path, ObjectValue value)
			{
				this.Frame = frame;
				this.ObjectPath = path;
				this.Value = value;
			}

			// Token: 0x060001DE RID: 478 RVA: 0x00006A90 File Offset: 0x00004C90
			public Property(StackFrame frame, ObjectValue value)
			{
				this.Frame = frame;
				this.Value = value;
			}

			// Token: 0x060001DF RID: 479 RVA: 0x00006AA6 File Offset: 0x00004CA6
			public Property(StackFrame frame, string error_msg)
			{
				this.Frame = frame;
				this.ErrorMessage = error_msg;
			}

			// Token: 0x060001E0 RID: 480 RVA: 0x00006ABC File Offset: 0x00004CBC
			public int EnumChildren(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, ref Guid guidFilter, enum_DBG_ATTRIB_FLAGS dwAttribFilter, string pszNameFilter, uint dwTimeout, out IEnumDebugPropertyInfo2 ppEnum)
			{
				if (this.Value == null || !this.Value.HasChildren)
				{
					ppEnum = null;
					return 1;
				}
				EvaluationOptions evaluationOptions = this.Frame.Session.EvaluationOptions;
				evaluationOptions.AllowMethodEvaluation = true;
				evaluationOptions.AllowTargetInvoke = true;
				evaluationOptions.AllowToStringCalls = false;
				evaluationOptions.EvaluationTimeout = (int)dwTimeout;
				evaluationOptions.MemberEvaluationTimeout = (int)dwTimeout;
				EvaluationOptions evaluationOptions2 = this.Frame.Session.EvaluationOptions;
				if (evaluationOptions2 != null)
				{
					evaluationOptions.EvaluationTimeout = evaluationOptions2.EvaluationTimeout;
					evaluationOptions.MemberEvaluationTimeout = evaluationOptions2.MemberEvaluationTimeout;
				}
				if (dwRadix == 16U)
				{
					evaluationOptions.IntegerDisplayFormat = IntegerDisplayFormat.Hexadecimal;
				}
				else
				{
					evaluationOptions.IntegerDisplayFormat = IntegerDisplayFormat.Decimal;
				}
				ObjectValue[] allChildren = this.Value.GetAllChildren(evaluationOptions);
				WaitHandle[] handles = (from x in allChildren
				select x.WaitHandle).ToArray<WaitHandle>();
				StackFrame.WaitAll(handles, (uint)evaluationOptions.MemberEvaluationTimeout);
				DEBUG_PROPERTY_INFO[] array = new DEBUG_PROPERTY_INFO[allChildren.Length];
				for (int i = 0; i < allChildren.Length; i++)
				{
					this.Frame.CreateProperty(ref array[i], this.ObjectPath, allChildren[i]);
				}
				ppEnum = new AD7PropertyInfoEnum(array);
				return 0;
			}

			// Token: 0x060001E1 RID: 481 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetDerivedMostProperty(out IDebugProperty2 ppDerivedMost)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001E2 RID: 482 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetExtendedInfo(ref Guid guidExtendedInfo, out object pExtendedInfo)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001E3 RID: 483 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001E4 RID: 484 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetMemoryContext(out IDebugMemoryContext2 ppMemory)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001E5 RID: 485 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetParent(out IDebugProperty2 ppParent)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001E6 RID: 486 RVA: 0x00006BE8 File Offset: 0x00004DE8
			public void UpdatePropertyValue(uint dwTimeout)
			{
				EvaluationOptions evaluationOptions = this.Frame.Session.EvaluationOptions;
				ObjectValue expressionValue = this.Frame.Frame.GetExpressionValue(this.Value.Name, evaluationOptions);
				if (expressionValue == null)
				{
					return;
				}
				if (!expressionValue.WaitHandle.WaitOne((int)dwTimeout))
				{
					return;
				}
				this.Value = expressionValue;
			}

			// Token: 0x060001E7 RID: 487 RVA: 0x00006C40 File Offset: 0x00004E40
			public int GetPropertyInfo(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, uint dwTimeout, IDebugReference2[] rgpArgs, uint dwArgCount, DEBUG_PROPERTY_INFO[] pPropertyInfo)
			{
				EvaluationOptions evaluationOptions = this.Frame.Session.EvaluationOptions;
				if (dwRadix == 16U && evaluationOptions.IntegerDisplayFormat != IntegerDisplayFormat.Hexadecimal)
				{
					evaluationOptions.IntegerDisplayFormat = IntegerDisplayFormat.Hexadecimal;
					this.UpdatePropertyValue(dwTimeout);
				}
				else if (dwRadix == 10U && evaluationOptions.IntegerDisplayFormat != IntegerDisplayFormat.Decimal)
				{
					evaluationOptions.IntegerDisplayFormat = IntegerDisplayFormat.Decimal;
					this.UpdatePropertyValue(dwTimeout);
				}
				if (this.Value != null)
				{
					this.Frame.CreateProperty(ref pPropertyInfo[0], this.ObjectPath, this.Value);
				}
				else
				{
					this.Frame.CreateProperty(ref pPropertyInfo[0], this.ErrorMessage);
				}
				return 0;
			}

			// Token: 0x060001E8 RID: 488 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetReference(out IDebugReference2 ppReference)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001E9 RID: 489 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetSize(out uint pdwSize)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001EA RID: 490 RVA: 0x00004B88 File Offset: 0x00002D88
			public int SetValueAsReference(IDebugReference2[] rgpArgs, uint dwArgCount, IDebugReference2 pValue, uint dwTimeout)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001EB RID: 491 RVA: 0x00006CDC File Offset: 0x00004EDC
			public int SetValueAsString(string pszValue, uint dwRadix, uint dwTimeout)
			{
				if (this.Value == null)
				{
					return -2147467259;
				}
				EvaluationOptions evaluationOptions = this.Frame.Session.EvaluationOptions;
				if (evaluationOptions != null)
				{
					dwTimeout = (uint)evaluationOptions.EvaluationTimeout;
				}
				EvaluationOptions defaultOptions = StackFrame.DefaultOptions;
				defaultOptions.EvaluationTimeout = (int)dwTimeout;
				defaultOptions.MemberEvaluationTimeout = (int)dwTimeout;
				defaultOptions.AllowMethodEvaluation = true;
				defaultOptions.AllowTargetInvoke = true;
				if (dwRadix == 16U)
				{
					defaultOptions.IntegerDisplayFormat = IntegerDisplayFormat.Hexadecimal;
				}
				else
				{
					defaultOptions.IntegerDisplayFormat = IntegerDisplayFormat.Decimal;
				}
				ObjectValue expressionValue = this.Frame.Frame.GetExpressionValue(pszValue, defaultOptions);
				if (!expressionValue.WaitHandle.WaitOne((int)dwTimeout))
				{
					return -2147221455;
				}
				this.Value.SetRawValue(expressionValue.GetRawValue());
				return 0;
			}

			// Token: 0x060001EC RID: 492 RVA: 0x00006D83 File Offset: 0x00004F83
			public int GetStringCharLength(out uint pLen)
			{
				this.InitializeChars();
				pLen = (uint)this._bytes.Length;
				return 0;
			}

			// Token: 0x060001ED RID: 493 RVA: 0x00006D98 File Offset: 0x00004F98
			private void InitializeChars()
			{
				string text = this.Value.GetRawValue() as string;
				this._bytes = text.ToCharArray();
			}

			// Token: 0x060001EE RID: 494 RVA: 0x00006DC4 File Offset: 0x00004FC4
			public int GetStringChars(uint buflen, ushort[] rgString, out uint pceltFetched)
			{
				pceltFetched = 0U;
				if (this._bytes == null)
				{
					return -2147467259;
				}
				pceltFetched = 0U;
				while ((ulong)pceltFetched < (ulong)Math.Min((long)this._bytes.Length, (long)((ulong)buflen)))
				{
					rgString[(int)pceltFetched] = (ushort)this._bytes[(int)pceltFetched];
					pceltFetched += 1U;
				}
				return 0;
			}

			// Token: 0x060001EF RID: 495 RVA: 0x00004B88 File Offset: 0x00002D88
			public int CreateObjectID()
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001F0 RID: 496 RVA: 0x00004B88 File Offset: 0x00002D88
			public int DestroyObjectID()
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001F1 RID: 497 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetCustomViewerCount(out uint pcelt)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001F2 RID: 498 RVA: 0x00004B88 File Offset: 0x00002D88
			public int GetCustomViewerList(uint celtSkip, uint celtRequested, DEBUG_CUSTOM_VIEWER[] rgViewers, out uint pceltFetched)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001F3 RID: 499 RVA: 0x00004B88 File Offset: 0x00002D88
			public int SetValueAsStringWithError(string pszValue, uint dwRadix, uint dwTimeout, out string errorString)
			{
				throw new NotImplementedException();
			}

			// Token: 0x040000E3 RID: 227
			private char[] _bytes;
		}

		// Token: 0x0200005B RID: 91
		private class ExpressionContext : IDebugExpressionContext2
		{
			// Token: 0x17000040 RID: 64
			// (get) Token: 0x060001F4 RID: 500 RVA: 0x00006E10 File Offset: 0x00005010
			// (set) Token: 0x060001F5 RID: 501 RVA: 0x00006E18 File Offset: 0x00005018
			public StackFrame Frame { get; private set; }

			// Token: 0x060001F6 RID: 502 RVA: 0x00006E21 File Offset: 0x00005021
			public ExpressionContext(StackFrame frame)
			{
				this.Frame = frame;
			}

			// Token: 0x060001F7 RID: 503 RVA: 0x00004B88 File Offset: 0x00002D88
			int IDebugExpressionContext2.GetName(out string pbstrName)
			{
				throw new NotImplementedException();
			}

			// Token: 0x060001F8 RID: 504 RVA: 0x00006E30 File Offset: 0x00005030
			int IDebugExpressionContext2.ParseText(string pszCode, enum_PARSEFLAGS dwFlags, uint nRadix, out IDebugExpression2 ppExpr, out string pbstrError, out uint pichError)
			{
				EvaluationOptions defaultOptions = StackFrame.DefaultOptions;
				defaultOptions.AllowMethodEvaluation = false;
				defaultOptions.AllowTargetInvoke = false;
				defaultOptions.AllowToStringCalls = false;
				defaultOptions.EvaluationTimeout = 1000;
				defaultOptions.MemberEvaluationTimeout = 1000;
				if (nRadix == 16U)
				{
					defaultOptions.IntegerDisplayFormat = IntegerDisplayFormat.Hexadecimal;
				}
				else
				{
					defaultOptions.IntegerDisplayFormat = IntegerDisplayFormat.Decimal;
				}
				ValidationResult validationResult = this.Frame.Frame.ValidateExpression(pszCode, defaultOptions);
				if (!validationResult.IsValid)
				{
					pbstrError = validationResult.Message;
					pichError = 0U;
					ppExpr = null;
					return 1;
				}
				pbstrError = null;
				pichError = 0U;
				ppExpr = new StackFrame.Expression(this.Frame, pszCode);
				return 0;
			}
		}

		// Token: 0x0200005C RID: 92
		private class Expression : IDebugExpression2
		{
			// Token: 0x17000041 RID: 65
			// (get) Token: 0x060001F9 RID: 505 RVA: 0x00006ECB File Offset: 0x000050CB
			// (set) Token: 0x060001FA RID: 506 RVA: 0x00006ED3 File Offset: 0x000050D3
			public StackFrame Frame { get; private set; }

			// Token: 0x17000042 RID: 66
			// (get) Token: 0x060001FB RID: 507 RVA: 0x00006EDC File Offset: 0x000050DC
			// (set) Token: 0x060001FC RID: 508 RVA: 0x00006EE4 File Offset: 0x000050E4
			public string Text { get; private set; }

			// Token: 0x060001FD RID: 509 RVA: 0x00006EED File Offset: 0x000050ED
			public Expression(StackFrame frame, string text)
			{
				this.Frame = frame;
				this.Text = text;
			}

			// Token: 0x060001FE RID: 510 RVA: 0x00006F03 File Offset: 0x00005103
			int IDebugExpression2.Abort()
			{
				this.SendResult(new StackFrame.Property(this.Frame, "Evaluation aborted."));
				return 0;
			}

			// Token: 0x060001FF RID: 511 RVA: 0x00006F1C File Offset: 0x0000511C
			int IDebugExpression2.EvaluateAsync(enum_EVALFLAGS dwFlags, IDebugEventCallback2 pExprCallback)
			{
				EvaluationOptions defaultOptions = StackFrame.DefaultOptions;
				defaultOptions.EvaluationTimeout = -1;
				defaultOptions.MemberEvaluationTimeout = -1;
				defaultOptions.EllipsizeStrings = false;
				defaultOptions.IntegerDisplayFormat = this.Frame.Session.EvaluationOptions.IntegerDisplayFormat;
				ObjectValue value = this.Frame.Frame.GetExpressionValue(this.Text, defaultOptions);
				value.ValueChanged += (o, e) => SendResult(new StackFrame.Property(this.Frame, value));
				return 0;
			}

			// Token: 0x06000200 RID: 512 RVA: 0x00006FA1 File Offset: 0x000051A1
			private void SendResult(StackFrame.Property property)
			{
				this.Frame.SendEvent(new ExpressionEvaluationCompleteEvent(this.Frame.Thread, this, property));
			}

			// Token: 0x06000201 RID: 513 RVA: 0x00006FC0 File Offset: 0x000051C0
			int IDebugExpression2.EvaluateSync(enum_EVALFLAGS dwFlags, uint dwTimeout, IDebugEventCallback2 pExprCallback, out IDebugProperty2 ppResult)
			{
				EvaluationOptions defaultOptions = StackFrame.DefaultOptions;
				defaultOptions.EvaluationTimeout = (int)dwTimeout;
				defaultOptions.MemberEvaluationTimeout = (int)dwTimeout;
				defaultOptions.EllipsizeStrings = false;
				defaultOptions.IntegerDisplayFormat = this.Frame.Session.EvaluationOptions.IntegerDisplayFormat;
				if ((dwFlags & enum_EVALFLAGS.EVAL_NOFUNCEVAL) != 0)
				{
					defaultOptions.AllowMethodEvaluation = false;
					defaultOptions.AllowTargetInvoke = false;
				}
				ObjectValue expressionValue = this.Frame.Frame.GetExpressionValue(this.Text, defaultOptions);
				if (expressionValue == null)
				{
					ppResult = null;
					return 1;
				}
				if (!expressionValue.WaitHandle.WaitOne((int)dwTimeout))
				{
					ppResult = null;
					return -2147221455;
				}
				if (expressionValue.IsError)
				{
					ppResult = null;
					return 1;
				}
				expressionValue.Name = this.Text;
				ppResult = new StackFrame.Property(this.Frame, expressionValue);
				return 0;
			}
		}
	}
}
