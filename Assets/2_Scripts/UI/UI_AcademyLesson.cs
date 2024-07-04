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
                mTexts[i].text = "...";
                mTexts[i].color = Color.black;
            }
        }

        public void DoLesson(int lessonNum)
        {
            mTexts[lessonNum].text = "... Studying";
        }

        public void SetResult(int lessonNum, ELessonResults result)
        {
            StringBuilder sb = new(20);
            sb.Append($"... {result}");
            mTexts[lessonNum].color = Color.blue;
            switch (result)
            {
                case ELessonResults.Fail:
                    sb.Append("..");
                    mTexts[lessonNum].color = Color.red;
                    break;

                case ELessonResults.Success:
                    sb.Append("!!");
                    break;

                case ELessonResults.Bonanza:
                    sb.Append("!!!");
                    break;
            }

            mTexts[lessonNum].text = sb.ToString();
        }
    }
}