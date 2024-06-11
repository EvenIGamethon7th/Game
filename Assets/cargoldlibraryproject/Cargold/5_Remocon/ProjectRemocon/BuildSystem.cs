using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Cargold.Remocon
{
    [System.Serializable]
    public class BuildSystem
    {
        private const string Str1 = "출시 세팅";
        private const string Str1S = Str1 + "/";

        private const string Str2 = "기본 정보";
        private const string Str2S = Str2 + "/";

        private const string BuildStr = "빌드";

#if UNITY_EDITOR
#if UNITY_ANDROID
        [SerializeField, LabelText("안드로이드 빌드 타입"), GUIColor(1f, .25f, .25f), OnValueChanged("CallEdit_SetBundle_Func")] private AndroidBuildType androidBuildType = AndroidBuildType.AAB;
#endif
        [FoldoutGroup("디파인 심볼"), LabelText("기본"), SerializeField] private string[] defaultDefineArr = new string[] { "ODIN_INSPECTOR", "ODIN_INSPECTOR_3", "ODIN_INSPECTOR_3_1", };
        [FoldoutGroup("디파인 심볼"), LabelText("테스트"), SerializeField] private string[] testDefineArr = new string[] { "Test_Cargold" };

        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + Str2), LabelText("앱 아이콘"), PreviewField(150f, ObjectFieldAlignment.Left)] private Texture2D appIconTexture2D = null;
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + Str2), LabelText("조직 이름")] private string companyName = "조직 이름 뭐에영?";
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + Str2), LabelText("게임 이름")] private string productName = "게임 이름 뭐에영?";
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + Str2), LabelText("별도의 패키지 네임"), OnValueChanged("CallEdit_SetPackageName_Func")] private bool isPackageNameOverride = false;
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + Str2), HorizontalGroup(Str1S + Str2S + "1"), HideLabel, ShowIf("isPackageNameOverride")] private string packageName1 = null;
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + Str2), HorizontalGroup(Str1S + Str2S + "1"), HideLabel, ShowIf("isPackageNameOverride")] private string packageName2 = null;
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + Str2), HorizontalGroup(Str1S + Str2S + "1"), HideLabel, ShowIf("isPackageNameOverride")] private string packageName3 = null;
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + "스플래쉬"), LabelText("사용 여부(Pro 전용)")] private bool isSplash = false;
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + "스플래쉬"), LabelText("스프라이트"), PreviewField(150f, ObjectFieldAlignment.Left)] private Sprite splashSprite = null;
        [SerializeField, FoldoutGroup(Str1), BoxGroup(Str1S + "스플래쉬"), LabelText("배경 컬러")] private Color splashColor = new Color(0.07843138f, 0.07843138f, 0.07843138f, 1f);
        [SerializeField, FoldoutGroup(Str1), FoldoutGroup(Str1S + "키스토어", false), LabelText("경로"), Sirenix.OdinInspector.FilePath] private string KeystorePath = "user.keystore";
        [SerializeField, FoldoutGroup(Str1), FoldoutGroup(Str1S + "키스토어", false), LabelText("비번")] private string KeystorePassward = "키스토어 비번 ㄱㄱ";
        [SerializeField, FoldoutGroup(Str1), FoldoutGroup(Str1S + "Alias", false), LabelText("이름")] private string KeyAliasName = "Alias 이름 ㄱㄱ";
        [SerializeField, FoldoutGroup(Str1), FoldoutGroup(Str1S + "Alias", false), LabelText("비번")] private string KeyAliasPassward = "Alias 비번 ㄱㄱ";
        [SerializeField, FoldoutGroup(BuildStr)] private StringData_C buildPathStrData = null;

        [SerializeField, FoldoutGroup("$CallEdit_VersionLabel_Func"), LabelText("버전 Tmp")] private List<Version_Script> versionClassList = new List<Version_Script>();

        public static bool IsTestMode =>
#if Test_Cargold
            true;
#else
            false;
#endif
#endif
        [SerializeField, FoldoutGroup("$CallEdit_VersionLabel_Func"), ToggleLeft, LabelText("$CallEdit_GetVersionDesc_Func")] private bool isManualVersionSetting = false;
        [SerializeField, FoldoutGroup("$CallEdit_VersionLabel_Func"), HorizontalGroup("$CallEdit_VersionLabel_Func" + "/1"), HideLabel, EnableIf("isManualVersionSetting")] private int version1 = 0;
        [SerializeField, FoldoutGroup("$CallEdit_VersionLabel_Func"), HorizontalGroup("$CallEdit_VersionLabel_Func" + "/1"), HideLabel, EnableIf("isManualVersionSetting")] private int version2 = 0;
        [SerializeField, FoldoutGroup("$CallEdit_VersionLabel_Func"), HorizontalGroup("$CallEdit_VersionLabel_Func" + "/1"), HideLabel, EnableIf("isManualVersionSetting")] private string version3 = "0";

        private string GetNextVersion3_Func(bool _isGetRuntume = false)
        {
            if (this.isManualVersionSetting == true || _isGetRuntume == true)
                return this.version3;
            else
                return (this.version3.ToInt() + 1).ToString();
        }
        public int GetVersion_Func(bool _isGetRuntume = true)
        {
            string _version3Str = this.GetNextVersion3_Func(_isGetRuntume);
            int _ver3 = _version3Str.Replace("v", "").ToInt();
            return (this.version1 * 1000000) + (this.version2 * 1000) + (_ver3);
        }

#if UNITY_EDITOR
        public string CallEdit_GetVersionStr_Func(string _version3 = null)
        {
            if (_version3.IsNullOrWhiteSpace_Func() == true)
                _version3 = this.GetNextVersion3_Func();

            return string.Format("{0}.{1}.{2}v", this.version1, this.version2, _version3);
        }

        [FoldoutGroup("$CallEdit_GetModeDesc_Func"), Button("테스트 모드로 바꾸기"), GUIColor(.4f, .4f, 1f, 1f), HideIf("IsTestMode")]
        private void CallEdit_TestModeOn_Func() => this.SetDefine_Func(true);
        [FoldoutGroup("$CallEdit_GetModeDesc_Func"), Button("공식 모드로 바꾸기"), GUIColor(1f, .4f, .4f, 1f), ShowIf("IsTestMode")]
        private void CallEdit_TestModeOff_Func() => this.SetDefine_Func(false);
        private void SetDefine_Func(bool _isTestMode)
        {
            BuildTargetGroup _buildTargetGroup = default;
#if UNITY_ANDROID
            _buildTargetGroup = _CallAOS_Func();
#elif UNITY_IOS
            _buildTargetGroup = _CallIOS_Func();
#endif

            if(_buildTargetGroup == default)
            {
                Debug.LogError("플랫폼 설정이 안 되어있음!!!");
                return;
            }

            List<string> _defineList = new List<string>();

            if (_isTestMode == true)
            {
                bool _isContain = false;
                for (int i = 0; i < this.testDefineArr.Length; i++)
                {
                    if (this.testDefineArr[i].IsCompare_Func("Test_Cargold") == true)
                    {
                        _isContain = true;
                        break;
                    }
                }

                if (_isContain == false)
                {
                    this.testDefineArr = this.testDefineArr.GetAdd_Func("Test_Cargold");

                    Editor_C.SetSaveAsset_Func(ProjectRemocon.Instance);

                    Debug.LogWarning("디파인 심볼 테스트에 Test_Cargold은 필수이므로 강제 추가했스므니다!!!");
                }

                _SetDefine_Func(this.testDefineArr);
            }

            _SetDefine_Func(this.defaultDefineArr);

            
            Editor_C.SetDefineSymbol_Func(_buildTargetGroup, _defineList);

            void _SetDefine_Func(string[] _defineArr)
            {
                foreach (string _define in _defineArr)
                {
                    string _str = StringBuilder_C.Append_Func(_define, ";");
                    _defineList.Add(_str);
                }
            }
            BuildTargetGroup _CallAOS_Func() => BuildTargetGroup.Android;
            BuildTargetGroup _CallIOS_Func() => BuildTargetGroup.iOS;
        }

        public void CallEdit_AddDefine_Func(string _str)
        {
            if (this.defaultDefineArr.Contain_Func(_str) == true)
            {
                Debug.Log($"{_str} 디파인 심볼은 이미 있습니다.");
                return;
            }

            this.defaultDefineArr = this.defaultDefineArr.GetAdd_Func(_str);

            this.SetDefine_Func(false);

            Debug.Log($"{_str} 디파인 심볼을 추가했습니다.");
        }
        public void RemoveDefine_Func(string _str)
        {
            if(this.defaultDefineArr.Contain_Func(_str) == true)
            {
                this.defaultDefineArr = this.defaultDefineArr.GetRemove_Func(_str);

                this.SetDefine_Func(IsTestMode);

                Debug.Log($"{_str} 디파인 심볼을 제거했습니다.");
            }
            else
            {
                Debug.Log($"{_str} 디파인 심볼이 없으므로 제거하지 않았습니다.");
            }
        }

        [Button("강제로 세팅하기")]
        private BuildTargetGroup CallEdit_SetVersion_Func()
        {
            if (BuildSystem.IsTestMode == true)
                this.CallEdit_TestModeOn_Func();
            else
                this.CallEdit_TestModeOff_Func();

            int _versionCode = this.GetVersion_Func(false);

            string _versionCodeStr = _versionCode.ToString();
            string _versionStr = this.CallEdit_GetVersionStr_Func();

            // 타이틀 버전 갱신
            if (this.versionClassList.IsHave_Func() == true)
            {
                foreach (Version_Script _versionClass in this.versionClassList)
                {
                    _versionClass.SetVersion_Func(_versionStr);
                    UnityEditor.PrefabUtility.SavePrefabAsset(_versionClass.gameObject);
                }
            }

            BuildTargetGroup _buildTargetGroup = BuildTargetGroup.Unknown;

#if UNITY_ANDROID
            _CallAos_Func(out _buildTargetGroup);
#elif UNITY_IOS
            _CallIos_Func(out _buildTargetGroup, out _versionStr);
#endif

            PlayerSettings.companyName = this.companyName;
            PlayerSettings.productName = this.productName;

            string _packageName = this.isPackageNameOverride == false
                ? StringBuilder_C.Append_Func("com.", this.companyName, ".", this.productName)
                : StringBuilder_C.Append_Func(this.packageName1, ".", this.packageName2, ".", this.packageName3);
            PlayerSettings.SetApplicationIdentifier(_buildTargetGroup, _packageName);

            PlayerSettings.bundleVersion = _versionStr;
            PlayerSettings.SplashScreen.show = this.isSplash;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            if(this.splashSprite != null)
            {
                PlayerSettings.SplashScreenLogo _splashScreenLogo = new PlayerSettings.SplashScreenLogo();
                _splashScreenLogo.logo = this.splashSprite;
                PlayerSettings.SplashScreen.logos = new PlayerSettings.SplashScreenLogo[]
                {
                    _splashScreenLogo
                };

                PlayerSettings.SplashScreen.backgroundColor = this.splashColor;
            }

            return _buildTargetGroup;

            void _CallAos_Func(out BuildTargetGroup _aosBuildTargetGroup)
            {
                // 230730 UnityEditor.Android.AndroidPlatformIconKind에 접근할 수가 없는데, 원인을 못 찾겠음 ㅠㅠ
                //PlatformIconKind _kind = UnityEditor.Android.AndroidPlatformIconKind.Adaptive;
                //PlayerSettings.SetPlatformIcons(BuildTargetGroup.Android, _kind, null);

                if (this.appIconTexture2D != null)
                {
                    PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new Texture2D[]
                    {
                        this.appIconTexture2D,
                        this.appIconTexture2D,
                        this.appIconTexture2D,
                        this.appIconTexture2D,
                        this.appIconTexture2D,
                        this.appIconTexture2D,
                    });
                }

#if UNITY_ANDROID
                if (this.androidBuildType == AndroidBuildType.AAB)
                {
                    PlayerSettings.Android.useCustomKeystore = true;
                    PlayerSettings.Android.keystoreName = KeystorePath;
                    PlayerSettings.Android.keystorePass = KeystorePassward;
                    PlayerSettings.Android.keyaliasName = KeyAliasName;
                    PlayerSettings.Android.keyaliasPass = KeyAliasPassward;
                }
                else
                {
                    PlayerSettings.Android.useCustomKeystore = false;
                } 
#endif

                PlayerSettings.Android.bundleVersionCode = _versionCode;

                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
                PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)33;

                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

                _aosBuildTargetGroup = BuildTargetGroup.Android;

                this.CallEdit_SetBundle_Func();
            }

            void _CallIos_Func(out BuildTargetGroup _iosBuildTargetGroup, out string _iosVersionStr)
            {
                PlayerSettings.iOS.buildNumber = _versionCodeStr;
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
                PlayerSettings.iOS.targetOSVersionString = "11.0";
                PlayerSettings.statusBarHidden = true;

                _iosBuildTargetGroup = BuildTargetGroup.iOS;
                _iosVersionStr = _versionCodeStr;
            }
        }

        private string GetBuildPath_Func()
        {
            if(this.buildPathStrData == null)
            {
                string _path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
                return StringBuilder_C.Append_Func(_path, System.IO.Path.DirectorySeparatorChar.ToString(), "Build");
            }
            else
            {
                return this.buildPathStrData.GetValue;
            }
        }

        [Button("빌드 폴더 열기")]
        public void OnBuildFolderOpen_Func()
        {
            this.OnBuildFolderOpen_Func(null);
        }
        public void OnBuildFolderOpen_Func(string _path)
        {
            if (_path.IsNullOrWhiteSpace_Func() == true)
                _path = this.GetBuildPath_Func();

            Editor_C.OpenWindowFolder_Func(_path);
        }

        [Button("빌드", ButtonSizes.Gigantic)]
        private void CallEdit_Build_Func()
        {
            this.CallEdit_SetVersion_Func();

            BuildPlayerOptions _buildPlayerOptions = new BuildPlayerOptions();

            EditorBuildSettingsScene[] _scenes = UnityEditor.EditorBuildSettings.scenes;
            List<string> _sceneNameStrList = new List<string>();
            foreach (EditorBuildSettingsScene _scene in _scenes)
                _sceneNameStrList.Add(_scene.path);
            _buildPlayerOptions.scenes = _sceneNameStrList.ToArray();

            System.DateTime _nowTime = System.DateTime.Now;

            string _buildPath = this.GetBuildPath_Func();
            string _nameStr = string.Format(StringBuilder_C.Append_Func(System.IO.Path.DirectorySeparatorChar.ToString(), "{0:00}{1:00}{2:00}_{3}_")
                , _nowTime.Year, _nowTime.Month, _nowTime.Day
                , PlayerSettings.productName
                );
            string _fullPath = StringBuilder_C.Append_Func(_buildPath, _nameStr);

            BuildTarget _buildTarget = default;

#if UNITY_ANDROID
            _AOS_Func(this.androidBuildType);
#elif UNITY_IOS
            _IOS_Func();
#else
    Debug.LogError("?");
                return;
#endif

            _buildPlayerOptions.target = _buildTarget;
            _buildPlayerOptions.locationPathName = _fullPath;

            UnityEditor.Build.Reporting.BuildReport _report = BuildPipeline.BuildPlayer(_buildPlayerOptions);

            this.OnBuildFolderOpen_Func(_buildPath);

            if(_report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                if(this.isManualVersionSetting == false)
                    this.version3 = this.GetNextVersion3_Func();
            }

            Editor_C.SetSaveAsset_Func(ProjectRemocon.Instance);

            Debug.Log($"빌드 결과 : {_report.summary.result }" +
                $"\n소요 시간 : { _report.summary.totalTime }" +
                $"\n시작 시점 : { _report.summary.buildStartedAt + System.DateTimeOffset.Now.Offset }" +
                $"\n종료 시점 : { _report.summary.buildEndedAt + System.DateTimeOffset.Now.Offset }" +
                $"\n플랫폼: { _report.summary.platform }" +
                $"\n경로: { _fullPath }");

            void _AOS_Func(AndroidBuildType _androidBuildType)
            {
                _buildTarget = BuildTarget.Android;

                string _extensionStr = null;
                if (_androidBuildType == AndroidBuildType.AAB)
                    _extensionStr = ".aab";
                else if (_androidBuildType == AndroidBuildType.Apk)
                    _extensionStr = ".apk";
                else
                    Debug.LogError("_androidBuildType : " + _androidBuildType);

                _fullPath = StringBuilder_C.Append_Func(_fullPath
                    , PlayerSettings.Android.bundleVersionCode.ToString()
                    , _extensionStr);
            }
            void _IOS_Func()
            {
                _buildTarget = BuildTarget.iOS;

                _fullPath = StringBuilder_C.Append_Func(_fullPath
                    , PlayerSettings.iOS.buildNumber
                    );
            }
        }

        [FoldoutGroup(BuildStr), Button("경로 스옵젝 생성하기"), HideIf("@this.buildPathStrData != null")]
        private void CallEdit_SetBuildPath_Func()
        {
            if (Editor_C.GenerateResult.Success.HasFlag(
                        Editor_C.TryGetLoadWithGenerateSobj_Func("빌드경로", out StringData_C _stringData, typeof(StringData_C), CargoldLibrary_C.GetFolderApath_Resources_C)) == true)
            {
                this.buildPathStrData = _stringData;
            }
        }

        private void CallEdit_SetBundle_Func()
        {
#if UNITY_ANDROID
            EditorUserBuildSettings.buildAppBundle = this.androidBuildType == AndroidBuildType.AAB; 
#endif
        }
        private void CallEdit_SetPackageName_Func()
        {
            if(this.isPackageNameOverride == true)
            {
                if (this.packageName1.IsNullOrWhiteSpace_Func() == true)
                    this.packageName1 = "com";

                if (this.packageName2.IsNullOrWhiteSpace_Func() == true)
                    this.packageName2 = this.companyName;

                if (this.packageName3.IsNullOrWhiteSpace_Func() == true)
                    this.packageName3 = this.productName;
            }
        }
        private string CallEdit_VersionLabel_Func()
        {
            if (this.isManualVersionSetting == true)
            {
                return $"프로젝트 버전 (빌드 버전 : {this.CallEdit_GetVersionStr_Func()})";
            }
            else
            {
                string _nextVersion3Str = this.GetNextVersion3_Func();
                string _nextVersionStr = this.CallEdit_GetVersionStr_Func(_nextVersion3Str);
                return $"프로젝트 버전 (현재 버전 : {this.CallEdit_GetVersionStr_Func(this.version3)}   ->   빌드 버전 : {_nextVersionStr})";
            }
        }
        private string CallEdit_GetVersionDesc_Func()
        {
            if (this.isManualVersionSetting == true)
            {
                return "수동으로 버전을 조정할 수 있습니다.";
            }
            else
            {
                return "자동으로 3번째 버전이 1 올라갑니다.";
            }
        }
        private string CallEdit_GetModeDesc_Func()
        {
            if(IsTestMode == true)
            {
                return "모드 (테스트 모드)";
            }
            else
            {
                return "모드 (공식 모드)";
            }
        }
        public void CallEdit_Subscribe_Version_Func(Version_Script _versionClass)
        {
            this.versionClassList.AddNewItem_Func(_versionClass);

            _versionClass.SetVersion_Func(this.CallEdit_GetVersionStr_Func());

            Editor_C.SetSaveAsset_Func(ProjectRemocon.Instance);
        }

        public enum AndroidBuildType
        {
            None = 0,

            Apk,
            AAB,
        }
#endif
    }
}