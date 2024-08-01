
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using _2_Scripts.Utils;
    using Cargold;
    using Sirenix.OdinInspector;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;

public enum ELabelNames
{
    Default,
    Demo,
    Material,
    SkeletonData,
    CharacterData,
    UIMaterial,
    MainCharacterData,
    Map,
    MainCharacterObject,
    Sound,
    CutScene
}

public class ResourceManager :Singleton<ResourceManager>
{
    public Dictionary<string, Object> _resources { get; private set; } = new();
    public bool IsPreLoad {get; private set; }
    
    public T Load<T>(string key ,Action<T> callback = null) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return resource as T;
        }

        if (typeof(T) == typeof(Sprite))
        {
            key = key + ".sprite";
            if (_resources.TryGetValue(key, out Object temp))
            { 
                callback?.Invoke(temp as T);
                return temp as T;
            }
        }

        return null;
    }
    
   
    #region 어드레서블
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        if (key.Contains(".multiplesprite"))
        {
            //멀티 스프라이트인 경우 하위객체를 배열 형태의 키값으로 추가시킴
            string parentKey = $"{key.Replace(".multiplesprite", "")}";
            var asyncOperation = Addressables.LoadAssetAsync<IList<T>>(key);
            asyncOperation.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    IList<T> sprites = op.Result;
                    for(int i=0; i< sprites.Count; i++)
                    {
                        string keyName = $"{parentKey}[{i}]";
                        // 캐시 확인.
                        if (_resources.TryGetValue(keyName, out Object resource))
                        {
                            callback?.Invoke(sprites[i]);
                            return;
                        }

                        _resources.Add(keyName, sprites[i]);
                        
                    }
                    callback?.Invoke(sprites[sprites.Count-1]);
                }
            };
        }
        else
        {
            //스프라이트인 경우 하위객체의 찐이름으로 로드하면 스프라이트로 로딩이 됌
            string loadKey = key;
            if (typeof(T) == typeof(Sprite))
            {
                // 폴더로 등록할 경우 스프라이트 등록 처리 포함
                int startIdx = key.IndexOf('/') + 1;
                string fileName = key.Substring(startIdx, key.IndexOf('.')-startIdx);
                string ext = key.Substring(key.IndexOf('.'));
                loadKey = $"{key}[{fileName}]";
            }
            var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
            asyncOperation.Completed += (op) =>
            {
                string keyName = key;
                int folderIndex = key.IndexOf('/');
                if(folderIndex > 0)
                {
                    folderIndex += 1;
                    keyName = key.Substring(folderIndex, key.IndexOf('.') - folderIndex);
                }
                 

                // 캐시 확인.
                if (_resources.TryGetValue(keyName, out Object resource))
                {
                    callback?.Invoke(op.Result);
                    return;
                }

                _resources.Add(keyName, op.Result);
                callback?.Invoke(op.Result);
            };
        }
       


    }

    public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        
        opHandle.Completed += (op) =>
        {
            int loadCount = 0;

            int totalCount = op.Result.Count;

            string[] spriteObjs = { ".sprite", ".multiplesprite", ".png", ".jpg" };


            foreach (var result in op.Result)
            {
                if (spriteObjs.Any(substring => result.PrimaryKey.Contains(substring)))
                {
                    LoadAsync<Sprite>(result.PrimaryKey, (obj) =>
                    {
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
                else
                {
                    LoadAsync<T>(result.PrimaryKey, (obj) =>
                    {
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
            }
        };
    }
    #endregion

    protected override void ChangeSceneInit(Scene prev, Scene next)
    {
     
    }

    protected override void AwakeInit()
    {
        string[] labelNames = Enum.GetNames(typeof(ELabelNames));
        for (int i = 0; i < labelNames.Length; ++i)
        {
            string name = labelNames[i];
            LoadAllAsync<Object>(name, (key, count, totalCount) =>
            {
                if (count == totalCount)
                {
                    IsPreLoad = true;
                    Debug_C.Log_Func($"{name} Label Resource Load Completed");
                    Debug.Log($"{name} Label Resource Load Completed");
                    ETaskList e = (ETaskList)Array.IndexOf(labelNames, name);
                    TaskMessage message = new(e);
                    MessageBroker.Default.Publish(message);
                }
            });
        }
    }
}
