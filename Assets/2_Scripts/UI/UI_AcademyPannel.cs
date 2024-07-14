using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.UI {
    public class UI_AcademyPannel : MonoBehaviour
    {
        private UI_AcademyLesson mLesson;
        private UI_AcademyStatus mStatus;

        private bool mDoLesson = false;

        private int mLessonCount = 0;

        private CharacterData mStudentData;

        public void Init()
        {
            mLesson = GetComponentInChildren<UI_AcademyLesson>(true);
            mStatus = GetComponentInChildren<UI_AcademyStatus>(true);

            mLesson.Init();
        }

        public bool CanLesson(CUnit student)
        {
            if (mDoLesson)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 수업을 듣는 영웅이 있습니다!");
            }

            //else if의 이유: 두 토스트 메시지가 함께 뜨면 짜쳐서
            else if (student.CharacterDatas.isAlumni)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 아카데미를 졸업한 학생입니다!");
            }

            return !student.CharacterDatas.isAlumni && !mDoLesson;
        }

        public void AcademyLesson(CUnit student)
        {
            mDoLesson = true;
            mStatus.SetStatus(student);
            StageManager.Instance.SubscribeWaveStart(LessonComplete);
            mStudentData = student.CharacterDatas;
            mStudentData.isAlumni = true;
        }

        private void SummonAlumni()
        {
            bool isCreateUnit = MapManager.Instance.CreateUnit(mStudentData, true);
            mLessonCount = 0;
            if (isCreateUnit)
            {
                StageManager.Instance.UnSubscribeWaveStart(LessonComplete);
                mDoLesson = false;
                mStatus.Clear();
                mLesson.Init();
            }
        }

        private void LessonComplete(int waveCount)
        {
            if (mLessonCount < 5)
            {
                DecideLessonResult();
                ++mLessonCount;
            }

            if (mLessonCount == 5)
            {
                SummonAlumni();
            }
        }

        private void DecideLessonResult()
        {
            ELessonResults result = (ELessonResults)Random.Range(0, 3);
            mLesson.SetResult(mLessonCount, result);
            if (result == ELessonResults.Fail) return;
            Handheld.Vibrate();
            switch (mLessonCount)
            {
                case 0:
                    if (result == ELessonResults.Success)
                        mStudentData.alumniAtk += 10;
                    else if (result == ELessonResults.Bonanza)
                        mStudentData.alumniAtk += 15;
                    break;

                case 1:
                    if (result == ELessonResults.Success)
                        mStudentData.alumniAtkSpeed += 0.1f;
                    else if (result == ELessonResults.Bonanza)
                        mStudentData.alumniAtkSpeed += 0.2f;
                    break;

                case 2:
                    if (result == ELessonResults.Success)
                        mStudentData.alumniAtk += 20;
                    else if (result == ELessonResults.Bonanza)
                        mStudentData.alumniAtk += 25;
                    break;

                case 3:
                    if (result == ELessonResults.Success)
                        mStudentData.alumniMatk += 10;
                    else if (result == ELessonResults.Bonanza)
                        mStudentData.alumniMatk += 20;
                    break;

                case 4:
                    if (result == ELessonResults.Success)
                        mStudentData.alumniAtkSpeed += 0.2f;
                    else if (result == ELessonResults.Bonanza)
                        mStudentData.alumniAtkSpeed += 0.3f;
                    break;
            }
            mStatus.SetStatus(mStudentData);
        }

        private void OnDestroy()
        {
            StageManager.Instance.UnSubscribeWaveStart(LessonComplete);
        }
    }
}