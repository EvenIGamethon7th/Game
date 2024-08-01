using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Dialog
{
    public class UI_FadeImage : MonoBehaviour
    {
        private Image mFade;
        private CancellationTokenSource mCts = new CancellationTokenSource();
        [SerializeField]
        private Color mBaseColor;
        [SerializeField]
        private Color mFadeColor;

        private void Awake()
        {
            mFade = GetComponent<Image>();
            mFadeColor = mFade.color;
            mBaseColor = new Color(mFadeColor.r, mFadeColor.g, mFadeColor.b, 1);
        }

        public void Fade(float duringTime, float fadeTime)
        {
            gameObject.SetActive(true);
            FadeAsync(duringTime, fadeTime).Forget();
        }

        private async UniTask FadeAsync(float duringTime, float fadeTime)
        {
            float temp = duringTime;

            while (temp > 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCts.Token);
                temp -= Time.deltaTime;
                mFade.color = Color.Lerp(mBaseColor, mFadeColor, temp);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(fadeTime), cancellationToken: mCts.Token);

            temp = duringTime;

            while (temp > 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCts.Token);
                temp -= Time.deltaTime;
                mFade.color = Color.Lerp(mFadeColor, mBaseColor, temp);
            }

            gameObject.SetActive(false);
        }

        public async UniTask OnlyFadeIn(float duringTime)
        {
            gameObject.SetActive(true);
            float temp = duringTime;

            while (temp > 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCts.Token);
                temp -= Time.deltaTime;
                mFade.color = Color.Lerp(mBaseColor, mFadeColor, temp);
            }
        }

        public void Clear()
        {
            mCts.Cancel();
            mCts.Dispose();
            mCts = new();
        }
    }
}