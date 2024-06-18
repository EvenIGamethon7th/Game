using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _2_Scripts.Demo
{
    public class AddressablePoolDemo : MonoBehaviour
    {
        private void Start()
        {
            WaitLoad().Forget();
        }

        private async UniTaskVoid WaitLoad()
        {
            await UniTask.WaitUntil(() => ResourceManager.Instance.IsPreLoad);
            ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Demo_Cube,Vector2.zero);
        }
    }
}