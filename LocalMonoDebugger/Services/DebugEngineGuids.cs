﻿using System;

namespace LocalMonoDebugger.Services
{
    public enum EngineType
    {
        //AD7Engine,
        //MonoEngine,
        XamarinEngine
    }

    public static class DebugEngineGuids
    {
        public readonly static EngineType UseAD7Engine = EngineType.XamarinEngine;

        public const string MonoEngineString = "CEFEF24F-C07D-4CB6-8F6B-33316F26B01D";

        public const string MonoProgramProviderString = "5C3DC3E2-2E0A-40D5-9578-275366C82722";

        public const string EngineName = "VSMonoDebugger";

        public static Guid ProgramProviderGuid
        {
            get
            {
                switch (UseAD7Engine)
                {
                    //case EngineType.AD7Engine:
                    //    return new Guid(AD7ProgramProviderString);
                    //case EngineType.MonoEngine:
                    //    return new Guid(MonoProgramProviderString);
                    case EngineType.XamarinEngine:
                        return new Guid(MonoProgramProviderString);
                    default:
                        throw new NotSupportedException(UseAD7Engine.ToString());

                }
            }
        }
        public static Guid EngineGuid
        {
            get
            {
                switch (UseAD7Engine)
                {
                    //case EngineType.AD7Engine:
                    //    return new Guid(AD7EngineString);
                    //case EngineType.MonoEngine:
                    //    return new Guid(MonoEngineString);
                    case EngineType.XamarinEngine:
                        return new Guid(MonoEngineString);
                    default:
                        throw new NotSupportedException(UseAD7Engine.ToString());
                }
            }
        }


        // Language guid for C++. Used when the language for a document context or a stack frame is requested.
        static private Guid s_guidLanguageCpp = new Guid("3a12d0b7-c26c-11d0-b442-00a0244a1dd2");
        static public Guid guidLanguageCpp
        {
            get { return s_guidLanguageCpp; }
        }

        static private Guid s_guidLanguageCs = new Guid("{3F5162F8-07C6-11D3-9053-00C04FA302A1}");
        static public Guid guidLanguageCs
        {
            get { return s_guidLanguageCs; }
        }

        static private Guid s_guidLanguageC = new Guid("63A08714-FC37-11D2-904C-00C04FA302A1");
        static public Guid guidLanguageC
        {
            get { return s_guidLanguageC; }
        }
    }
}
