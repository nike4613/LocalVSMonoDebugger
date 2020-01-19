using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Mono.Debugging.VisualStudio
{
	// Token: 0x0200004B RID: 75
	public class Utils
	{
		// Token: 0x060001A6 RID: 422 RVA: 0x000063BC File Offset: 0x000045BC
		static Utils()
		{
			Debug.AutoFlush = true;
			Trace.AutoFlush = true;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x000063CA File Offset: 0x000045CA
		public static void Message(string message, params object[] args)
		{
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x000063CC File Offset: 0x000045CC
		public static void RequireOk(int hr)
		{
			if (hr != 0)
			{
				Exception exceptionForHR = Marshal.GetExceptionForHR(hr, IntPtr.Zero);
			}
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x000063E8 File Offset: 0x000045E8
		public static void CheckOk(int hr)
		{
			if (hr != 0)
			{
				Exception exceptionForHR = Marshal.GetExceptionForHR(hr, IntPtr.Zero);
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00006404 File Offset: 0x00004604
		public static int UnexpectedException(Exception e)
		{
			return -2147417851;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000640B File Offset: 0x0000460B
		public static bool IsSubPathOf(string path, string root)
		{
			return Utils.GetSubPath(path, root) != null;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00006418 File Offset: 0x00004618
		public static string GetSubPath(string path, string root)
		{
			if (string.IsNullOrEmpty(root))
			{
				return path;
			}
			if (path.Length < 2 || root.Length < 2)
			{
				return null;
			}
			path = Path.GetFullPath(path);
			root = Path.GetFullPath(root);
			if (path[1] == Path.VolumeSeparatorChar && root[1] == Path.VolumeSeparatorChar)
			{
				if (char.ToLower(path[0]) != char.ToLower(root[0]))
				{
					return null;
				}
				path = path.Substring(2);
				root = root.Substring(2);
			}
			if (path.Length < root.Length)
			{
				return null;
			}
			if (path.Length == root.Length)
			{
				if (path.Equals(root))
				{
					return new string(Path.DirectorySeparatorChar, 1);
				}
				return null;
			}
			else
			{
				if (!path.Substring(0, root.Length).Equals(root))
				{
					return null;
				}
				if (path[root.Length] != Path.DirectorySeparatorChar)
				{
					return null;
				}
				return path.Substring(root.Length);
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000650C File Offset: 0x0000470C
		public static bool ArePathsEqual(string a, string b)
		{
			if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
			{
				return false;
			}
			if (a.Length < 2 || b.Length < 2)
			{
				return false;
			}
			a = Path.GetFullPath(a);
			b = Path.GetFullPath(b);
			if (a[1] == Path.VolumeSeparatorChar && a[1] == Path.VolumeSeparatorChar)
			{
				if (char.ToLower(a[0]) != char.ToLower(a[0]))
				{
					return false;
				}
				a = a.Substring(2);
				b = b.Substring(2);
			}
			return a.Equals(b);
		}

		// Token: 0x040000B9 RID: 185
		private const int RPC_E_SERVERFAULT = -2147417851;
	}
}
