using Cargold;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public enum ELessonResults
    {
        Success,
        Bonanza,
        Fail,
        During
    }

    public class UI_AcademyLesson : MonoBehaviour
    {
        private readonly Color Bonanza = new Color(0.996f, 0.859f, 0.373f);
        private readonly Color Success = new Color(0.227f, 0.635f, 0.447f);
        private readonly Color Fail = new Color(0.996f, 0.169f, 0.153f);

        [SerializeField]
        private TextMeshProUGUI[] mResultTexts;
        [SerializeField]
        private TextMeshProUGUI[] mLessonTexts;

        [SerializeField]
        private Image mPortrail;

        [SerializeField]
        private Image[] mArrowImages;

        [SerializeField]
        private Sprite[] mSprites;

        public void Init()
        {
            for (int i = 0; i < mResultTexts.Length; ++i) 
            {
                mResultTexts[i].text = "";
                mLessonTexts[i].text = "";
                mArrowImages[i].gameObject.SetActive(false);
            }
        }

        public void SetLesson(CharacterData data)
        {
            for (int i = 0; i < mLessonTexts.Length; ++i)
            {
                DataBase_Manager.Instance.GetAcademyClass.TryGetData_Func($"AcademyClass_{(data.academyClass - 1) * 5 + i}", out var classData);
                mLessonTexts[i].text = classData.Name;
            }
        }

        public void DoLesson(int lessonNum)
        {
                mArrowImages[lessonNum].gameObject.SetActive(true);
                mArrowImages[lessonNum].sprite = mSprites[(int)ELessonResults.During];
        }

        public void SetResult(int lessonNum, ELessonResults result)
        {
            switch (result)
            {
                case ELessonResults.Fail:
                    mResultTexts[lessonNum].text = "실패..";
                    mResultTexts[lessonNum].color = Fail;
                    mArrowImages[lessonNum].sprite = mSprites[(int)ELessonResults.Fail];
                    break;

                case ELessonResults.Success:
                    mResultTexts[lessonNum].text = "성공!!";
                    mResultTexts[lessonNum].color = Success;
                    mArrowImages[lessonNum].sprite = mSprites[(int)ELessonResults.Success];
                    break;

                case ELessonResults.Bonanza:
                    mResultTexts[lessonNum].text = "대성공!!!";
                    mResultTexts[lessonNum].color = Bonanza;
                    mArrowImages[lessonNum].sprite = mSprites[(int)ELessonResults.Bonanza];
                    break;
            }
        }
    }
}