using Cargold;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI
{
    public enum ELessonResults
    {
        Fail,
        Success,
        Bonanza
    }

    public class UI_AcademyLesson : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI[] mTexts;

        public void Init()
        {
            for (int i = 0; i < mTexts.Length; ++i) 
            {
                mTexts[i].text = "";
            }
        }

        public void DoLesson(int lessonNum)
        {
            mTexts[lessonNum].text = "... 진행중";
            mTexts[lessonNum ].color = Color.black;
        }

        public void SetResult(int lessonNum, ELessonResults result)
        {
            switch (result)
            {
                case ELessonResults.Fail:
                    mTexts[lessonNum].text = "실패..";
                    mTexts[lessonNum].color = Color.red;
                    break;

                case ELessonResults.Success:
                    mTexts[lessonNum].text = "성공!!";
                    mTexts[lessonNum].color = Color.green;
                    break;

                case ELessonResults.Bonanza:
                    mTexts[lessonNum].text = "대성공!!!";
                    mTexts[lessonNum].color = Color.yellow;
                    break;
            }
        }
    }
}