using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
#if UNITY_EDITOR
using UnityEditor;

namespace Cargold
{
    public static class Prefab_C
    {
        public static bool IsPrefab_Func(Object _obj, bool _isLog = true)
        {
            if(_obj == null)
            {
                if(_isLog == true)
                    Debug_C.Error_Func("매개변수가 비어있습니다.");

                return false;
            }

            if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(_obj) == true)
            {
                if(_isLog == true)
                    Debug_C.Log_Func(_obj.name + "은 프리팹이 맞습니당.");

                return true;
            }
            else
            {
                if (_isLog == true)
                    Debug_C.Log_Func(_obj.name + "은 프리팹이 아닌데?");

                return false;
            }
        }

        public static void Apply_Func(GameObject _gobj, bool _isCheck = true, bool _isLog = true)
        {
            if(_isCheck == true && IsPrefab_Func(_gobj, _isLog) == false)
                return;

            UnityEditor.PrefabUtility.ApplyPrefabInstance(_gobj, UnityEditor.InteractionMode.AutomatedAction);

            if(_isLog == true)
                Debug_C.Log_Func(_gobj.name + " 프리팹 Apply");
        }
        public static void UnpackByInstance_Func(GameObject _instanceObj, UnityEditor.PrefabUnpackMode _prefabUnpackMode = PrefabUnpackMode.Completely)
        {
            UnityEditor.PrefabUtility.UnpackPrefabInstance(_instanceObj, _prefabUnpackMode, UnityEditor.InteractionMode.AutomatedAction);
        }
        public static void UnpackByAsset_Func(GameObject _assetObj, UnityEditor.PrefabUnpackMode _prefabUnpackMode = PrefabUnpackMode.Completely)
        {
            UnityEditor.PrefabUtility.UnpackAllInstancesOfPrefab(_assetObj, _prefabUnpackMode, UnityEditor.InteractionMode.AutomatedAction);
        }

        public static Object Instantiate_Func(Object _obj, Transform _parentTrf = null, bool _isLog = true)
        {
            Object _instanceObj = PrefabUtility.InstantiatePrefab(_obj, _parentTrf);
            if (_instanceObj == null)
            {
                Debug_C.Error_Func("프리팹 인스턴스 실패");
                return null;
            }

            return _instanceObj;
        }
        public static T Instantiate_Func<T>(Object _obj, Transform _parentTrf = null, bool _isLog = true) where T : Object
        {
            Object _instanceObj = Instantiate_Func(_obj, _parentTrf, _isLog);

            T _instanceComponent = (_instanceObj as GameObject).GetComponent<T>();
            if(_instanceComponent == null)
            {
                Debug_C.Error_Func("프리팹 인스턴스 겟컴포넌트 실패");
                return null;
            }

            return _instanceComponent;
        }

        public static GameObject CreateOverridePrefabAsset_Func<T>(Object _baseAsset, string _newName
            , string _createPath = null, Transform _parentTrf = null, bool _isLog = true) where T : Component
        {
            string _baseAssetPath = Editor_C.GetPath_Func(_baseAsset);
            return CreateOverridePrefabAsset_Func<T>(_baseAssetPath, _newName, out _, _createPath, _parentTrf, _isLog);
        }
        public static GameObject CreateOverridePrefabAsset_Func<T>(Object _baseAsset, string _newName, out T _instance
            , string _createPath = null, Transform _parentTrf = null, bool _isLog = true) where T : Component
        {
            string _baseAssetPath = Editor_C.GetPath_Func(_baseAsset);
            return CreateOverridePrefabAsset_Func<T>(_baseAssetPath, _newName, out _instance, _createPath, _parentTrf, _isLog);
        }
        public static GameObject CreateOverridePrefabAsset_Func<T>(string _baseAssetPath, string _newName, out T _instance
            , string _createPath = null, Transform _parentTrf = null, bool _isLog = true) where T : Component
        {
            _instance = null;

            // 원본 프리팹 불러오기
            T _baseAsset = Editor_C.GetLoadAssetAtPath_Func<T>(_baseAssetPath);
            if (_baseAsset == null)
            {
                Debug_C.Error_Func("다음 경로의 프리팹 원본 에셋을 찾지 못했습니다. : " + _baseAssetPath);
                return null;
            }

            // 원본 프리팹 인스턴스
            T _cloneComponet = Prefab_C.Instantiate_Func(_baseAsset, _isLog: _isLog) as T;
            if (_cloneComponet == null)
                return null;

            if (_createPath.IsNullOrWhiteSpace_Func() == true)
            {
                string[] _pathArr = _baseAssetPath.Split(Editor_C.SeparatorStr);

                for (int i = 0; i < _pathArr.Length - 1; i++)
                    _createPath = StringBuilder_C.Append_Func(_createPath, _pathArr[i], Editor_C.SeparatorStr);

                _createPath = StringBuilder_C.Append_Func(_createPath, _newName, ".prefab");
            }

            // 오버라이드 프리팹 에셋 생성
            GameObject _overridePrefab = UnityEditor.PrefabUtility.SaveAsPrefabAsset(_cloneComponet.gameObject, _createPath, out bool _isSuccess);
            if(_overridePrefab == null || _isSuccess == false)
            {
                Debug_C.Error_Func("다음 경로로 오버라이드 프리팹을 생성하지 못했습니다. : " + _createPath);
                return null;
            }

            // 원본 프리팹 인스턴스 제거
            GameObject.DestroyImmediate(_cloneComponet.gameObject, true);

            // 오버라이드 프리팹 인스턴스
            _instance = Prefab_C.Instantiate_Func<T>(_overridePrefab, _parentTrf, _isLog);

            AssetDatabase.Refresh();

            if(_isLog == true)
                Debug_C.Log_Func("오버라이드 프리팹 생성 성공! 경로 : " + _createPath);

            return _overridePrefab;
        }

        public static GameObject CreatePrefabAsset_Func(GameObject _rootObj, string _aPath, string _prefabName)
        {
            return CreatePrefabAsset_Func(_rootObj, _aPath, _prefabName, out _);
        }
        public static GameObject CreatePrefabAsset_Func(GameObject _rootObj, string _aPath, string _prefabName, out bool _isSuccess)
        {
            if(Editor_C.TryCheckOrGenerateFolder_Func(_aPath) == true)
            {
                string _fPath = StringBuilder_C.Append_Func(_aPath, Editor_C.SeparatorStr, _prefabName, ".prefab");
                return PrefabUtility.SaveAsPrefabAsset(_rootObj, _fPath, out _isSuccess);
            }
            else
            {
                _isSuccess = false;

                return null;
            }
        }

        public static string GetAssetPath_Func(Object _obj)
        {
            return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(_obj);
        }
        public static T GetAsset_Func<T>(Object _instance) where T : Object
        {
            string _path = Prefab_C.GetAssetPath_Func(_instance);
            return Editor_C.GetLoadAssetAtPath_Func<T>(_path);
        }
        public static T GetOriginalSource_Func<T>(T _target) where T : Object
        {
            // 타겟이...
            // 오버라이드 프리팹 에셋일 때 -> 오리지날 프리팹 에셋
            // 오버라이드 프리팹 인스턴스일 때 ->  -> 오리지날 프리팹 에셋

            return PrefabUtility.GetCorrespondingObjectFromOriginalSource(_target);
        }
        public static T GetSource_Func<T>(T _target) where T : Object
        {
            // 타겟이...
            // 오버라이드 프리팹 에셋일 때 -> 오리지날 프리팹 에셋
            // 오버라이드 프리팹 인스턴스일 때 -> 인스턴스된 에셋

            return PrefabUtility.GetCorrespondingObjectFromSource(_target);
        }

        public static void DeleteAsset_Func(Object _obj, bool _isLog = true)
        {
            string _path = GetAssetPath_Func(_obj);
            Editor_C.DeleteAsset_Func(_path, _isLog);
        }
    }
}
#endif