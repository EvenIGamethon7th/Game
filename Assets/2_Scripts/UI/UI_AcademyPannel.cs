using _2_Scripts.Game.Sound;
using _2_Scripts.Game.Unit;
using _2_Scripts.UI;
using _2_Scripts.UI.Ingame;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

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
        private bool mIsVacation = true;
        private bool mOutAsync = false;

        private int mLessonCount = 0;

        private CharacterData mStudentData;
        private CharacterData mTempAlumniData;

        private AcademyClassData[] mClassData = new AcademyClassData[5];
        private float[] mClassRate = new float[3];

        private GameMessage<bool> mCanLesson;

        [SerializeField]
        private SpriteAtlas mAtlas;

        [SerializeField]
        private Image mClassImage;

        private UI_AcademyInfo mInfo;
        [SerializeField]
        private GameObject mVacation;
        private float mLessonTime = 2;
        private int mLessonInWaveCount;

        [SerializeField]
        private UI_AcademyToast mToast;

        [SerializeField]
        private TextMeshProUGUI mOverlayText;

        private bool mTest;

        private CancellationTokenSource mCts = new ();

        public void Init()
        {
            mLessonTime = BackEndManager.Instance.IsUserTutorial ? 2 : 1;
            SceneLoadManager.Instance.SceneClear += Clear;
            mTest = GameManager.Instance.IsTest || BackEndManager.Instance.IsUserTutorial;
            mLesson = GetComponentInChildren<UI_AcademyLesson>(true);
            mStatus = GetComponentInChildren<UI_AcademyStatus>(true);
            mInfo = GetComponentInChildren<UI_AcademyInfo>(true);
            mLesson.Init();
            mInfo.Init();
            mToast?.Init();
            mStatus.Clear();
            mClassImage.gameObject.SetActive(false);

            MessageBroker.Default.Receive<GameMessage<int>>().Where(message => message.Message == EGameMessage.StageChange)
                .Subscribe(message =>
                {
                    LessonComplete(message.Value);
                }).AddTo(this);
            MessageBroker.Default.Receive<GameMessage<CUnit>>().Where(message => message.Message == EGameMessage.GoAcademy)
                .Subscribe(message =>
                {
                    CanLesson(message.Value);
                }).AddTo(this);

            mCanLesson = new GameMessage<bool>(EGameMessage.GoAcademy, false);
        }

        private void CanLesson(CUnit student)
        {
            mCanLesson.SetValue(mDoLesson || mIsVacation || mLessonInWaveCount != 0);
            
            if (mDoLesson)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 수업을 듣는 영웅이 있습니다!", isIgnoreTimeScale: true);
            }

            else if (mIsVacation)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("아카데미 방학 시즌입니다!", isIgnoreTimeScale: true);
            }

            else if (mLessonInWaveCount != 0)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 아카데미를 이번 웨이브에 다녀왔습니다!", isIgnoreTimeScale: true);
            }

            else
            {
                mToast?.PlayToast("아카데미 다녀올게요!", false);
                AcademyLesson(student);
            }

            MessageBroker.Default.Publish(mCanLesson);
        }

        private void AcademyLesson(CUnit student)
        {
            if (mTest)
                mToast?.Clear();
            mDoLesson = true;
            mTempAlumniData = MemoryPoolManager<CharacterData>.CreatePoolingObject();
            mStudentData = MemoryPoolManager<CharacterData>.CreatePoolingObject();
            mStudentData.Init(student.CharacterDatas, null);
            mStatus.Init(student);

            for (int i = 0; i < mClassData.Length; ++i)
            {
                DataBase_Manager.Instance.GetAcademyClass.TryGetData_Func($"AcademyClass_{(student.CharacterDatas.academyClass - 1) * 5 + i}", out var classData);
                mClassData[i] = classData;
            }

            mLesson.SetLesson(student.CharacterDatas);
            mClassImage.gameObject.SetActive(true);

            mStudentData.isAlumni = true;
            ++mLessonInWaveCount;
            DoLessonAsync().Forget();
        }

        private void SummonAlumni()
        {
            mStudentData.AddAlumniInfo(mTempAlumniData);
            mTempAlumniData.Clear();
            
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

            if (isCreateUnit)
            {
                mToast?.PlayToast("아카데미에서 돌아왔어요!", false);
                mLessonCount = 0;
                mClassImage.gameObject.SetActive(false);
                mDoLesson = false;
                mStudentData.Clear();
                mStudentData = null;
                mStatus.Clear();
                mLesson.Init();
                mTempAlumniData = null;
                if (!BackEndManager.Instance.IsUserTutorial && !GameManager.Instance.IsTest) MessageBroker.Default.Publish(new GameMessage<bool>(EGameMessage.TutorialProgress, true));
                SetOverlay();
            }
        }

        private void LessonComplete(int waveCount)
        {
            mLessonInWaveCount = 0;
            if (waveCount % 4 == 0 && (waveCount %= 10) != 0) 
            { 
                mIsVacation = false;
                if (mTest && !mDoLesson)
                {
                    SoundManager.Instance.Play2DSound(AddressableTable.Sound_Academy_Open);
                    mToast?.PlayToast("아카데미 오픈");
                }
            }
            else
            {
                if (mTest)
                    mToast?.Clear();
                mIsVacation = true;
            }

            SetOverlay();

            if (mLessonCount >= 5 && mOutAsync)
            {
                SummonAlumni();
            }
        }

        private void SetOverlay()
        {
            if (mIsVacation)
            {
                mVacation?.SetActive(!mDoLesson);
                mOverlayText.text = "방학입니다!";
            }

            else if (!mDoLesson && mLessonInWaveCount > 0)
            {
                mVacation.SetActive(true);
                mOverlayText.text = "수업 완료!";
            }

            else
            {
                mVacation.SetActive(false);
            }
        }

        private async UniTask DoLessonAsync()
        {
            mOutAsync = false;
            float time = 0;

            mLesson.DoLesson(mLessonCount);
            mClassImage.sprite = mAtlas.GetSprite(mClassData[mLessonCount].AcademyClassKey);
            SetInfoRate();

            if (!BackEndManager.Instance.IsUserTutorial && !GameManager.Instance.IsTest) await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger);

            while (mLessonCount < 5)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCts.Token);
                time += Time.deltaTime;
                if (time < mLessonTime) continue;

                mLesson.DoLesson(mLessonCount);
                mClassImage.sprite = mAtlas.GetSprite(mClassData[mLessonCount].AcademyClassKey);
                SetInfoRate();

                time -= mLessonTime;
                DecideLessonResult();
                mLessonCount++;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(mLessonTime), cancellationToken: mCts.Token);
            mOutAsync = true;
            SummonAlumni();
        }

        private void DecideLessonResult()
        {
            int rate = global::Utils.GetRandomIntBasedOnRates(mClassRate);
            ELessonResults result = (ELessonResults)rate;
            mLesson.SetResult(mLessonCount, result);

            if (result == ELessonResults.Fail) return;
            
            else if (result == ELessonResults.Bonanza) 
            {
                SoundManager.Instance.Vibrate();
                SelectStat(mClassData[mLessonCount].Stat_value2);
                Cargold.UI.UI_Toast_Manager.Instance.Activate_WithContent_Func("아카데미 수업 대성공!", isIgnoreTimeScale: true);
            }

            else
            {
                SelectStat(mClassData[mLessonCount].Stat_value1);
            }

            
            mStatus.SetStatus(mTempAlumniData);
        }

        private void SelectStat(float stat)
        {
            if (mClassData[mLessonCount].Stat_type1.Contains("마법공격력"))
            {
                mTempAlumniData.alumniMatk += stat;
            }

            else if (mClassData[mLessonCount].Stat_type1.Contains("공격속도"))
            {
                mTempAlumniData.alumniAtkSpeed += stat;
            }

            else
            {
                mTempAlumniData.alumniAtk += stat;
            }
        }

        private void SetInfoRate()
        {
            if (GameManager.Instance.IsUseItem(EItemType.Lecturer1st))
            {
                mClassRate[0] = mClassData[mLessonCount].Success_pro_item1 * 100;
                mClassRate[1] = mClassData[mLessonCount].Great_pro_item1 * 100;
                mClassRate[2] = mClassData[mLessonCount].Fail_pro_item1 * 100;
            }

            else if (GameManager.Instance.IsUseItem(EItemType.Lecturer2nd))
            {
                mClassRate[0] = mClassData[mLessonCount].Success_pro_item2 * 100;
                mClassRate[1] = mClassData[mLessonCount].Great_pro_item2 * 100;
                mClassRate[2] = mClassData[mLessonCount].Fail_pro_item2 * 100;
            }

            else
            {
                mClassRate[0] = mClassData[mLessonCount].Success_pro * 100;
                mClassRate[1] = mClassData[mLessonCount].Great_pro * 100;
                mClassRate[2] = mClassData[mLessonCount].Fail_pro * 100;
            }
            mInfo.SetText(mClassRate);
        }

        private void Clear()
        {
            SceneLoadManager.Instance.SceneClear -= Clear;
            mStudentData?.Clear();
            mTempAlumniData?.Clear();

            mCts.Cancel();
            mCts.Dispose();
        }
    }
}