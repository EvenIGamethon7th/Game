using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System.IO;
using Cargold.Example;
using System;

namespace Cargold
{
    public partial class LibraryRemocon
    {
        [FoldoutGroup(FrameWorkData.KorStr), Indent(LibraryRemocon.IndentLv)]
        public partial class FrameWorkData // 메인
        {
            public const string KorStr = "프레임워크";
            public const int IndentLv = LibraryRemocon.IndentLv + 1;
            public const string GameManager = "게임 매니저";
            public const string LczManager = "로컬라이즈 매니저";
            public const string SaveManager = "세이브 매니저";
            public const string PoolManager = "풀링 매니저";

            public static FrameWorkData Instance => LibraryRemocon.Instance.frameWorkData;

            [SerializeField, LabelText("클래스 이름")] private string gameManagerName = "GameSystem_Manager";
            [SerializeField, LabelText("경로"), FolderPath] private string gameManagerPath = CargoldLibrary_C.GetInitError;
            [SerializeField, LabelText("프리팹 캐싱")] private FrameWork.GameSystem_Manager gameSystem_Manager = null;
            [SerializeField, LabelText("생성된 프리팹, Scene 배치 여부")] private bool isPlaceInScene = true;

            [InlineProperty, HideLabel] public UserSystemData userManagerData = new UserSystemData();
            [InlineProperty, HideLabel] public LocalizeSystemData lczData = new LocalizeSystemData();
            [InlineProperty, HideLabel] public SaveSystemData saveData = new SaveSystemData();
            [InlineProperty, HideLabel] public PoolingSystemData poolData = new PoolingSystemData();
            [InlineProperty, HideLabel] public SoundSystemData soundSystemData = new SoundSystemData();
            [InlineProperty, HideLabel] public DatabaseData databaseData = new DatabaseData(); 

            public void Init_Func()
            {
                this.gameManagerPath = CargoldLibrary_C.GetFolderApath_Resources_C;

                this.lczData.Init_Func();
                this.databaseData.Init_Func();
                this.soundSystemData.Init_Func();
            }

#if UNITY_EDITOR
            [Button("프레임워크 자동 구축", ButtonSizes.Medium, ButtonStyle.Box), GUIColor(.9f, 1f, .9f), PropertyOrder(0)]
            private void CallEdit_Generate_Func(bool 확인 = false, string 주의 = "사용 여부가 체크된 기능만 생성됩니다.")
            {
                if (확인 == false)
                {
                    Debug_C.Log_Func("확인해주세요!");
                    return;
                }

                LibraryRemocon.Instance.UnsubscribeAll_OnCompileDone_Func();

                string _exampleScriptFolderPath = LibraryRemocon.Instance.GetExampleScriptFolderPath;

                LibraryRemocon.CallEdit_Generate_Script_Func(_exampleScriptFolderPath, typeof(게임매니저), this.gameManagerName, _generateSubFolderPathStrArr: "FrameWork");

                this.userManagerData.CallEdit_Generate_Func(_exampleScriptFolderPath);
                this.lczData.CallEdit_Generate_Func(_exampleScriptFolderPath);
                this.saveData.CallEdit_Generate_Func(_exampleScriptFolderPath);
                this.poolData.CallEdit_Generate_Func(_exampleScriptFolderPath);
                this.soundSystemData.CallEdit_Generate_Func(_exampleScriptFolderPath);

                Type _projectRemoconType = typeof(Cargold.Remocon.ProjectRemocon);
                Editor_C.TryGetLoadWithGenerateSobj_Func<Cargold.Remocon.ProjectRemocon>(_projectRemoconType.Name, out _, _projectRemoconType, instance.GetResourcesFolderPath);

                LibraryRemocon.instance.Subscribe_OnCompileDone_Func(this.CallEdit_OnCompileDone_Func);
            }
            private void CallEdit_OnCompileDone_Func()
            {
                LibraryRemocon.instance.Unsubscribe_OnCompileDone_Func(this.CallEdit_OnCompileDone_Func);

                // 프레임워크 구축 끝난 후 컴포넌트까지 자동 세팅되게 하려고 하는데... 하다가 막혀서 포기!
                //Type _gameManagerType = Type.GetType(this.gameManagerName);
                //Editor_C.TryGetLoadWithGeneratePrefab_Func(
                //    this.gameManagerName, out FrameWork.GameSystem_Manager _gsm, this.isPlaceInScene, _gameManagerType, this.gameManagerPath);

                //this.gameSystem_Manager = _gsm;
            }
#endif

            #region UserSystemData
            [FoldoutGroup(UserSystemData.KorStr), Indent(FrameWorkData.IndentLv)]
            public class UserSystemData : ScriptGenerate
            {
                public const string KorStr = "유저 시스템";
                public const string WealthEnum = "재화 Enum";
                public const string WealthEnumG = WealthEnum + "/1";
                public const string WealthEnumGS = WealthEnumG + "/";

                [SerializeField, LabelText("유저 데이터 이름")] private string userDataName = "UserData";
                [SerializeField, LabelText("유저 아이템 데이터 이름")] private string userWealthDataName = "UserWealthData";
                [SerializeField, LabelText("재화 밸류 타입")] private string wealthQuantityTypeName = "int";
                [BoxGroup(WealthEnum), HorizontalGroup(WealthEnumG)]
                [BoxGroup(WealthEnumGS + "이름"), SerializeField, HideLabel] private string wealthTypeName = "WealthType";
                [BoxGroup(WealthEnumGS + "요소"), SerializeField, HideLabel] private string[] wealthTypeArr = new string[0];
                [SerializeField, LabelText("테스트 유저 데이터 이름")] private string testUserDataName = "Test_UserData";

                protected override bool GetIsEnableDefault => true;
                protected override string GetClassNameDefault => "UserSystem_Manager";
                protected override System.Type GetExampleType => typeof(Cargold.Example.유저매니저);

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, System.Func<string, string> _codeModifyDel, string _className, string _scriptName, params string[] _subFolderStrArr)
                {
                    string _userDataTypeStr = typeof(Cargold.Example.유저데이터).Name;
                    string _wealthTypeStr = typeof(Cargold.Example.재화타입).Name;
                    string _userWealthDataTypeStr = typeof(Cargold.Example.유저재화데이터).Name;
                    string _userWealthQuantityTypeStr = typeof(Cargold.Example.재화밸류).Name;

                    string[] _subFolderPathStrArr = new[] { "FrameWork", "UserSystem" };
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, Get_Func, _subFolderStrArr: _subFolderPathStrArr);

                    LibraryRemocon.CallEdit_Generate_Script_Func(_exampleScriptFolderPath, typeof(Cargold.Example.유저데이터), this.userDataName, Get_Func
                        , _generateSubFolderPathStrArr: _subFolderPathStrArr);

                    LibraryRemocon.CallEdit_Generate_Script_Func(_exampleScriptFolderPath, typeof(Cargold.Example.재화타입), this.wealthTypeName, (string _scriptStr) =>
                    {
                        string _enumWealthStr = default;
                        foreach (string wealthType in this.wealthTypeArr)
                        {
                            if (wealthType.IsNullOrWhiteSpace_Func() == true)
                                continue;

                            _enumWealthStr = StringBuilder_C.Append_Func(_enumWealthStr, "    ", wealthType, ",\n");
                        }

                        return string.Format(DB.TableImporter.TableImporter_Generate.GetScriptGen_Enum, this.wealthTypeName, _enumWealthStr);
                    }, true, _generateSubFolderPathStrArr: _subFolderPathStrArr);

                    LibraryRemocon.CallEdit_Generate_Script_Func(_exampleScriptFolderPath, typeof(Cargold.Example.테스트유저데이터), this.testUserDataName
                        , (_scriptStr) =>
                        {
                            _scriptStr = _scriptStr.Replace(typeof(Cargold.Example.유저데이터).Name, this.userDataName);

                            return _scriptStr;
                        }, _generateSubFolderPathStrArr: _subFolderPathStrArr);

                    string Get_Func(string _scriptStr)
                    {
                        _scriptStr = _scriptStr.Replace(_userDataTypeStr, this.userDataName);
                        _scriptStr = _scriptStr.Replace(_wealthTypeStr, this.wealthTypeName);
                        _scriptStr = _scriptStr.Replace(_userWealthDataTypeStr, this.userWealthDataName);
                        _scriptStr = _scriptStr.Replace(_userWealthQuantityTypeStr, this.wealthQuantityTypeName);

                        return _scriptStr;
                    }
                }
#endif
            }
            #endregion
            #region SaveSystemData
            [FoldoutGroup(SaveSystemData.KorStr), Indent(FrameWorkData.IndentLv)]
            public class SaveSystemData : ScriptGenerate
            {
                public const string KorStr = "세이브";
                 
                protected override bool GetIsEnableDefault => true;
                protected override string GetClassNameDefault => "SaveSystem_Manager";
                protected override System.Type GetExampleType => typeof(Cargold.Example.세이브매니저);

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, System.Func<string, string> _codeModifyDel, string _className, string _scriptName, params string[] _subFolderStrArr)
                {
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _subFolderStrArr: "FrameWork");
                } 
#endif
            }
            #endregion
            #region PoolingSystemData
            [FoldoutGroup(PoolingSystemData.KorStr), Indent(FrameWorkData.IndentLv)]
            public class PoolingSystemData : ScriptGenerate
            {
                public const string KorStr = "풀링";

                protected override bool GetIsEnableDefault => true;
                protected override string GetClassNameDefault => "PoolingSystem_Manager";
                protected override System.Type GetExampleType => typeof(Cargold.Example.풀링매니저);

#if UNITY_EDITOR
                protected override void CallEdit_Generate_Func(string _exampleScriptFolderPath, System.Func<string, string> _codeModifyDel, string _className, string _scriptName, params string[] _subFolderStrArr)
                {
                    base.CallEdit_Generate_Func(_exampleScriptFolderPath, _codeModifyDel, _subFolderStrArr: "FrameWork");
                } 
#endif
            }
            #endregion
        }
    } 
}