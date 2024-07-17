using System;
using Cargold;
using Sirenix.OdinInspector;
using UniRx;

namespace _2_Scripts.Utils.Components
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIImageAnimation : SerializedMonoBehaviour
    {

        [SerializeField] private Dictionary<string, Sprite[]> mAnimDic;
        private Image mImage;

        private void Start()
        {
            mImage = GetComponent<Image>();
        }

        public void PlayAnimation(string key, float speed,Action onCompleteCallback = null)
        {
            if(!mAnimDic.ContainsKey(key))
            {
                Debug.LogError("Not Found Animation Key!");
            }

            int idx = 0;
            Observable.Interval(System.TimeSpan.FromSeconds(speed))
                .TakeWhile(_=> idx < mAnimDic[key].Length)
                .Subscribe(_ =>
                    {
                        mImage.sprite = mAnimDic[key][idx++];
                    }, 
                    () =>
                    {
                        idx = 0;
                        onCompleteCallback?.Invoke();
                    })
                .AddTo(this);
        }

    }
}