using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold.PoolingSystem;
using Cargold.Effect;

namespace Cargold.FrameWork
{
    // 클래스 풀링도 만들자
    public class PoolingSystem_Manager : SerializedMonoBehaviour, GameSystem_Manager.IInitializer // C
    {
        public static PoolingSystem_Manager Instance;

        [ShowInInspector, ReadOnly] private PoolingSystemManager<string> innerPoolManager;

        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr)] [SerializeField] private BasePoolingData<Cargold.UI.UI_BaseToast_Script> baseToastData =
            new BasePoolingData<Cargold.UI.UI_BaseToast_Script>();
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr)] [SerializeField] private BasePoolingData_EffectKey<Cargold.Effect.Effect_Script>[] effectDataArr = null;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                if (Instance == null)
                    Instance = this;

                this.innerPoolManager = new PoolingSystemManager<string>(this.transform);

                this.TryGeneratePool_Func(PoolingKey.ToastElem, this.baseToastData);

                foreach (BasePoolingData_EffectKey<Cargold.Effect.Effect_Script> _effectData in this.effectDataArr)
                    this.TryGeneratePool_Func(_effectData.basePooler.GetPoolingKey, _effectData);
            }
        }

        public T Spawn_Func<T>(string _poolingKey, Transform _parentTrf = null, float _scale = 1f) where T : Component, IPooler
        {
            T _pooler = this.innerPoolManager.Spawn<T>(_poolingKey);

            if (_parentTrf != null)
            {
                _pooler.transform.SetParent(_parentTrf);
                _pooler.transform.localPosition = Vector3.zero;
            }

            _pooler.transform.rotation = Quaternion.identity;
            _pooler.transform.localScale = Vector3.one * _scale;

            return _pooler;
        }

        public void Despawn_Func<T>(string _poolingKey, T _componentType, bool _isSetParent = true) where T : Component, IPooler
        {
            this.innerPoolManager.Despawn_Func(_poolingKey, _componentType, _isSetParent);
        }

        public bool TryGeneratePool_Func<ComponentType>(string _poolingKey, BasePoolingData<ComponentType> _basePoolingData) where ComponentType : Component, IPooler
        {
            GameObject _poolerObj = _basePoolingData.basePooler?.gameObject;

            Transform _poolGroupTrf = _basePoolingData.poolGroupTrf;
            bool _isSetParent = _poolGroupTrf != null;
            return this.innerPoolManager.TryGeneratePool<ComponentType>(_poolingKey, _poolerObj, _basePoolingData.loadCount, _isSetParent, _poolGroupTrf);
        }
        public bool TryGeneratePool_Func<ComponentType>(string _poolingKey, GameObject _poolerObj
            , int _preloadNum = 0, bool _isSetParent = true, Transform _groupTrf = null) where ComponentType : Component, IPooler
        {
            return this.innerPoolManager.TryGeneratePool<ComponentType>(_poolingKey, _poolerObj, _preloadNum, _isSetParent, _groupTrf);
        }
        public bool TryGetPoolerID_Func<ComponentType>(string _poolingKey, ComponentType _poolerComponent, out int _poolerID) where ComponentType : Component, IPooler
        {
            return this.innerPoolManager.TryGetPoolerID_Func<ComponentType>(_poolingKey, _poolerComponent, out _poolerID);
        }

        public bool IsGeneratedPool(string _poolingKey)
        {
            return this.innerPoolManager.IsGeneratedPool(_poolingKey);
        }
        public int GetTotalLoadedPoolerNum_Func<T>(string _poolingKey) where T : Component, IPooler
        {
            return this.innerPoolManager.GetTotalLoadedPoolerNum_Func<T>(_poolingKey);
        }

#if UNITY_EDITOR
        public void CallEdit_SetToast_Func(Cargold.UI.UI_BaseToast_Script _toast)
        {
            this.baseToastData.basePooler = _toast;
        }
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
        }

        [FoldoutGroup("카라리 이펙트"), Button(CargoldLibrary_C.CatchingStr)]
        private void CallEdit_EffectAdd_Func(Effect_Script[] _effectClassArr)
        {
            if(_effectClassArr.IsHave_Func() == false)
            {
                Debug_C.Error_Func("?");

                return;
            }

            List<BasePoolingData_EffectKey<Effect_Script>> _list = new List<BasePoolingData_EffectKey<Effect_Script>>();

            if (this.effectDataArr.IsHave_Func() == true)
                _list.AddRange(_list);

            for (int i = 0; i < _effectClassArr.Length; i++)
            {
                BasePoolingData_EffectKey<Effect_Script> _basePoolingData = new BasePoolingData_EffectKey<Effect_Script>();
                _basePoolingData.basePooler = _effectClassArr[i];

                _list.Add(_basePoolingData);
            }

            this.effectDataArr = _list.ToArray();
        }
#endif
    }

    [System.Serializable]
    public class BasePoolingData<PoolerType> : PoolingSystemManager<string>.BasePoolingData<PoolerType> where PoolerType : Component, IPooler { }

    [System.Serializable]
    public class BasePoolingData_EffectKey<PoolerType> : BasePoolingData<PoolerType> where PoolerType : Cargold.Effect.Effect_Script { }
}

public partial class PoolingKey
{
    public const string ToastElem = "ToastElem";
}