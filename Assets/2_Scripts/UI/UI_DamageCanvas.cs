using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_DamageCanvas : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI mText;
        //private CancellationTokenSource mCts;

        private static Color mClear = new Color(1, 1, 1, 0);

        void Awake()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
            //mCts = new CancellationTokenSource();
        }

        public void SetDamage(float damage)
        {
            gameObject.SetActive(true);
            mText.text = $"-{damage}";
            FadeAsync().Forget();
        }

        private async UniTask FadeAsync(float time = 0.5f)
        {
            float originTime = time;
            while (time >= 0)
            {
                await UniTask.DelayFrame(1);
                time -= Time.deltaTime;
                mText.color = Color.Lerp(mClear, Color.white, time / originTime);
                transform.position += Vector3.up * Time.deltaTime;
            }

            Clear();
        }

        private void Clear()
        {
            gameObject.SetActive(false);
            mText.color = Color.white;
        }
    }
}