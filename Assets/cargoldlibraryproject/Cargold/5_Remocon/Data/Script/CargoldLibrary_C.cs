using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Cargold
{
    public class CargoldLibrary_C : ScriptableObject 
    {
        public const string GetFolderApath_Resources = GetAseetsStr + SeparatorStr + GetResourcesStr;
        public const string GetFolderApath_ResourcesC = GetFolderApath_Resources + SeparatorStr + GetLibraryFolderName;
        public const string GetPathDataResourcesPath = GetLibraryFolderName + SeparatorStr + GetPathDataName;
        public const string GetAseetsStr = "Assets";
        public const string GetScriptsStr = "Scripts";
        public const string GetResourcesStr = "Resources";
        public const string GetLibraryFolderName = "Library_C";
        public const string GetPathDataName = "CargoldLibraryDataPath";
        public const string GetFolderName_1Scenes = "1_Scenes";
        public const string GetFolderName_2Scripts = "2_Scripts";
        public const string GetFolderName_3Resources = "3_Resources";
        public const string GetFolderApath_Scripts_C = GetAseetsStr + SeparatorStr + GetFolderName_2Scripts + SeparatorStr + GetLibraryFolderName;
        public const string GetFolderApath_Resources_C = GetAseetsStr + SeparatorStr + GetFolderName_3Resources + SeparatorStr + GetLibraryFolderName;
        public const string GetInitError = "초기화 오류";
        public const string GetLibraryKorStr = "카라리";
        public const string Mandatory = "필수";
        public const string Optional = "선택";
        public const string SeparatorStr = "/";
        public const string CatchingStr = "캐싱 ㄱㄱ ~";
        public const char SeparatorChar = '/';
        public const string CargoldStr = "카골";
        public const string CargoldSStr = CargoldStr + SeparatorStr;
        public const string UnitTestStr = "유닛 테스트";
        public const string Obsolete = "폐기 대상";

        public const string MandatoryS = Mandatory + SeparatorStr;
        public const string OptionalS = Optional + SeparatorStr;
        public const string GetLibraryKorStrS = GetLibraryKorStr + SeparatorStr;
        public const string GetSobjPathS = GetLibraryFolderName + SeparatorStr;

        [ShowInInspector, FolderPath, LabelText("데이터 폴더 경로"), InfoBox("카라리 데이터 스옵젝을 생성할 경로를 지정해주세요."), HideIf("IsInstance_Func")
            , ShowIf("@isProejctSettingCargoldStyle == false")]
        private string libraryDataSobjPath = GetFolderApath_ResourcesC;

        [SerializeField, LabelText("카골드 스타일 프로젝트 세팅 여부"), HideInInspector] private bool isProejctSettingCargoldStyle = true;

        public void Init_Func()
        {

        }

        private bool IsInstance_Func()
        {
            return LibraryRemocon.Instance is null == false;
        }

#if UNITY_EDITOR
        private bool CallEdit_IsNotResourcePath_Func()
        {
            return CallEdit_IsResourcePath_Func() == false && this.isProejctSettingCargoldStyle == false;
        }
        private bool CallEdit_IsResourcePath_Func()
        {
            return this.isProejctSettingCargoldStyle == true || Editor_C.TryGetExtractResourcePath_Func(this.libraryDataSobjPath, out _);
        }
        [Button("라이브러리 데이터 생성"), EnableIf("CallEdit_IsResourcePath_Func"), HideIf("IsInstance_Func")
            , InfoBox("경로에 Resources 폴더가 포함되지 않았습니다.", "CallEdit_IsNotResourcePath_Func", InfoMessageType = InfoMessageType.Error)]
        private void CallEdit_LibraryDataSobjGenerate_Func()
        {
            string _libraryScriptsPath = null;
            string _assetExampleFolderPath = null;
            string _resourcesFolderPath = null;
            string _animationFolderPath = null;
            string _spriteFolderPath = null;
            string _soundFolderPath = null;
            string _prefabFolderPath = null;

            // 카골드 스타일의 프로젝트인가요?
            if (this.isProejctSettingCargoldStyle == true)
            {
                // Scene 폴더 만들기
                string _initScenePath = Editor_C.GetPath_Func(GetAseetsStr, GetFolderName_1Scenes);
                Editor_C.TryCheckOrGenerateFolder_Func(_initScenePath);

                // Script 및 카라리용 폴더 만들기
                _libraryScriptsPath = GetFolderApath_Scripts_C;
                Editor_C.TryCheckOrGenerateFolder_Func(_libraryScriptsPath);

                // Resource 폴더 만들기
                string _initResourcesPath = Editor_C.GetPath_Func(GetAseetsStr, GetFolderName_3Resources);
                Editor_C.TryCheckOrGenerateFolder_Func(_initResourcesPath);

                _assetExampleFolderPath = Editor_C.GetPath_Func(_initResourcesPath, GetLibraryFolderName);

                _resourcesFolderPath = Editor_C.GetPath_Func(_initResourcesPath, GetResourcesStr, GetLibraryFolderName);
                this.libraryDataSobjPath = _resourcesFolderPath;

                _animationFolderPath = Editor_C.GetPath_Func(_resourcesFolderPath, "Animation");
                _spriteFolderPath = Editor_C.GetPath_Func(_resourcesFolderPath, "Sprite");
                _soundFolderPath = Editor_C.GetPath_Func(_resourcesFolderPath, "Sound");
                _prefabFolderPath = Editor_C.GetPath_Func(_resourcesFolderPath, "Prefab");
            }

            // Sobj 생성
            if (Editor_C.GenerateResult.Success.HasFlag(
                Editor_C.TryGetLoadWithGenerateSobj_Func(LibraryRemocon.Str, out LibraryRemocon _libraryData, _aPath: _resourcesFolderPath)) == true) 
            {
                string _scriptFolderPath = default;
                if (this.isProejctSettingCargoldStyle == true)
                    _scriptFolderPath = _libraryScriptsPath;

                // 예제 경로 찾기
                Editor_C.TryGetFindFolderPath_Func("Example_C", out string _exampleScriptFolderPath);
                Editor_C.TryGetFindFolderPath_Func("AnimationExample_C", out string _exampleAnimationFolderPath);
                Editor_C.TryGetFindFolderPath_Func("PrefabExample_C", out string _examplePrefabFolderPath);

                // 예제 스크립트 경로 캐싱하기
                LibraryRemocon.SetCatching_Func(_libraryData, _scriptFolderPath, _resourcesFolderPath, _assetExampleFolderPath
                    , _exampleScriptFolderPath, _exampleAnimationFolderPath, _examplePrefabFolderPath
                    , _animationFolderPath, _spriteFolderPath, _soundFolderPath, _prefabFolderPath
                    , this.isProejctSettingCargoldStyle);

                // 카라리 재정의용 스크립트 생성
                LibraryRemocon.CallEdit_Generate_Script_Func(_exampleScriptFolderPath, typeof(Cargold.Example.라이브러리), LibraryRemocon.Str, _del: (string _code) =>
                {
                     _code = _code.Replace("// 네임스페이스시작", @"namespace Cargold
{");
                     _code = _code.Replace(typeof(Cargold.Example.라이브러리.프레임워크).Name,
                         typeof(LibraryRemocon.FrameWorkData).Name);

                     _code = _code.Replace(typeof(Cargold.Example.라이브러리.프레임워크.데이터베이스).Name,
                         typeof(LibraryRemocon.FrameWorkData.DatabaseData).Name);

                     _code = _code.Replace(typeof(Cargold.Example.라이브러리.프레임워크.데이터베이스.테이블임포터).Name,
                         typeof(LibraryRemocon.FrameWorkData.DatabaseData.TableImporterData).Name);

                     _code = _code.Replace("// 네임스페이스끝", "}");

                     return _code;
                 }, _scriptName: "LibraryOverride_C");

                Editor_C.TryCheckOrGenerateFolder_Func(GetFolderApath_ResourcesC);

                // 카라리 데이터 스옵젝의 경로를 저장할 String Data 스옵젝 생성
                if (Editor_C.GenerateResult.Success.HasFlag(
                        Editor_C.TryGetLoadWithGenerateSobj_Func(GetPathDataName, out StringData_C _stringData, typeof(StringData_C), GetFolderApath_ResourcesC)) == true)
                {
                    if (Editor_C.TryGetExtractResourcePath_Func(_resourcesFolderPath, out string _resourcePath) == false)
                        Debug_C.Error_Func("?");

                    // 카라리 데이터 스옵젝 경로 저장
                    string _resourceFullPath = Editor_C.GetPath_Func(_resourcePath, LibraryRemocon.Str);
                    _stringData.SetValue_Func(_resourceFullPath);
                }
            }

            Editor_C.SetDefineSymbol_Func(UnityEditor.BuildTargetGroup.Standalone, "Test_Cargold");
            Editor_C.SetDefineSymbol_Func(UnityEditor.BuildTargetGroup.Android, "Test_Cargold");
        }
#endif
    }
}