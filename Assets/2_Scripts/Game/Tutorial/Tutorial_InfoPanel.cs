using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class Tutorial_InfoPanel : MonoBehaviour
    {
        private TextMeshProUGUI mText;

        [SerializeField]
        private string[] mInfoString;

        private int mCount;

        public int CurrentNum 
        { 
            set 
            {
                mCurrentNum = value;
                gameObject.SetActive(false);
                if (mCount >= mInfoString.Length) return;

                if (mCurrentNum == ints[mCount])
                {
                    DelayActive().Forget();
                }
            } 
        }
        private int mCurrentNum;

        private async UniTask DelayActive()
        {
            await UniTask.WaitUntil(() => Time.timeScale > 0.1f);
            gameObject.SetActive(true);
        }

        [SerializeField]
        private int[] ints;

        private void Awake()
        {
            mText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            mText.text = mInfoString[mCount];
            ++mCount;
        }
    }
}