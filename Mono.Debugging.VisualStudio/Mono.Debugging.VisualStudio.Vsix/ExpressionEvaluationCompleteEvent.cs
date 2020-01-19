using System;
using Microsoft.VisualStudio.Debugger.Interop;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x02000030 RID: 48
	public sealed class ExpressionEvaluationCompleteEvent : ThreadEvent, IDebugExpressionEvaluationCompleteEvent2
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x000039D8 File Offset: 0x00001BD8
		public ExpressionEvaluationCompleteEvent(Thread thread, IDebugExpression2 expr, IDebugProperty2 result) : base(thread, new Guid("C0E13A85-238A-4800-8315-D947C960A843"))
		{
			this.expression = expr;
			this.result = result;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000039F9 File Offset: 0x00001BF9
		public int GetExpression(out IDebugExpression2 ppExpr)
		{
			ppExpr = this.expression;
			return 0;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003A04 File Offset: 0x00001C04
		public int GetResult(out IDebugProperty2 ppResult)
		{
			ppResult = this.result;
			return 0;
		}

		// Token: 0x04000038 RID: 56
		public const string IID = "C0E13A85-238A-4800-8315-D947C960A843";

		// Token: 0x04000039 RID: 57
		private IDebugExpression2 expression;

		// Token: 0x0400003A RID: 58
		private IDebugProperty2 result;
	}
}
