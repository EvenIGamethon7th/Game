using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

namespace _2_Scripts.Game.Dialog
{
    public class UI_DialogPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI mNameText;

        [SerializeField]
        private TextMeshProUGUI mDescriptionText;

        [SerializeField]
        private UI_FadeImage mImage;

        private CancellationTokenSource mCts = new CancellationTokenSource();
        private StringBuilder mText = new StringBuilder(100);

        private string mCurrentDialog;

        public async UniTask SetTextAsync(string name, string dialog)
        {
            //mImage.gameObject.SetActive(false);
            mDescriptionText.text = "";
            mCurrentDialog = dialog;
            mNameText.text = name;

            for (int i = 0; i < mCurrentDialog.Length; ++i)
            {
                if (mCurrentDialog[i].Equals('<'))
                {
                    while (true)
                    {
                        mText.Append(mCurrentDialog[i]);
                        if (mCurrentDialog[i].Equals('>'))
                            break;
                        ++i;
                    }
                    mDescriptionText.text = mText.ToString();
                }

                else
                {
                    mText.Append(mCurrentDialog[i]);
                    mDescriptionText.text = mText.ToString();
                }
                await UniTask.DelayFrame(3, cancellationToken: mCts.Token);
            }
            mText.Clear();
        }

        public void Cancel()
        {
            //mImage.gameObject.SetActive(true);
            //mImage.Fade(0.5f, 0);
            mCts.Cancel();
            mCts.Dispose();
            mCts = new CancellationTokenSource();
            mDescriptionText.text = mCurrentDialog;
            mCurrentDialog = "";
            mText.Clear();
        }
    }
}