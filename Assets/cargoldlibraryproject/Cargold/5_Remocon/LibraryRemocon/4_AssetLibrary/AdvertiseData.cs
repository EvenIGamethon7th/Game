using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;
using System;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class AssetLibraryData
        {
            [FoldoutGroup(AdvertiseData.KorStr), Indent(AdvertiseData.IndentLv)]
            public partial class AdvertiseData // 메인
            {
                public const string KorStr = "광고";
                public const string Str = "Ads";
                public const int IndentLv = AssetLibraryData.IndentLv + 1;

                [LabelText("무반응 강제 종료 시간")] public float unresponseCancleTime = 10f;
                [InlineProperty, HideLabel] public UnityAdsData unityAdsData = new UnityAdsData();

                public void Init_Func()
                {
                    this.unityAdsData.Init_Func();
                }

                [FoldoutGroup(UnityAdsData.KorStr), Indent(UnityAdsData.IndentLv)]
                public class UnityAdsData : ScriptGenerate // 메인
                {
                    public const string KorStr = "유니티 애즈";
                    public const int IndentLv = AdvertiseData.IndentLv + 1;
                    public const string OpenUrlStr = "UGS 수익화 페이지로 이동";
                    public const string DefineSymbolStr = "Ads_Unity_C";

                    [Title("Game IDs")]
                    [InfoBox("@GetInfoBoxStr", "IsInfoBoxShow_Func")]
                    [BoxGroup(Cargold.CargoldLibrary_C.Mandatory), LabelText("Android")] public string aosGameID = null;
                    [BoxGroup(Cargold.CargoldLibrary_C.Mandatory), LabelText("iOS")] public string iosGameID = null;

                    [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), Title("전면 광고")]
                    [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("AOS")] public string aosUnitID_Interstitial = "Interstitial_Android";
                    [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("IOS")] public string iosUnitID_Interstitial = "Interstitial_iOS";

                    [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), Title("보상형 광고")]
                    [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("AOS")] public string aosUnitID_Reward = "Rewarded_Android";
                    [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("IOS")] public string iosUnitID_Reward = "Rewarded_iOS";

                    protected override string GetClassNameDefault => "AdsSystem_Manager";
#if Ads_Unity_C
                    protected override Type GetExampleType => typeof(Cargold.Example.광고_유니티);
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
#if Ads_Unity_C
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


                    private string GetInfoBoxStr
                    {
                        get
                        {
                            return "Game IDs는 아래에서 확인 가능합니다.\n" +
                            "1. '" + OpenUrlStr + "' 버튼 클릭 \n" +
                            "2. 적용하고자 하는 프로젝트 선택\n" +
                            "3. Monetization -> Current Project -> Settings 선택\n" +
                            "4. 스크롤 중간 쯤에 Game IDs 가져오셈";
                        }
                    }

                    public override void Init_Func()
                    {
                        base.Init_Func();

                        base.subFolderPathArr = new string[1] { AdvertiseData.Str };
                    }

                    private bool IsInfoBoxShow_Func()
                    {
                        return this.aosGameID.IsNullOrWhiteSpace_Func() || this.iosGameID.IsNullOrWhiteSpace_Func();
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

                    [BoxGroup(Cargold.CargoldLibrary_C.Mandatory), Button(OpenUrlStr)]
                    private void OnMonetization_Func()
                    {
                        Application.OpenURL("https://dashboard.unity3d.com/monetization");
                    }
                }
            }
        }
    }
}