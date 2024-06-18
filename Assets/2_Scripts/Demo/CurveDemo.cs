using System;
using System.Collections;
using Cargold;
using Cargold.FrameWork;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _2_Scripts.Demo
{
    public class CurveDemo : MonoBehaviour
    {
        private void Start()
        {
            WaitLoad().Forget();
        }

        private async UniTaskVoid WaitLoad()
        {
            await UniTask.WaitUntil(() => ResourceManager.Instance.IsPreLoad);
            StartCoroutine(curvedTest());
        }

        private WaitForSeconds _waitSecond = new WaitForSeconds(3f);
        IEnumerator curvedTest()
        {
            while (true)
            {
                CurveSystemManager.Instance.AddGoldCurve(Vector2.left,100);
                
                yield return _waitSecond;
            }

            yield return null;
        }
    }
}