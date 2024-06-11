using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class AssetLibraryData
        {
            [FoldoutGroup(TrackingLogData.KorStr), Indent(TrackingLogData.IndentLv)]
            public partial class TrackingLogData // 메인
            {
                public const string KorStr = "트래킹 로그";
                public const string Str = "TrackingLog";
                public const int IndentLv = AssetLibraryData.IndentLv + 1;
                public const string DefineSymbolStr = "Log_Unity_C";

                [InlineProperty, HideLabel] public UnityLogData unityLogData = new UnityLogData();

                public void Init_Func()
                {
                    this.unityLogData.Init_Func();
                }

                [FoldoutGroup(UnityLogData.KorStr), Indent(UnityLogData.IndentLv)]
                public class UnityLogData : ScriptGenerate
                {
                    public const string KorStr = "유니티 트래킹로그";
                    public const int IndentLv = TrackingLogData.IndentLv + 1;

                    protected override string GetClassNameDefault => "LogSystem_Manager";

#if Log_Unity_C
                    protected override Type GetExampleType => typeof(Cargold.Example.트래킹로그_유니티);
#else
                    protected override Type GetExampleType
                    {
                        get
                        {
#if UNITY_EDITOR
                            Remocon.ProjectRemocon.Instance.buildSystem.CallEdit_AddDefine_Func(DefineSymbolStr); 
#endif

                            LibraryRemocon.instance.Subscribe_OnCompileDone_Func(this.CallDel_OnCompileDone_Func);

                            throw new Exception($"디파인 심볼에 {DefineSymbolStr}을 추가하고 있습니다...");
                        }
                    }
#endif

                    private void CallDel_OnCompileDone_Func()
                    {
                        LibraryRemocon.instance.Unsubscribe_OnCompileDone_Func(this.CallDel_OnCompileDone_Func);

                        bool _isWaitDone = false;
#if Log_Unity_C
                        _isWaitDone = true;
#endif

#if UNITY_EDITOR
                        if (_isWaitDone == true)
                        {
                            base.CallEdit_Generate_Func();
                        }
                        else
                        {
                            Debug_C.Error_Func("스크립트 생성 실패");
                        }
#endif
                    }


                    public override void Init_Func()
                    {
                        base.Init_Func();

                        base.subFolderPathArr = new string[1] { TrackingLogData.Str };
                    }

#if UNITY_EDITOR
                    protected override void CallEdit_Generate_Func(
                                    string _exampleScriptFolderPath, Func<string, string> _codeModifyDel
                                    , string _className = null, string _scriptName = null, params string[] _subFolderStrArr)
                    {
                        base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _className, _scriptName, _subFolderStrArr);

                        Remocon.ProjectRemocon.Instance.buildSystem.CallEdit_AddDefine_Func(DefineSymbolStr);
                    }
#endif
                }
            }
        }
    }
}