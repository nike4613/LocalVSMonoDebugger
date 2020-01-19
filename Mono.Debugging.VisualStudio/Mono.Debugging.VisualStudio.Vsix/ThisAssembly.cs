using System;

// Token: 0x02000002 RID: 2
internal class ThisAssembly
{
	// Token: 0x0200004F RID: 79
	public static class Project
	{
		// Token: 0x040000BC RID: 188
		public const string RootNamespace = "Mono.Debugging.VisualStudio";

		// Token: 0x040000BD RID: 189
		public const string AssemblyName = "Mono.Debugging.VisualStudio.Vsix";

		// Token: 0x040000BE RID: 190
		public const string ProjectGuid = "{50BD9387-CD6B-471C-ABB3-13060CCECC67}";

		// Token: 0x040000BF RID: 191
		public const string TargetFrameworkVersion = "v4.5";

		// Token: 0x040000C0 RID: 192
		public const string TargetFrameworkIdentifier = ".NETFramework";

		// Token: 0x040000C1 RID: 193
		public const string TargetFrameworkMoniker = ".NETFramework,Version=v4.5";

		// Token: 0x040000C2 RID: 194
		public const string TargetPlatformVersion = "7.0";

		// Token: 0x040000C3 RID: 195
		public const string TargetPlatformIdentifier = "Windows";

		// Token: 0x040000C4 RID: 196
		public const string TargetPlatformMoniker = "Windows,Version=7.0";
	}

	// Token: 0x02000050 RID: 80
	public class Git
	{
		// Token: 0x040000C5 RID: 197
		public const bool IsDirty = false;

		// Token: 0x040000C6 RID: 198
		public const string IsDirtyString = "false";

		// Token: 0x040000C7 RID: 199
		public const string Branch = "d15-6";

		// Token: 0x040000C8 RID: 200
		public const string Commit = "f143ac1";

		// Token: 0x040000C9 RID: 201
		public const string Sha = "f143ac132ec7b669ec03974cb39bdbe1d113a796";

		// Token: 0x040000CA RID: 202
		public const string Commits = "10";

		// Token: 0x040000CB RID: 203
		public const string Tag = "";

		// Token: 0x040000CC RID: 204
		public const string BaseTag = "";

		// Token: 0x02000061 RID: 97
		public class BaseVersion
		{
			// Token: 0x040000ED RID: 237
			public const string Major = "4";

			// Token: 0x040000EE RID: 238
			public const string Minor = "9";

			// Token: 0x040000EF RID: 239
			public const string Patch = "0";
		}

		// Token: 0x02000062 RID: 98
		public class SemVer
		{
			// Token: 0x040000F0 RID: 240
			public const string Major = "4";

			// Token: 0x040000F1 RID: 241
			public const string Minor = "9";

			// Token: 0x040000F2 RID: 242
			public const string Patch = "10";

			// Token: 0x040000F3 RID: 243
			public const string Label = "pre";

			// Token: 0x040000F4 RID: 244
			public const string DashLabel = "-pre";

			// Token: 0x040000F5 RID: 245
			public const string Source = "File";
		}
	}

	// Token: 0x02000051 RID: 81
	public static class Vsix
	{
		// Token: 0x040000CD RID: 205
		public const string Identifier = "Mono.Debugging.VisualStudio";

		// Token: 0x040000CE RID: 206
		public const string Name = "Mono Debugging for Visual Studio";

		// Token: 0x040000CF RID: 207
		public const string Description = "Support for debugging Mono processes with Visual Studio.";

		// Token: 0x040000D0 RID: 208
		public const string Author = "Xamarin";
	}
}
