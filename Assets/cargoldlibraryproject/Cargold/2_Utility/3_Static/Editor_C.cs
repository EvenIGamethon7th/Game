using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; 
#endif

namespace Cargold
{
    public static class Editor_C
    {
        // rPath = Resources Path = Resources 폴더부터 시작하는 경로
        // aPath = Assets Path = Assets으로 시작하는 경로
        // fPath = Full Path = Application.dataPath로 시작하여 유니티 프로젝트 경로까지 포함하는 것
        // path = 경우에 따라 달라지는 경로. 정의되지 않은 경로

        public const string SeparatorStr = "/";
        public const char SeparatorChar = '/';
        public const string GetUnityStartPathStr = "Assets";

        public const string Mandatory = "필수";
        public const string Optional = "선택";
        public const string OptionS = Editor_C.Optional + "/";

#if UNITY_EDITOR
        /// <summary>
        /// 런타임 상태인가요?
        /// </summary>
        public static bool IsRuntime => EditorApplication.isPlaying;

        public static string GetPath_Func(UnityEngine.Object _object, bool _isLog = false)
        {
            string _pathStr = AssetDatabase.GetAssetPath(_object);

            if (_isLog == true)
                Debug_C.Log_Func("_pathStr : " + _pathStr);

            return _pathStr;
        }

        public static UnityEngine.Object GetLoadAssetAtPath_Func(string _path, bool _isLog = true, AssetType _assetType = AssetType.None)
        {
            if (_assetType != AssetType.None)
            {
                string _extensionStr = GetTypeExtensionStr_Func(_assetType, _isLog);
                _path = StringBuilder_C.Append_Func(_path, _extensionStr);
            }

            UnityEngine.Object _obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(_path);
            if (_obj == null && _isLog == true)
                Debug.LogError($"'{_path}'경로의 '{_assetType.ToString_Func()}'을 못 불러옴 ~");

            return _obj;
        }
        public static T GetLoadAssetAtPath_Func<T>(string _path, bool _isLog = true, bool _isAddExtentionStr = false) where T : UnityEngine.Object
        {
            if (_isAddExtentionStr == true)
            {
                Type _type = typeof(T);
                string _extensionStr = GetTypeExtensionStr_Func(_type, _isLog);
                _path = StringBuilder_C.Append_Func(_path, _extensionStr);
            }

            T _asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_path);
            if (_asset == null && _isLog == true)
                Debug.LogError($"'{_path}'경로의 '{typeof(T)}'을 못 불러옴 ~");

            return _asset;
        }
        public static List<string> GetSubFolderPath_Func(string _path)
        {
            List<string> _pathList = new List<string>();

            SetSubFolderPath_Func(_path, _pathList, true);

            return _pathList;
        }
        public static bool TryGetFindFolderPath_Func(string _folderName, out string _findPath, string _startPath = null)
        {
            if (_startPath.IsNullOrWhiteSpace_Func() == true)
                _startPath = "Assets";

            string[] _subFolderPathArr = UnityEditor.AssetDatabase.GetSubFolders(_startPath);
            if (0 < _subFolderPathArr.Length)
            {
                foreach (string _subFolderPath in _subFolderPathArr)
                {
                    string _path = StringBuilder_C.Append_Func(_startPath, Editor_C.SeparatorStr, _folderName);
                    if (_path.IsCompare_Func(_subFolderPath) == false)
                    {
                        if (TryGetFindFolderPath_Func(_folderName, out _findPath, _subFolderPath) == true)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        _findPath = _subFolderPath;

                        return true;
                    }
                }
            }

            _findPath = null;

            return false;
        }

        public static string GetTypeExtensionStr_Func(Type _type, bool _isLog = true)
        {
            return GetTypeExtensionStr_Func(_type.Name);
        }
        public static string GetTypeExtensionStr_Func(AssetType _assetType, bool _isLog = true)
        {
            string _typeStr = _assetType.ToString_Func();
            return GetTypeExtensionStr_Func(_typeStr, _isLog);
        }
        public static string GetTypeExtensionStr_Func(string _typeStr, bool _isLog = true)
        {
            string _extensionStr = string.Empty;
            switch (_typeStr)
            {
                case "AnimationClip":
                    _extensionStr = ".anim";
                    break;

                case "GameObject":
                case "Prefab":
                    _extensionStr = ".prefab";
                    break;

                case "Sprite":
                case "Texture":
                    _extensionStr = ".png";
                    break;

                case "ScriptableObject":
                    _extensionStr = ".asset";
                    break;

                case "Script":
                    _extensionStr = ".cs";
                    break;

                case "Material":
                    _extensionStr = ".mat";
                    break;

                default:
                    if (_isLog == true)
                        Debug.LogError("GetTypeExtensionStr_Func) 정의하지 않은 타입입니다. : " + _typeStr);
                    break;
            }

            return _extensionStr;
        }
        public static string GetScriptContent_Func(string _path, Type _type, bool _isLog = true)
        {
            string _scriptName = _type.Name;
            string _extensionStr = GetTypeExtensionStr_Func(AssetType.Script, _isLog);
            string _scriptPath = Editor_C.GetPath_Func(_path, StringBuilder_C.Append_Func(_scriptName, _extensionStr));
            return File.ReadAllText(_scriptPath, System.Text.Encoding.UTF8);
        }
        public static string GetScriptContent_Func(string _path, string _scriptName, bool _isLog = true)
        {
            string _extensionStr = GetTypeExtensionStr_Func(AssetType.Script, _isLog);
            string _scriptPath = Editor_C.GetPath_Func(_path, StringBuilder_C.Append_Func(_scriptName, _extensionStr));
            return File.ReadAllText(_scriptPath, System.Text.Encoding.UTF8);
        }
        public static string GetFirstCharLower_Func(string _str)
        {
            return StringBuilder_C.Append_Func(_str[0].ToString().ToLower(), _str.Substring(1));
        }

        public static List<T> GetLoadAssetListAtPath_Func<T>(string _path,
            string[] _containNameFilterArr = null, string[] _excludeNameFilterArr = null, bool _isContainSubFolder = true, bool _isLog = true) where T : UnityEngine.Object
        {
            Type _type = typeof(T);
            string _extensionStr = GetTypeExtensionStr_Func(_type, _isLog);

            return Editor_C.GetLoadAssetListAtPath_Func<T>(_path, _extensionStr, _containNameFilterArr, _excludeNameFilterArr, _isContainSubFolder);
        }
        public static List<T> GetLoadAssetListAtPath_Func<T>(string _path, AssetType _assetType,
            string[] _containNameFilterArr = null, string[] _excludeNameFilterArr = null, bool _isContainSubFolder = true) where T : UnityEngine.Object
        {
            string _extensionStr = GetTypeExtensionStr_Func(_assetType);
            return GetLoadAssetListAtPath_Func<T>(_path, _extensionStr, _containNameFilterArr, _excludeNameFilterArr, _isContainSubFolder);
        }
        public static List<T> GetLoadAssetListAtPath_Func<T>(string _path, string _extensionStr,
            string[] _containNameFilterArr = null, string[] _excludeNameFilterArr = null, bool _isContainSubFolder = true) where T : UnityEngine.Object
        {
            List<string> _assetPathList = new List<string>();
            Editor_C.SetAssetPath_Func(_path, _assetPathList, _containNameFilterArr, _excludeNameFilterArr);

            if (_isContainSubFolder == true)
            {
                List<string> _folderPathList = Editor_C.GetSubFolderPath_Func(_path);

                foreach (string _folderPath in _folderPathList)
                    Editor_C.SetAssetPath_Func(_folderPath, _assetPathList, _containNameFilterArr, _excludeNameFilterArr);
            }

            for (int i = _assetPathList.Count - 1; i >= 0; i--)
            {
                string _assetPath = _assetPathList[i];
                if (_assetPath.Contains(_extensionStr) == false)
                {
                    _assetPathList.RemoveAt(i);
                }
            }

            List<T> _assetList = new List<T>();
            foreach (var _assetPath in _assetPathList)
            {
                T _asset = Editor_C.GetLoadAssetAtPath_Func<T>(_assetPath);
                _assetList.Add(_asset);
            }

            return _assetList;
        }

        public static void SetAssetPath_Func(string _targetPath, List<string> _assetPathList,
            string[] _containNameFilterArr = null, string[] _excludeNameFilterArr = null, bool _isExcludeMeta = true)
        {
            string[] _assetPathArr = System.IO.Directory.GetFiles(_targetPath);

            if (0 < _assetPathArr?.Length)
            {
                foreach (string _assetPath in _assetPathArr)
                {
                    if (_isExcludeMeta == true && _assetPath.Contains(".meta") == true)
                        continue;

                    bool _isContainDone = false;

                    if (_containNameFilterArr == null || _containNameFilterArr.Length == 0)
                    {
                        _isContainDone = true;
                    }
                    else
                    {
                        foreach (string _containName in _containNameFilterArr)
                        {
                            if (_assetPath.Contains(_containName) == true)
                            {
                                _isContainDone = true;
                                break;
                            }
                        }
                    }

                    bool _isExcludeDone = true;

                    if (_excludeNameFilterArr != null && 0 <= _excludeNameFilterArr.Length)
                    {
                        foreach (string _excludeName in _excludeNameFilterArr)
                        {
                            if (_assetPath.Contains(_excludeName) == true)
                            {
                                _isExcludeDone = false;
                                break;
                            }
                        }
                    }

                    if (_isContainDone == true && _isExcludeDone == true)
                        _assetPathList.Add(_assetPath);
                }
            }
        }

        public static void SetSubFolderPath_Func(string _path, List<string> _pathList, bool _isCallHeadFolder = true, params string[] _excludeFolderNameArr)
        {
            string[] _subFolderPathArr = UnityEditor.AssetDatabase.GetSubFolders(_path);

            if (0 < _subFolderPathArr.Length)
            {
                foreach (string _subFolderPath in _subFolderPathArr)
                {
                    SetSubFolderPath_Func(_subFolderPath, _pathList, false);
                }
            }
            else
            {
                bool _isAddable = _isCallHeadFolder != true;

                if (0 < _excludeFolderNameArr?.Length)
                {
                    foreach (var _exclusionFolderName in _excludeFolderNameArr)
                    {
                        if (_path.Contains(_exclusionFolderName) == true)
                        {
                            _isAddable = false;
                            break;
                        }
                    }
                }

                if (_isAddable == true)
                    _pathList.Add(_path);
            }
        }

        public static bool TryCheckOrGenerateFolder_Func(string _aPath, bool _isLog = true, bool _isGenerate = true)
        {
            if (Directory.Exists(_aPath) == false)
            {
                if (_isGenerate == false)
                    return false;

                string[] _pathArr = _aPath.Split(Editor_C.SeparatorChar);
                string _currentPath = _pathArr[0];

                for (int i = 1; ; i++)
                {
                    if (Directory.Exists(_currentPath) == false)
                    {
                        Directory.CreateDirectory(_currentPath);

                        if (_isLog == true)
                            Debug_C.Log_Func("다음 경로에 폴더를 생성합니다. : " + _currentPath);
                    }

                    if (_pathArr.Length <= i)
                        break;

                    string _pathItem = _pathArr[i];
                    _currentPath = StringBuilder_C.Append_Func(_currentPath, Editor_C.SeparatorStr, _pathItem);
                }

                UnityEditor.AssetDatabase.Refresh();
            }

            return true;
        }
        public static GenerateResult TryGetLoadWithGenerateScript_Func(string _path, string _className, string _code,
            bool _isOverWrite = false, bool _isFolderGenerate = true, string _scriptName = null, bool _isRefresh = true, bool _isLog = true)
        {
            if (Editor_C.TryCheckOrGenerateFolder_Func(_path, _isLog, _isFolderGenerate) == false)
                return GenerateResult.Fail_NoneFolder;

            if (_scriptName.IsNullOrWhiteSpace_Func() == true)
                _scriptName = _className;

            string _fullPath = Editor_C.GetPath_Func(_path, StringBuilder_C.Append_Func(_scriptName, ".cs"));

            bool _isExist = File.Exists(_fullPath);
            if (_isOverWrite == true || _isExist == false)
            {
                File.WriteAllText(_fullPath, _code);

                if (_isRefresh == true)
                    UnityEditor.AssetDatabase.Refresh();

                if (_isLog == true)
                    Debug_C.Log_Func("다음 스크립트를 생성했습니다. / 경로 : " + _fullPath);

                return GenerateResult.Success_Generate;
            }
            else
            {
                if (_isLog == true)
                    Debug_C.Log_Func("이미 스크립트가 존재하며, 덮어쓰지 않습니다. / 경로 : " + _fullPath);

                return GenerateResult.Fail_Exist;
            }
        }
        public static GenerateResult TryGetLoadWithGenerateSobj_Func<T>(string _resourceName, out T _loadObj, string _typeName,
            string _aPath = null, bool _isRefresh = true, bool _isLog = true) where T : UnityEngine.ScriptableObject
        {
            Type _type = Type.GetType(_typeName);
            return TryGetLoadWithGenerateSobj_Func<T>(_resourceName, out _loadObj, _type, _aPath, _isRefresh, _isLog);
        }
        public static GenerateResult TryGetLoadWithGenerateSobj_Func<T>(string _sobjName, out T _loadSobj,
            Type _type = null, string _aPath = null, bool _isRefresh = true, bool _isLog = true) where T : UnityEngine.ScriptableObject
        {
            return TryGetLoadWithGenerateAsset_Func(_sobjName, out _loadSobj, (Type _typeDel, string _fullPath) =>
            {
                T _creatSobj = ScriptableObject.CreateInstance(_typeDel) as T;

                if (_creatSobj != null)
                {
                    UnityEditor.AssetDatabase.CreateAsset(_creatSobj, _fullPath);

                    if (_isRefresh == true)
                        UnityEditor.AssetDatabase.Refresh();

                    if (_isLog == true)
                        Debug_C.Log_Func("스옵젝 생성) _fullPath : " + _fullPath);

                    Coroutine_C.Invoke_Func(() =>
                    {
                        Editor_C.SetSaveAsset_Func(_creatSobj);
                    });

                    return _creatSobj;
                }
                else
                {
                    if (_isLog == true)
                        Debug_C.Error_Func("리소스 생성 실패) 다음 타입으로 캐스팅할 수 없습니다. _typeDel : " + _typeDel);

                    return null;
                }
            }
            , AssetType.ScriptableObject, _type, _aPath, _isRefresh, _isLog);
        }
        public static GenerateResult TryGetLoadWithGeneratePrefab_Func<T>(string _prefabName, out T _loadComponent, bool _isGenerateScene = false,
            Type _type = null, string _aPath = null, bool _isRefresh = true, bool _isLog = true) where T : MonoBehaviour
        {
            return TryGetLoadWithGenerateAsset_Func(_prefabName, out _loadComponent, (Type _tempType, string _fullPath) =>
            {
                GameObject _generateObj = new GameObject(_prefabName);

                GameObject _prefabObj = null;

                if (_isGenerateScene == false)
                {
                    _prefabObj = UnityEditor.PrefabUtility.SaveAsPrefabAsset(_generateObj, _fullPath);

                    GameObject.DestroyImmediate(_generateObj);
                }
                else
                {
                    _prefabObj = UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(_generateObj, _fullPath, UnityEditor.InteractionMode.AutomatedAction);
                }

                Component _prefabComponent = _prefabObj.AddComponent(_tempType);

                if (_isRefresh == true)
                    UnityEditor.AssetDatabase.Refresh();

                if (_isLog == true && _prefabComponent is null == false)
                    Debug_C.Log_Func("프리팹 생성) _fullPath : " + _fullPath);

                T _t = _prefabComponent as T;
                return _t;
            },
            AssetType.Prefab, _type, _aPath, _isRefresh, _isLog);
        }
        public static GenerateResult TryGetLoadWithGenerateAsset_Func<T>(
            string _assetName, out T _loadAsset, Func<Type, string, T> _creatDel, AssetType _assetType,
            Type _type = null, string _aPath = null, bool _isRefresh = true, bool _isLog = true) where T : UnityEngine.Object
        {
            _loadAsset = TryGetExtractResourcePath_Func(_aPath, out string _resourcesPath, _assetName) == true
                    ? Resources.Load<T>(_resourcesPath)
                    : null;

            if (_loadAsset == null)
            {
                if (_type == null)
                    _type = typeof(T);

                if (_aPath.IsNullOrWhiteSpace_Func() == true || _type == null)
                {
                    if (_isLog == true)
                        Debug_C.Log_Func("리소스 생성 자료가 없으므로 생성하지 않습니다.");

                    return GenerateResult.Fail_NoneData;
                }

                if (_type != null)
                {
                    if (_type.IsAbstract == false)
                    {
                        Editor_C.TryCheckOrGenerateFolder_Func(_aPath, _isLog);

                        string _extensionStr = GetTypeExtensionStr_Func(_assetType);
                        string _fullPath = StringBuilder_C.Append_Func(_aPath, SeparatorStr, _assetName, _extensionStr);

                        _loadAsset = _creatDel(_type, _fullPath);

                        return _loadAsset != null
                            ? GenerateResult.Success_Generate
                            : GenerateResult.Fail_Generate;
                    }
                    else
                        return GenerateResult.Fail_Abstract;
                }
                else
                {
                    if (_isLog == true)
                        Debug_C.Error_Func("리소스 생성 실패) 적절한 타입을 찾을 수 없습니다. _type : " + _type);

                    return GenerateResult.Fail_NullType;
                }
            }
            else
            {
                if (_isLog == true)
                    Debug_C.Log_Func("리소스 발견 : " + _loadAsset);

                return GenerateResult.Success_Find;
            }
        }

        public static bool TryGetExtractResourcePath_Func(string _path, out string _resourcesPath, string _resourceName = null)
        {
            _resourcesPath = null;

            if (_path.IsNullOrWhiteSpace_Func() == true || _path.Contains(Editor_C.SeparatorStr + CargoldLibrary_C.GetResourcesStr) == false)
                return false;

            string[] _pathArr = _path.Split(SeparatorChar);

            bool _isFind = false;
            bool _isFirst = false;
            foreach (string _pathStr in _pathArr)
            {
                if (_isFind == true)
                {
                    if (_isFirst == false)
                    {
                        _isFirst = true;
                    }
                    else
                    {
                        _resourcesPath = StringBuilder_C.Append_Func(_resourcesPath, SeparatorStr);
                    }

                    _resourcesPath = StringBuilder_C.Append_Func(_resourcesPath, _pathStr);
                    continue;
                }

                if (_pathStr.IsCompare_Func("Resources") == true)
                    _isFind = true;
            }

            if (_resourceName.IsNullOrWhiteSpace_Func() == false)
            {
                _resourcesPath = _resourcesPath.IsNullOrWhiteSpace_Func() == true
                    ? _resourceName
                    : StringBuilder_C.Append_Func(_resourcesPath, SeparatorStr, _resourceName);
            }

            return true;
        }

        public static void SetRenameAsset_Func(UnityEngine.Object _obj, string _renameStr, bool _isSave = true, bool _isLog = true)
        {
            string _path = Editor_C.GetPath_Func(_obj, _isLog);
            Editor_C.SetRenameAsset_Func(_path, _renameStr, _isLog);

            if(_isSave == true)
                Editor_C.SetSaveAsset_Func(_obj);
        }
        public static void SetRenameAsset_Func(string _path, string _renameStr, bool _isLog = true)
        {
            string _resultStr = UnityEditor.AssetDatabase.RenameAsset(_path, _renameStr);
            if (_resultStr.IsNullOrWhiteSpace_Func() == false)
                Debug_C.Error_Func(_resultStr);
            else if (_isLog == true)
                Debug_C.Log_Func("Rename : " + _renameStr);
        }
        public static void SetSaveAsset_Func(UnityEngine.Object _obj)
        {
            if(_obj == null)
            {
                Debug_C.Error_Func("빈 오브젝트입니다.");
                return;
            }

            UnityEditor.EditorUtility.SetDirty(_obj);

#if UNITY_2020_3_OR_NEWER
            UnityEditor.AssetDatabase.SaveAssetIfDirty(_obj);
#else
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
        public static void SetDefineSymbol_Func(UnityEditor.BuildTargetGroup _buildTargetGroup, List<string> _defineStrList)
        {
            SetDefineSymbol_Func(_buildTargetGroup, _defineStrList.ToArray());
        }
        public static void SetDefineSymbol_Func(UnityEditor.BuildTargetGroup _buildTargetGroup, params string[] _defineStrArr)
        {
#if UNITY_2020_1_OR_NEWER
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildTargetGroup, _defineStrArr);
#else
            string _defineStr = default;
            foreach (string _str in _defineStrArr)
                _defineStr = StringBuilder_C.Append_Func(_defineStr, _str, "; ");

            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildTargetGroup, _defineStr);
#endif
        }

        public static AnimationClip OnGenerateAnimationClip_Func(AnimationClip _clip, string _generatePathStr = null, bool _isLog = true)
        {
            if (_clip.empty == false)
            {
                string _extensionStr = Editor_C.GetTypeExtensionStr_Func(AssetType.AnimationClip);
                return Editor_C.OnGenerateAsset_Func(_clip, () => new AnimationClip(), _extensionStr, _generatePathStr, _isLog);
            }
            else
            {
                Debug_C.Log_Func("애니메이션 클립이 비어있으므로 복제는 취소되었습니다.");
                return null;
            }
        }
        public static GameObject OnGeneratePrefab_Func(GameObject _prefabObj, string _generatePathStr = null, bool _isLog = true, string _nameStr = null)
        {
            if(PrefabUtility.IsPartOfAnyPrefab(_prefabObj) == true)
            {
                string _extensionStr = Editor_C.GetTypeExtensionStr_Func(AssetType.Prefab);

                if (_nameStr.IsNullOrWhiteSpace_Func() == true)
                    _nameStr = StringBuilder_C.Append_Func("_", DateTime.Now.GetTotalSec_Func().ToString());

                return Editor_C.OnGenerateAsset_Func(_prefabObj, () => new GameObject(), _extensionStr, _generatePathStr, _isLog, true, _nameStr);
            }
            else
            {
                return null;
            }
        }
        public static T OnGenerateAsset_Func<T>(T _baseAseet, Func<T> _generateAssetDel, string _extensionStr
            , string _generatePathStr = null, bool _isLog = true, bool _isPrefab = false, string _nameStr = null) where T : UnityEngine.Object
        {
            T _generateAseet = null;

            if(_baseAseet != null)
            {
                if (_generatePathStr.IsNullOrWhiteSpace_Func() == true)
                {
                    _generatePathStr = AssetDatabase.GetAssetPath(_baseAseet);
                }
                else
                {
                    if (_nameStr.IsNullOrWhiteSpace_Func() == true)
                        _nameStr = _baseAseet.name;

                    _generatePathStr = Path.Combine(_generatePathStr, _nameStr);
                    _generatePathStr = StringBuilder_C.Append_Func(_generatePathStr, _extensionStr);
                }

                if (_isPrefab == false)
                {
                    _generatePathStr = AssetDatabase.GenerateUniqueAssetPath(_generatePathStr);

                    _generateAseet = _generateAssetDel();
                    EditorUtility.CopySerialized(_baseAseet, _generateAseet);
                    AssetDatabase.CreateAsset(_generateAseet, _generatePathStr);
                }
                else
                {
                    // 프리팹 에셋을 인스턴스하기
                    GameObject _prefabObj = PrefabUtility.InstantiatePrefab(_baseAseet) as GameObject;
                    _prefabObj.name = _nameStr;

                    // 인스턴스한 프리팹 객체를 언팩하기
                    Prefab_C.UnpackByInstance_Func(_prefabObj);

                    // 언팩된 인스턴스 객체를 프리팹 에셋으로 저장하기
                    GameObject _generateObj = PrefabUtility.SaveAsPrefabAsset(_prefabObj, _generatePathStr, out bool _isSuccess);
                    _generateAseet = _generateObj as T;

                    // 인퍽된 인스턴스 객체를 제거하기
                    GameObject.DestroyImmediate(_prefabObj);
                }

                if(_isLog == true)
                    Debug_C.Log_Func(_nameStr + "을 생성했습니다.");
            }
            else
            {
                Debug_C.Log_Func("생성하고자 하는 원본 에셋이 비어있으므로 에셋 생성이 취소되었습니다.");
            }

            return _generateAseet;
        }

        private static TextEditor textEditor = new TextEditor();
        /// <summary>
        /// Ctrl + C
        /// </summary>
        /// <param name="_str">복사할 문자열</param>
        /// <param name="_isLog"></param>
        /// 
        public static void SetClipboard_Func(string _str, bool _isLog = true)
        {
            if (textEditor == null)
                textEditor = new TextEditor();

            textEditor.text = _str;
            textEditor.OnFocus();
            textEditor.Copy();

            if(_isLog == true)
                Debug.Log("복사 : " + _str);
        }


        /// <summary>
        /// Project Window에 특정 에셋을 선택합니다.
        /// </summary>
        /// <param name="_path">선택할 에셋의 경로</param>
        public static void SetSelection_Func(string _path)
        {
            UnityEngine.Object _obj = Editor_C.GetLoadAssetAtPath_Func(_path, false);
            Editor_C.SetSelection_Func(_obj, true);
        }
        /// <summary>
        /// Project Window에 특정 에셋을 선택합니다.
        /// </summary>
        /// <param name="_obj">선택할 에셋</param>
        public static void SetSelection_Func(UnityEngine.Object _obj, bool _isBaseObj = false)
        {
            if(_isBaseObj == false)
            {
                string _path = Editor_C.GetPath_Func(_obj);
                _obj = Editor_C.GetLoadAssetAtPath_Func(_path, false);
            }

            UnityEditor.Selection.activeObject = _obj;
        }

        public static void DeleteAsset_Func(UnityEngine.Object _obj, bool _isLog = true)
        {
            string _pathStr = Editor_C.GetPath_Func(_obj);
            DeleteAsset_Func(_pathStr, _isLog);
        }
        public static void DeleteAsset_Func(string _pathStr, bool _isLog = true)
        {
            if (UnityEditor.AssetDatabase.DeleteAsset(_pathStr) == false)
            {
                if (_isLog == true)
                    Debug_C.Error_Func("제거 실패!");
            }
        }

        /// <summary>
        /// 윈도우 폴더 열기
        /// </summary>
        /// <param name="_path">윈도우 폴더 경로</param>
        public static void OpenWindowFolder_Func(string _path)
        {
            Application.OpenURL(_path);
        }

        /// <summary>
        /// 에디터 코루틴
        /// </summary>
        /// <param name="_enumerator"></param>
        public static void StartCoroutine_Func(System.Collections.IEnumerator _enumerator)
        {
            Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(_enumerator, LibraryRemocon.Instance);
        }

        /// <summary>
        /// 강제로 컴파일 시도하기
        /// </summary>
        public static void OnCompile_Func()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        public static ReferenceState GetReferenceState_Func(UnityEngine.Object _obj)
        {
            if(_obj != null)
            {
                return ReferenceState.Reference;
            }
            else
            {
                if (UnityEngine.Object.ReferenceEquals(_obj, null) == false)
                {
                    return ReferenceState.Missing;
                }
                else
                {
                    return ReferenceState.Empty;
                }
            }
        }
#endif
        /// <summary>
        /// 문자열 배열 사이에 / 문자 붙여주기
        /// </summary>
        /// <param name="_strArr"></param>
        /// <returns></returns>
        public static string GetPath_Func(params string[] _strArr)
        {
            return StringBuilder_C.GetPath_Func(SeparatorStr, _strArr);
        }

        [Flags]
        public enum GenerateResult
        {
            None = 0,

            Success_Find = 1 << 1,
            Success_Generate = 1 << 2,

            Success = Success_Find | Success_Generate,

            Fail_NullType = 1 << 3,
            Fail_NoneData = 1 << 4,
            Fail_Generate = 1 << 5,
            Fail_Exist = 1 << 6,
            Fail_NoneFolder = 1 << 7,
            Fail_Abstract = 1 << 8,

            Fail = Fail_NoneData | Fail_NullType | Fail_Generate | Fail_Exist | Fail_NoneFolder | Fail_Abstract
        }

        public enum AssetType
        {
            None = 0,

            ScriptableObject,
            Prefab,
            Script,
            AnimationClip,
        }

        public enum ReferenceState
        {
            None = 0,

            /// <summary>
            /// 참조되어 있지 않음
            /// </summary>
            Empty = 10,

            /// <summary>
            /// 참조되었으나 현재는 알 수 없음
            /// </summary>
            Missing = 20,

            /// <summary>
            /// 참조 상태
            /// </summary>
            Reference = 30,
        }
    }
}