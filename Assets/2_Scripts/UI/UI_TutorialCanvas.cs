using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.WSA;

namespace _2_Scripts.UI
{
    public class UI_TutorialCanvas : UI_Base
    {
        private Queue<StoryData> mTutorialData = new Queue<StoryData>();
        private StoryData mCurrentData;
        private GameMessage<bool> mTutorialMessage;

        [SerializeField]
        private GameObject mCursor;

        [SerializeField]
        private List<UnityEngine.UI.Button> mButtons;
        [SerializeField]
        private List<bool> mControllerbool;

        private UI_TutorialDialog mDialog;

        private int mCount;
        private int mGroup = 2;

        private bool mIsSpeaking;
        private int mHardClick;

        private void Awake()
        {
            mDialog = GetComponentInChildren<UI_TutorialDialog>(true);
            mTutorialMessage = new GameMessage<bool>(EGameMessage.Tutorial, false);
            for (int i = 0; i < mButtons.Count; ++i)
            {
                if (mButtons[i] != null)
                    mButtons[i].interactable = false;
            }
        }

        private void OnEnable()
        {
            MessageBroker.Default.Publish(mTutorialMessage);
            SetInteractable();
        }

        private void SetText()
        {
            mDialog.gameObject.SetActive(true);
            if (mIsSpeaking)
            {
                mIsSpeaking = false;
                mDialog.Cancel();
            }

            else
            {
                if (mTutorialData.Count <= 0)
                {
                    return;
                }

                mCurrentData = mTutorialData.Peek();

                if (mCurrentData.Story_Group != mGroup)
                {
                    mDialog.gameObject.SetActive(false);
                    mGroup = mCurrentData.Story_Group;
                    return;
                }

                SetTextAsync(mCurrentData).Forget();
            }
        }

        private async UniTask SetTextAsync(StoryData data)
        {
            mTutorialData.Dequeue();
            mIsSpeaking = true;

            if (!String.IsNullOrEmpty(data.sound))
            {
                BGMManager.Instance.PlaySound(data.sound, true);
            }

            await mDialog.SetTextAsync(data.Description);
            mIsSpeaking = false;
        }

        private async UniTask SetTextAsync(string data)
        {
            mIsSpeaking = true;

            await mDialog.SetTextAsync(data);
            mIsSpeaking = false;
        }

        private void SetInteractable()
        {
            if (mButtons[mCount] != null)
            {
                mButtons[mCount].onClick.RemoveListener(PublishMessage);
                mButtons[mCount].onClick.AddListener(PublishMessage);
                mButtons[mCount].interactable = true;
            }
        }

        private void PublishMessage()
        {
            if (mButtons[mCount] != null)
            {
                mButtons[mCount].onClick.RemoveListener(PublishMessage);
                mButtons[mCount].interactable = false;
            }

            mTutorialMessage.SetValue(mControllerbool[mCount]);
            MessageBroker.Default.Publish(mTutorialMessage);

            ++mCount;

            if (!mTutorialMessage.Value && mButtons[mCount] != null)
                SetInteractable();
        }

        protected override void StartInit()
        {
            //After Summon
            this.ObserveEveryValueChanged(_ => IngameDataManager.Instance.CurrentGold)
                .Where(_ => IngameDataManager.Instance.CurrentGold > 30)
                .Take(1)
                .Subscribe(_ => {
                    StopWorld();
                })
                .AddTo(this);

            this.ObserveEveryValueChanged(_ => StageManager.Instance.WaveCount)
                .Where(_ => StageManager.Instance.WaveCount == 2)
                .Take(1)
                .Subscribe(_ => {
                    StopWorld();
                })
                .AddTo(this);

            this.ObserveEveryValueChanged(_ => StageManager.Instance.WaveCount)
                .Where(_ => StageManager.Instance.WaveCount == 3)
                .Take(1)
                .Subscribe(_ => {
                    StopWorld();
                })
                .AddTo(this);

            for (int i = 1; i < 100; ++i)
            {
                if (!DataBase_Manager.Instance.GetStory.TryGetData_Func($"Script_{i + 1000}", out var data)) break;

                mTutorialData.Enqueue(data);
            }

            SetTextAsync("안녕 소환사님!\n먼저 학생을 소환해주세모!").Forget();

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonUp(0) && mDialog.gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    SetText();
                }).AddTo(this);

            this.UpdateAsObservable()
                .Subscribe(_ => {
                if (mButtons[mCount] != null && Time.timeScale > 0.1f)
                {
                    mCursor.transform.position = mButtons[mCount].transform.position;
                    mCursor.SetActive(mButtons[mCount].interactable && mButtons[mCount].gameObject.activeInHierarchy);
                }
            }).AddTo(this);

            MessageBroker.Default.Receive<GameMessage<bool>>().
                Where(message => message.Message == EGameMessage.TutorialProgress).
                Subscribe(message => {
                    StopWorld();
                }).AddTo(this);

            MessageBroker.Default.Receive<GameMessage<bool>>().
               Where(message => message.Message == EGameMessage.TutorialRewind).
               Subscribe(message => {
                   if (mGroup == 9)
                    StopWorld();
                   else if (mGroup == 10)
                   {
                       SetTextAsync(mCurrentData).Forget();
                   }
               }).AddTo(this);

            mButtons[0].onClick.AddListener(Click);

            void Click()
            {
                ++mHardClick;
                if (mHardClick == 3)
                {
                    mButtons[0].onClick.RemoveListener(Click);
                    StopWorld();
                }
            }

            void StopWorld()
            {
                mTutorialMessage.SetValue(false);
                MessageBroker.Default.Publish(mTutorialMessage);
                SetInteractable();
                SetText();
            }
        }
    }
}