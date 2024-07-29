using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_DamageCanvas : UI_Base
    {
        [SerializeField]
        private TextMeshProUGUI mText;
        private CancellationTokenSource mCts;

        private static Color mClear = new Color(1, 1, 1, 0);
        private static Color mTopMinDamage = new Color(0, 0.3658f, 1, 1);
        private static Color mTopMaxDamage = new Color(1, 0.3658f, 0, 1);
        private static Color mBottomMinDamage = new Color(0, 1, 1, 1);
        private static Color mBottomMaxDamage = new Color(1, 1, 0, 1);

        public void SetDamage(float damage)
        {
            gameObject.SetActive(true);
            mText.text = $"{damage:F1}";
            float rate = damage * 0.001f;
            Color topCol = Color.Lerp(mTopMinDamage, mTopMaxDamage, rate);
            Color botCol = Color.Lerp(mBottomMinDamage, mBottomMaxDamage, rate);
            mText.colorGradient = new VertexGradient(topCol, topCol, botCol, botCol);
            FadeAsync().Forget();
        }

        private async UniTask FadeAsync(float time = 0.5f)
        {
            float originTime = time;
            while (time >= 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCts.Token);
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

        private void OnDestroy()
        {
            mCts.Cancel();
            mCts.Dispose();
        }

        private void Awake()
        {
            mCts = new CancellationTokenSource();
        }

        protected override void StartInit()
        {
        }
    }
}