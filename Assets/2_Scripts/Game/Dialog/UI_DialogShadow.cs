using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Dialog
{
    public class UI_DialogShadow : MonoBehaviour
    {
        private Image mImage;
        private int mCount;
        private Color prevColor = Color.clear;

        private void Awake()
        {
            mImage = GetComponent<Image>();
        }

        public async UniTask Setalpha(float alpha)
        {
            ++mCount;
            mImage.color = prevColor;
            Color newColor = new Color(0, 0, 0, alpha);
            prevColor = newColor;
            Color originColor = mImage.color;

            for (int i = 0; i < 5; ++i)
            {
                await UniTask.DelayFrame(3);
                if (mCount > 1)
                {
                    break;
                }

                mImage.color = Color.Lerp(originColor, newColor, (float)i * 0.2f);
            }
            --mCount;
        }
    }
}