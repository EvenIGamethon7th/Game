using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using _2_Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI {
    public class UI_AcademyPannel : MonoBehaviour
    {
        private readonly Dictionary<int, string> mSummonProjectileDictionary = new()
        {
            {1,AddressableTable.Default_NormalProjectile},
            {2,AddressableTable.Default_RareProjectile},
            {3,AddressableTable.Default_UniqueProjectile},
        };
        [SerializeField]
        private RectTransform mCharacterRectTransform;

        private UI_AcademyLesson mLesson;
        private UI_AcademyStatus mStatus;

        private bool mDoLesson = false;

        private int mLessonCount = 0;

        private CharacterData mStudentData;

        [SerializeField]
        private TextMeshProUGUI mGradeText;

        public void Init()
        {
            mLesson = GetComponentInChildren<UI_AcademyLesson>(true);
            mStatus = GetComponentInChildren<UI_AcademyStatus>(true);

            mLesson.Init();

            MessageBroker.Default.Receive<GameMessage<int>>().Where(message => message.Message == EGameMessage.StageChange)
                .Subscribe(message =>
                {
                    LessonComplete(message.Value);
                }).AddTo(this);
        }

        public bool CanLesson()
        {
            if (mDoLesson)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 수업을 듣는 영웅이 있습니다!");
            }

            return !mDoLesson;
        }

        public void AcademyLesson(CUnit student)
        {
            mDoLesson = true;
            mStatus.SetStatus(student);
            mStudentData = student.CharacterDatas;
            mStudentData.isAlumni = true;
        }

        private void SummonAlumni()
        {
            bool isCreateUnit = MapManager.Instance.CreateUnit(mStudentData, true, (tilePos) =>
            {
                var uiWorldPos = global::Utils.GetUIWorldPosition(mCharacterRectTransform);
                var projectile = ObjectPoolManager.Instance.CreatePoolingObject(mSummonProjectileDictionary[mStudentData.rank], uiWorldPos);
                projectile.transform.DOMove(tilePos, 0.5f).OnComplete(() =>
                {
                    projectile.gameObject.SetActive(false);
                    var effect = ObjectPoolManager.Instance.CreatePoolingObject(Define.SpawnEffectDictionary[mStudentData.rank], tilePos);
                });
            });
            mLessonCount = 0;
            if (isCreateUnit)
            {
                mDoLesson = false;
                mStatus.Clear();
                mLesson.Init();
            }
        }

        private void LessonComplete(int waveCount)
        {
            if (!mDoLesson) return;

            if (mLessonCount < 5)
            {
                // while (mLessonCount < 5)
                // {
                    DecideLessonResult();
                    ++mLessonCount;
                // }
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
            if (result == ELessonResults.Bonanza) Handheld.Vibrate();

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
            
        }
    }
}