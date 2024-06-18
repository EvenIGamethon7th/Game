using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cargold.PoolingSystem
{
    public class PoolingContainer { }

    [System.Serializable]
    public class PoolingContainer<PoolerComponent> : PoolingContainer, IDisposable where PoolerComponent : Component, IPooler
    {
        private Transform groupTrf;
        private GameObject basePoolerObj;
        private Queue<PoolerComponent> poolQueue;
        private int loadCount;
        private bool isSetParent;
        private bool isDisposed;
        private Dictionary<PoolerComponent, int> poolerToIdDic;

        public bool HasObject { get { return 0 < this.poolQueue.Count ? true : false; } }
        public int LoadPoolerNum { get { return this.poolQueue.Count; } }
        public int TotalLoadPoolerNum => this.loadCount;

        public PoolingContainer(Transform _groupTrf, GameObject _basePoolerObj, int _loadCount, bool _isSetParent = true)
        {
            this.groupTrf = _groupTrf;
            this.basePoolerObj = _basePoolerObj;
            this.poolQueue = new Queue<PoolerComponent>();
            this.loadCount = 0;
            this.isSetParent = _isSetParent;
            this.isDisposed = false;
            this.poolerToIdDic = new Dictionary<PoolerComponent, int>();

            for (int i = 0; i < _loadCount; i++)
            {
                PoolerComponent _poolingComponent = this.GenerateComponent(_isSetParent);
                this.poolQueue.Enqueue(_poolingComponent);
            }
        }
        ~PoolingContainer()
        {
            this.Dispose();
        }

        private PoolerComponent GenerateComponent(bool _isSetParent = true)
        {
            GameObject _obj = GameObject.Instantiate(this.basePoolerObj);

            int _poolerID = this.loadCount;

#if UNITY_EDITOR
            _obj.name = StringBuilder_C.Append_Func(this.basePoolerObj.name, "_", this.loadCount.ToString());
#endif

            if (_isSetParent == true && this.groupTrf != null)
                _obj.transform.SetParent(this.groupTrf);
            ++this.loadCount;

            PoolerComponent _poolingComponent = _obj.GetComponent<PoolerComponent>();
            if (_poolingComponent != null)
            {
                this.poolerToIdDic.Add_Func(_poolingComponent, _poolerID);

                _poolingComponent.InitializedByPoolingSystem();
            }
            else
            {
                Debug.LogError("풀링) 다음 컴포넌트를 생성하지 못했습니다. : " + _poolingComponent);
            }

            return _poolingComponent;
        }

        public PoolerComponent Spawn()
        {
            if(0 < this.poolQueue.Count)
            {
                PoolerComponent _pooler = poolQueue.Dequeue();

                return _pooler;
            }
            else
            {
                return this.GenerateComponent(this.isSetParent);
            }
        }
        public void Despawn(PoolerComponent _poolingComponent)
        {
            this.Despawn(_poolingComponent, this.isSetParent);
        }
        public void Despawn(PoolerComponent _poolerComponent, bool _isSetParentTrf)
        {
            if (this.poolQueue.Contains(_poolerComponent) == false)
            {
                this.poolQueue.Enqueue(_poolerComponent);

                if (_isSetParentTrf == true)
                    _poolerComponent.transform.SetParent(this.groupTrf);
            }
            else
            {
                Debug.LogWarning("이미 저장되어 있는 Item을 ObjectPooling에 저장하려고 합니다. : " + _poolerComponent);
            }
        }

        public bool TryGetPoolerID_Func(PoolerComponent _poolerComponent, out int _poolerID)
        {
            return this.poolerToIdDic.TryGetValue(_poolerComponent, out _poolerID);
        }

        public void Dispose()
        {
            if(this.isDisposed == false)
            {
                this.isDisposed = true;

                while (0 < this.poolQueue.Count)
                {
                    PoolerComponent _pooler = poolQueue.Dequeue();
                    try
                    {
                        Component.Destroy(_pooler);
                    }
                    catch(Exception e)
                    {
                        Debug_C.Warning_Func(e);
                    }
                    
                }

                this.groupTrf = null;
                this.basePoolerObj = null;
                this.poolQueue = null;
                this.loadCount = 0;
                this.isSetParent = false;

                System.GC.SuppressFinalize(this);
            }
        }
    }

    public interface IPooler
    {
        void InitializedByPoolingSystem();
    }
}