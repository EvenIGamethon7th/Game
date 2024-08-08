using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_TutorialDialog : MonoBehaviour
    {
        private float mPrevSpeed;
        private CancellationTokenSource mCts = new CancellationTokenSource();

        [SerializeField]
        private RectTransform mRect;
        [SerializeField]
        private TextMeshProUGUI mDescriptionText;

        private StringBuilder mText = new StringBuilder(100);

        private string mCurrentDialog;

        private void OnEnable()
        {
            mPrevSpeed = Time.timeScale;
            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            Time.timeScale = mPrevSpeed;
        }

        public void SetPosition(Vector2 pos)
        {
            mRect.position = pos;
        }

        public async UniTask SetTextAsync(string dialog)
        {
            mDescriptionText.text = "";
            mCurrentDialog = dialog;

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
            mCts.Cancel();
            mCts.Dispose();
            mCts = new CancellationTokenSource();
            mDescriptionText.text = mCurrentDialog;
            mCurrentDialog = "";
            mText.Clear();
        }
    }
}