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

        private string[] mInfoString = { "<color=#C90F0F>엘피</color>를 클릭하고\n유닛 합성을 클릭해서 합성해라모", "<color=#C90F0F>학생을 아무나</color> 선택하고\n오른쪽 아카데미 버튼을 클릭해서\n입학시키라모!" };

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