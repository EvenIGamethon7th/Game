using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

// 0.1.3 ('20.05.02)
// Developed By Cargold
namespace Cargold.PoolingSystem
{
    public class PoolingSystemManager<PoolingKey>
    {
        private static System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        private const string PrefixWord = "(PoolerGroup) ";

        protected Transform mainPoolGroupTrf;
        [ShowInInspector, ReadOnly] protected Dictionary<PoolingKey, PoolingContainer> poolingContainerDic;

        public PoolingSystemManager(Transform _mainPoolGroupTrf)
        {
            this.mainPoolGroupTrf = _mainPoolGroupTrf;

            this.poolingContainerDic = new Dictionary<PoolingKey, PoolingContainer>();
        }
        public PoolingSystemManager(Transform _mainPoolGroupTrf, IEqualityComparer<PoolingKey> _iEqualityComparer)
        {
            this.mainPoolGroupTrf = _mainPoolGroupTrf;

            this.poolingContainerDic = new Dictionary<PoolingKey, PoolingContainer>(_iEqualityComparer);
        }

        public bool TryGeneratePool<ComponentType>(PoolingKey _poolingKey, GameObject _basePoolerObj, int _loadCount, bool _isSetParent = true, Transform _basePoolGroupTrf = null)
            where ComponentType : Component, IPooler
        {
            if (this.poolingContainerDic.ContainsKey(_poolingKey) == false)
            {
                string _poolingName = PoolingSystemManager<PoolingKey>.PrefixWord;

#if UNITY_EDITOR
                stringBuilder.Clear();
                stringBuilder.Append(PrefixWord);
                stringBuilder.Append(_poolingKey.ToString());
                _poolingName = stringBuilder.ToString();
#endif

                Transform _poolGroupTrf = null;
                if(_basePoolGroupTrf == null)
                {
                    _poolGroupTrf = new GameObject(_poolingName).transform;
                    _poolGroupTrf.SetParent(mainPoolGroupTrf);
                }
                else
                {
                    _poolGroupTrf = _basePoolGroupTrf;
                }
                
                PoolingContainer<ComponentType> _objectPoolingChild = new PoolingContainer<ComponentType>(_poolGroupTrf, _basePoolerObj, _loadCount, _isSetParent);
                this.poolingContainerDic.Add(_poolingKey, _objectPoolingChild);

                return true;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("다음 Key는 이미 존재합니다. : " + _poolingKey);
#endif

                return false;
            }
        }
        public void GeneratePool<ComponentType>(PoolingKey poolingKey, GameObject basePoolerObj, int loadCount, bool isSetParent = true, Transform poolGroupTrf = null) where ComponentType : Component, IPooler
        {
            this.TryGeneratePool<ComponentType>(poolingKey, basePoolerObj, loadCount, isSetParent, poolGroupTrf);
        }

        public bool TryDestroyPool<ComponentType>(PoolingKey poolingKey) where ComponentType : Component, IPooler
        {
            PoolingContainer _poolingSystem = null;
            if (poolingContainerDic.TryGetValue(poolingKey, out _poolingSystem) == true)
            {
                PoolingContainer<ComponentType> _objectPoolingChild = (PoolingContainer<ComponentType>)_poolingSystem;

                _objectPoolingChild.Dispose();

                poolingContainerDic.Remove(poolingKey);

                return true;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("다음 Key는 존재하지 않습니다. : " + poolingKey);
#endif
            }

            return false;
        }
        public void DestroyPool<ComponentType>(PoolingKey poolingKey) where ComponentType : Component, IPooler
        {
            this.TryDestroyPool<ComponentType>(poolingKey);
        }

        public ComponentType Spawn<ComponentType>(PoolingKey poolingKey) where ComponentType : Component, IPooler
        {
            PoolingContainer _poolingSystem = null;
            if (poolingContainerDic.TryGetValue(poolingKey, out _poolingSystem) == true)
            {
                if(_poolingSystem is PoolingContainer<ComponentType>)
                {
                    PoolingContainer<ComponentType> _objectPoolingChild = (PoolingContainer<ComponentType>)_poolingSystem;

                    return _objectPoolingChild.Spawn();
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("다음 Key의 캐스팅은 실패했습니다. : " + poolingKey);
#endif

                    return null;
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("다음 Key는 존재하지 않습니다. : " + poolingKey);
#endif

                return null;
            }
        }
        public void Despawn<ComponentType>(PoolingKey poolingKey, ComponentType returnComponent) where ComponentType : Component, IPooler
        {
            PoolingContainer _poolingSystem = null;
            if (poolingContainerDic.TryGetValue(poolingKey, out _poolingSystem) == true)
            {
                if (_poolingSystem is PoolingContainer<ComponentType>)
                {
                    PoolingContainer<ComponentType> _objectPoolingChild = (PoolingContainer<ComponentType>)_poolingSystem;

                    _objectPoolingChild.Despawn(returnComponent);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("다음 Key의 캐스팅은 실패했습니다. : " + poolingKey);
#endif
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("다음 Key는 존재하지 않습니다. : " + poolingKey);
#endif
            }
        }
        public void Despawn_Func<ComponentType>(PoolingKey _poolingKey, ComponentType _returnComponent, bool _isSetParent) where ComponentType : Component, IPooler
        {
            PoolingContainer _poolingSystem = null;
            if (poolingContainerDic.TryGetValue(_poolingKey, out _poolingSystem) == true)
            {
                PoolingContainer<ComponentType> _objectPoolingChild = (PoolingContainer<ComponentType>)_poolingSystem;

                _objectPoolingChild.Despawn(_returnComponent, _isSetParent);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("다음 Key는 존재하지 않습니다. : " + _poolingKey);
#endif
            }
        }

        public int GetLoadNum<ComponentType>(PoolingKey poolingKey) where ComponentType : Component, IPooler
        {
            PoolingContainer _poolingSystem = null;
            if (poolingContainerDic.TryGetValue(poolingKey, out _poolingSystem) == true)
            {
                PoolingContainer<ComponentType> _poolSystem = (PoolingContainer<ComponentType>)_poolingSystem;

                return _poolSystem.LoadPoolerNum;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("다음 Key는 존재하지 않습니다. : " + poolingKey);
#endif

                return -1;
            }
        }
        public int GetTotalLoadedPoolerNum_Func<ComponentType>(PoolingKey poolingKey) where ComponentType : Component, IPooler
        {
            PoolingContainer _poolingSystem = null;
            if (poolingContainerDic.TryGetValue(poolingKey, out _poolingSystem) == true)
            {
                PoolingContainer<ComponentType> _poolSystem = (PoolingContainer<ComponentType>)_poolingSystem;

                return _poolSystem.TotalLoadPoolerNum;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("다음 Key는 존재하지 않습니다. : " + poolingKey);
#endif

                return -1;
            }
        }
        public bool IsGeneratedPool(PoolingKey _poolingKey)
        {
            return this.poolingContainerDic.ContainsKey(_poolingKey);
        }
        public bool IsGeneratedPool<ComponentType>(PoolingKey _poolingKey) where ComponentType : Component, IPooler
        {
            PoolingContainer _poolingSystem = null;
            if (poolingContainerDic.TryGetValue(_poolingKey, out _poolingSystem) == true)
            {
                return _poolingSystem is PoolingContainer<ComponentType>;
            }
            else
            {
                return false;
            }
        }
        public bool TryGetPoolerID_Func<ComponentType>(PoolingKey _poolingKey, ComponentType _poolerComponent, out int _poolerID) where ComponentType : Component, IPooler
        {
            PoolingContainer _poolingSystem = null;
            if (poolingContainerDic.TryGetValue(_poolingKey, out _poolingSystem) == true)
            {
                PoolingContainer<ComponentType> _poolSystem = (PoolingContainer<ComponentType>)_poolingSystem;

                return _poolSystem.TryGetPoolerID_Func(_poolerComponent, out _poolerID);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("다음 Key는 존재하지 않습니다. : " + _poolingKey);
#endif
                _poolerID = -1;
                return false;
            }
        }

        [System.Serializable]
        public class BasePoolingData<PoolerType> where PoolerType : Component, IPooler
        {
            [LabelText("풀러")] public PoolerType basePooler = null;
            [LabelText("기 생성 개수")] public int loadCount = 0;
            [LabelText("풀러 그룹 Trf")] public Transform poolGroupTrf = null;
        }
    }
}