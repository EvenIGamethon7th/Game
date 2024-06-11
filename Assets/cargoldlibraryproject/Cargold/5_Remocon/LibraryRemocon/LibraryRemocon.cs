using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System.IO;
using UnityEngine.Events;

namespace Cargold
{
    public partial class LibraryRemocon : SerializedScriptableObject, Debug_C.IDebug_C // 메인
    {
        public const string Str = "LibraryRemocon";
        public const string KorStr = "카골드 라이브러리";
        public const int IndentLv = 0;
        public const float SpaceGap = 10f;
        public const string ExampleStr = "예제";

        #region Instance
        private static LibraryRemocon instance;
        public static LibraryRemocon Instance
        {
            get
            {
                if (instance == null)
                {
                    StringData_C _stringData = Resources.Load<StringData_C>(CargoldLibrary_C.GetPathDataResourcesPath);

                    if (_stringData is null == false)
                    {
                        string _libraryDataPath = _stringData.GetValue;
                        LibraryRemocon _libraryData = Resources.Load<LibraryRemocon>(_libraryDataPath);

                        if (_libraryData is null == false)
                        {
                            instance = _libraryData;

                            _libraryData.Init_Func();
                        }
                    }
                }

                return instance;
            }
        } 
        #endregion

        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, LabelText("로그")] public Debug_C.PrintLogType logType = Debug_C.PrintLogType.Common;
        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, FolderPath, LabelText("스크립트 폴더 경로")] private string scriptFolderPath = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, FolderPath, LabelText("리소스 폴더 경로")] private string resourcesFolderPath = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), FoldoutGroup(CargoldLibrary_C.MandatoryS + "복제"), SerializeField, FolderPath, LabelText("복제 에셋 폴더 경로")] private string assetExampleFolderPath = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), FoldoutGroup(CargoldLibrary_C.MandatoryS + "복제"), SerializeField, FolderPath, LabelText("복제 애니메이션 폴더 경로")] private string animationExampleFolderPath = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), FoldoutGroup(CargoldLibrary_C.MandatoryS + "복제"), SerializeField, FolderPath, LabelText("복제 스프라이트 폴더 경로")] private string spriteExampleFolderPath = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), FoldoutGroup(CargoldLibrary_C.MandatoryS + "복제"), SerializeField, FolderPath, LabelText("복제 사운드 폴더 경로")] private string soundExampleFolderPath = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), FoldoutGroup(CargoldLibrary_C.MandatoryS + "복제"), SerializeField, FolderPath, LabelText("복제 프리팹 폴더 경로")] private string prefabExampleFolderPath = null;
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, ReadOnly] Cargold.Observer.Observer_Action OnCompiledDoneObs = new Observer.Observer_Action();
        [FoldoutGroup(CargoldLibrary_C.OptionalS + ExampleStr), SerializeField, ReadOnly, LabelText("스크립트 폴더 경로")] private string exampleScriptFolderPath = null;
        [FoldoutGroup(CargoldLibrary_C.OptionalS + ExampleStr), SerializeField, ReadOnly, LabelText("애니메이션 폴더 경로")] private string exampleAnimationFolderPath = null;
        [FoldoutGroup(CargoldLibrary_C.OptionalS + ExampleStr), SerializeField, ReadOnly, LabelText("프리팹 폴더 경로")] private string examplePrefabFolderPath = null;
        [FoldoutGroup(CargoldLibrary_C.OptionalS + ExampleStr), SerializeField, ReadOnly, LabelText("외부 에셋 폴더 경로")] private string externalAssetFolderPath = null;
        [GUIColor(0f, 0f, 0f, 0f), ShowInInspector] private bool poolOdin;
        [InlineProperty, HideLabel] public FrameWorkData frameWorkData = new FrameWorkData();
        [InlineProperty, HideLabel] public UtilityClassData utilityClassData = new UtilityClassData();
        [InlineProperty, HideLabel] public AssetLibraryData assetLibraryData = new AssetLibraryData();
        protected bool isProejctSettingCargoldStyle;
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField] private bool isInit = false;

        #region Property
        public string GetScriptFolderPath => this.scriptFolderPath;
        public string GetResourcesFolderPath => this.resourcesFolderPath;
        public string GetAssetExampleFolderPath => this.assetExampleFolderPath;
        public string GetAnimationFolderPath => this.animationExampleFolderPath;
        public string GetSpriteFolderPath => this.spriteExampleFolderPath;
        public string GetSoundFolderPath => this.soundExampleFolderPath;
        public string GetPrefabFolderPath => this.prefabExampleFolderPath;

        public string GetExampleScriptFolderPath
        {
            get
            {
#if UNITY_EDITOR
                if (this.exampleScriptFolderPath.IsNullOrWhiteSpace_Func() == true)
                    Editor_C.TryGetFindFolderPath_Func("Example_C", out this.exampleScriptFolderPath);
#endif

                return this.exampleScriptFolderPath;
            }
        }
        public string GetExampleAnimationFolderPath
        {
            get
            {
#if UNITY_EDITOR
                if (this.exampleAnimationFolderPath.IsNullOrWhiteSpace_Func() == true)
                    Editor_C.TryGetFindFolderPath_Func("AnimationExample_C", out this.exampleAnimationFolderPath);
#endif

                return this.exampleAnimationFolderPath;
            }
        }
        public string GetExamplePrefabFolderPath
        {
            get
            {
#if UNITY_EDITOR
                if (this.examplePrefabFolderPath.IsNullOrWhiteSpace_Func() == true)
                    Editor_C.TryGetFindFolderPath_Func("PrefabExample_C", out this.examplePrefabFolderPath);
#endif

                return this.examplePrefabFolderPath;
            }
        }
        public string GetExternalAssetFolderPath
        {
            get
            {
#if UNITY_EDITOR
                if (this.externalAssetFolderPath.IsNullOrWhiteSpace_Func() == true)
                    Editor_C.TryGetFindFolderPath_Func("ExternalAsset_C", out this.externalAssetFolderPath);
#endif

                return this.externalAssetFolderPath;
            }
        }
        #endregion

        public void Init_Func
            (string _scriptFolderPath = null, string _resourcesFolderPath = null, string _assetExampleFolderPath = null
            , string _exScriptFolderPath = null, string _exAnimationFolderPath = null, string _exPrefabFolderPath = null
            , string _animationFolderPath = null, string _spriteFolderPath = null, string _soundFolderPath = null, string _prefabFolderPath = null
            , bool _isProejctSettingCargoldStyle = true)
        {
            Debug_C.Log_Func("this.isInit : " + this.isInit + " / _scriptFolderPath : " + _scriptFolderPath);

            if (this.isInit == true)
                return;

            if (this.scriptFolderPath.IsNullOrWhiteSpace_Func() == true && _scriptFolderPath.IsNullOrWhiteSpace_Func() == true)
                return;

            if (this.scriptFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.scriptFolderPath = _scriptFolderPath;

            if (this.resourcesFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.resourcesFolderPath = _resourcesFolderPath;

            if (this.assetExampleFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.assetExampleFolderPath = _assetExampleFolderPath;

            if (this.exampleScriptFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.exampleScriptFolderPath = _exScriptFolderPath;

            if (this.exampleAnimationFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.exampleAnimationFolderPath = _exAnimationFolderPath;

            if (this.examplePrefabFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.examplePrefabFolderPath = _exPrefabFolderPath;

            if (this.animationExampleFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.animationExampleFolderPath = _animationFolderPath;

            if (this.spriteExampleFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.spriteExampleFolderPath = _spriteFolderPath;

            if (this.soundExampleFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.soundExampleFolderPath = _soundFolderPath;

            if (this.prefabExampleFolderPath.IsNullOrWhiteSpace_Func() == true)
                this.prefabExampleFolderPath = _prefabFolderPath;

            this.isProejctSettingCargoldStyle = _isProejctSettingCargoldStyle;

            this.frameWorkData.Init_Func();
            this.utilityClassData.Init_Func();
            this.assetLibraryData.Init_Func();

            this.isInit = true;

#if UNITY_EDITOR
            Editor_C.SetSaveAsset_Func(this); 
#endif
        }

        public void Subscribe_OnCompileDone_Func(System.Action _del)
        {
            if(this.OnCompiledDoneObs.IsSubscribed_Func(_del) == false)
                this.OnCompiledDoneObs.Subscribe_Func(_del);
        }
        public void Unsubscribe_OnCompileDone_Func(System.Action _del)
        {
            this.OnCompiledDoneObs.Unsubscribe_Func(_del);
        }

        [FoldoutGroup(Editor_C.Optional), Button("컴파일 후처리 초기화")]
        public void UnsubscribeAll_OnCompileDone_Func()
        {
            this.OnCompiledDoneObs.UnsubscribeAll_Func();
        }

        public static void SetCatching_Func
            (LibraryRemocon _libraryData
            , string _scriptFolderPath, string _resourcesFolderPath, string _assetExampleFolderPath
            , string _exScriptFolderPath, string _exAnimationFolderPath, string _exPrefabFolderPath
            , string _animationFolderPath, string _spriteFolderPath, string _soundFolderPath, string _prefabFolderPath
            , bool _isProejctSettingCargoldStyle)
        {
            instance = _libraryData;

            _libraryData.Init_Func(_scriptFolderPath, _resourcesFolderPath, _assetExampleFolderPath, _exScriptFolderPath, _exAnimationFolderPath, _exPrefabFolderPath
                , _animationFolderPath, _spriteFolderPath, _soundFolderPath, _prefabFolderPath
                , _isProejctSettingCargoldStyle);
        }

#if UNITY_EDITOR
        [FoldoutGroup(Editor_C.Optional), Button("강제 컴파일")]
        private void CallEdit_OnCompile_Func()
        {
            Editor_C.OnCompile_Func();
        }

        [UnityEditor.Callbacks.DidReloadScripts()]
        private static void CallEdit_OnCompiledDone_Func()
        {
            if (LibraryRemocon.Instance == null || LibraryRemocon.Instance.OnCompiledDoneObs.GetSubscriberNum_Func() <= 0)
                return;

            LibraryRemocon.Instance.OnCompiledDoneObs.Notify_Func();

            LibraryRemocon.Instance.OnCompiledDoneObs.UnsubscribeAll_Func();
        }

        public static Editor_C.GenerateResult CallEdit_Generate_Script_Func(string _exampleScriptFolderPath, System.Type _type, string _className
            , System.Func<string, string> _del = null, bool _isOverWrite = false, string _scriptName = null, string _scriptFolderPath = null
            , params string[] _generateSubFolderPathStrArr)
        {
            string _typeStr = _type.Name;
            return CallEdit_Generate_Script_Func(_exampleScriptFolderPath, _typeStr, _className, _del, _isOverWrite, _scriptName, _scriptFolderPath, _generateSubFolderPathStrArr);
        }
        public static Editor_C.GenerateResult CallEdit_Generate_Script_Func(string _exampleScriptFolderPath, string _typeStr, string _className
            , System.Func<string, string> _del = null, bool _isOverWrite = false, string _scriptName = null, string _scriptFolderPath = null
            , params string[] _generateSubFolderPathStrArr)
        {
            if (_exampleScriptFolderPath.IsNullOrWhiteSpace_Func() == true)
                _exampleScriptFolderPath = LibraryRemocon.Instance.GetExampleScriptFolderPath;

            Editor_C.TryCheckOrGenerateFolder_Func(_exampleScriptFolderPath);

            string _code = Editor_C.GetScriptContent_Func(_exampleScriptFolderPath, _typeStr, false);

            _code = _code.Replace("namespace Cargold.Example {", null);
            _code = _code.Replace(_typeStr, _className);
            _code = _code.Replace("} // End", null);

            if (_del != null)
                _code = _del(_code);

            if(_scriptFolderPath.IsNullOrWhiteSpace_Func() == true)
                _scriptFolderPath = LibraryRemocon.Instance.GetScriptFolderPath;

            if (_generateSubFolderPathStrArr.IsHave_Func() == true)
            {
                _generateSubFolderPathStrArr = _generateSubFolderPathStrArr.InsertFirst_Func(_scriptFolderPath);

                _scriptFolderPath = Editor_C.GetPath_Func(_generateSubFolderPathStrArr);
            }

            return Editor_C.TryGetLoadWithGenerateScript_Func(_scriptFolderPath, _className, _code, _isOverWrite: _isOverWrite, _scriptName: _scriptName);
        }
#endif

        bool Debug_C.IDebug_C.IsLogType_Func(Debug_C.PrintLogType _logType)
        {
            return this.logType.HasFlag(_logType);
        }

        public interface IOverride
        {
            bool IsOverride { get; }
        }

        public abstract class ScriptGenerate
        {
            public const string GenerateStr = "스크립트 생성";

            [LabelText("사용 여부")] public bool isEnable = false;
            [LabelText("클래스 이름")] public string className;

            [FoldoutGroup(Editor_C.Optional), LabelText("스크립트 생성 경로"), SerializeField, FolderPath] protected string scriptFolderPath = null;
            [FoldoutGroup(Editor_C.Optional), LabelText("하위 폴더 경로"), SerializeField] protected string[] subFolderPathArr = new string[0];

            protected virtual bool GetIsEnableDefault => false; // 기본 활성화 여부
            protected abstract string GetClassNameDefault { get; } // 기본 클래스 이름
            protected abstract System.Type GetExampleType { get; } // 예제 스크립트 이름
            protected virtual bool IsOverWrite => false; // 스크립트 덮어쓰기 여부
            protected virtual bool IsActionAfterCompiled => false; // 컴파일 후 행동이 있는 경우

            public ScriptGenerate()
            {
                this.isEnable = this.GetIsEnableDefault;
                this.className = this.GetClassNameDefault;
            }

            [FoldoutGroup(CargoldLibrary_C.Optional), Button("초기화")]
            public virtual void Init_Func() { }

#if UNITY_EDITOR
            [Button(ScriptGenerate.GenerateStr)]
            public void CallEdit_Generate_Func()
            {
                this.CallEdit_Generate_Func(false);
            }
            public void CallEdit_Generate_Func(bool _isForcedGenerate, string _className = null, string _scriptName = null)
            {
                string _exampleScriptFolderPath = LibraryRemocon.Instance.GetExampleScriptFolderPath;
                this.CallEdit_Generate_Func(_exampleScriptFolderPath, _isForcedGenerate, _className, _scriptName);
            }
            public void CallEdit_Generate_Func(string _exampleScriptFolderPath, bool _isForcedGenerate = false, string _className = null, string _scriptName = null)
            {
                if (this.isEnable == false && _isForcedGenerate == false)
                {
                    Debug_C.Log_Func("생성 실패) '사용 여부'가 비활성화 되어있습니다.");
                    return;
                }

                this.CallEdit_Generate_Func(_exampleScriptFolderPath, null, _className, _scriptName);
            }
            protected virtual void CallEdit_Generate_Func(string _exampleScriptFolderPath, System.Func<string, string> _codeModifyDel
                , string _className = null, string _scriptName = null, params string[] _subFolderStrArr)
            {
                if (_className.IsNullOrWhiteSpace_Func() == true)
                    _className = this.className;

                this.CallEdit_Generate_Func
                    (
                        _exampleScriptFolderPath, this.GetExampleType, _className, _codeModifyDel, this.IsOverWrite, _scriptName, this.scriptFolderPath, _subFolderStrArr
                    );
            }
            protected void CallEdit_Generate_Func(System.Type _type, string _className, System.Func<string, string> _del = null)
            {
                string _exampleScriptFolderPath = LibraryRemocon.Instance.GetExampleScriptFolderPath;
                this.CallEdit_Generate_Func(_exampleScriptFolderPath, _type, _className, _del, false, null, this.scriptFolderPath, this.subFolderPathArr);
            }
            private void CallEdit_Generate_Func(string _exampleScriptFolderPath, System.Type _type, string _className
            , System.Func<string, string> _del = null, bool _isOverWrite = false, string _scriptName = null, string _scriptFolderPath = null
            , params string[] _generateSubFolderPathStrArr)
            {
                if (_generateSubFolderPathStrArr.IsHave_Func() == false)
                    _generateSubFolderPathStrArr = this.subFolderPathArr;

                Editor_C.GenerateResult _generateResult = 
                    LibraryRemocon.CallEdit_Generate_Script_Func(
                        _exampleScriptFolderPath, _type, _className, _del, _isOverWrite, _scriptName, _scriptFolderPath, _generateSubFolderPathStrArr);

                if (this.GetIsEnableDefault == false)
                    this.isEnable = false;

                if (this.IsActionAfterCompiled == true && Editor_C.GenerateResult.Success.HasFlag(_generateResult) == true)
                {
                    LibraryRemocon.Instance.OnCompiledDoneObs.Subscribe_Func(this.CallEdit_GenerateDone_Func);
                }
            }

            public void CallEdit_Duplicate_Func(Editor_C.AssetType _assetType, string _assetName, string _categoryStr = null)
            {
                this.CallEdit_Duplicate_Func<UnityEngine.Object>(_assetType, _assetName, _categoryStr);
            }
            public T CallEdit_Duplicate_Func<T>(Editor_C.AssetType _assetType, string _assetName, string _categoryStr = null) where T : UnityEngine.Object
            {
                switch (_assetType)
                {
                    case Editor_C.AssetType.Prefab:
                        {
                            string _generatePath = LibraryRemocon.Instance.GetPrefabFolderPath;
                            string _path = _GetPath_Func(LibraryRemocon.Instance.GetExamplePrefabFolderPath, _generatePath);
                            GameObject _asset = Editor_C.GetLoadAssetAtPath_Func<GameObject>(_path, _isAddExtentionStr: true);
                            return Editor_C.OnGeneratePrefab_Func(_asset, _generatePath, _nameStr: _assetName) as T;
                        }

                    case Editor_C.AssetType.AnimationClip:
                        {
                            string _generatePath = LibraryRemocon.Instance.GetAnimationFolderPath;
                            string _path = _GetPath_Func(LibraryRemocon.Instance.GetExampleAnimationFolderPath, _generatePath);
                            AnimationClip _asset = Editor_C.GetLoadAssetAtPath_Func<AnimationClip>(_path, _isAddExtentionStr: true);
                            return Editor_C.OnGenerateAnimationClip_Func(_asset, _generatePath) as T;
                        }

                    default:
                        Debug_C.Error_Func("_assetType : " + _assetType);
                        return null;
                }

                string _GetPath_Func(string _path, string _generatePath)
                {
                    Editor_C.TryCheckOrGenerateFolder_Func(_generatePath);

                    if (_categoryStr.IsNullOrWhiteSpace_Func() == true)
                        return StringBuilder_C.Append_Func(_path, Editor_C.SeparatorStr, _assetName);
                    else
                        return StringBuilder_C.Append_Func(_path, Editor_C.SeparatorStr, _categoryStr, "_", _assetName);
                }
            }

            protected virtual void CallEdit_GenerateDone_Func()
            {
                if(this.IsActionAfterCompiled == true)
                {
                    LibraryRemocon.Instance.OnCompiledDoneObs.Unsubscribe_Func(this.CallEdit_GenerateDone_Func);
                }
            }
#endif
        }
    }
}