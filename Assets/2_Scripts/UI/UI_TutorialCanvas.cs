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


namespace _2_Scripts.UI
{
    public class UI_TutorialCanvas : UI_Base
    {
        private Queue<StoryData> mTutorialData = new Queue<StoryData>();
        private StoryData mCurrentData;
        private StoryData mPrevData;
        private GameMessage<bool> mTutorialMessage;

        [SerializeField]
        private Tutorial_InfoPanel mInfoPanel;

        [SerializeField]
        private GameObject mCursor;

        [SerializeField]
        private GameObject mCursor2;

        [SerializeField]
        private List<UnityEngine.UI.Button> mButtons;
        [SerializeField]
        private List<bool> mControllerbool;

        private UI_TutorialDialog mDialog;

        private int mCount;
        private int mGroup = 2;

        private bool mIsSpeaking;
        private int mHardClick;
        private bool mIsRewind;

        private void Awake()
        {
            mDialog = GetComponentInChildren<UI_TutorialDialog>(true);
            mTutorialMessage = new GameMessage<bool>(EGameMessage.Tutorial, false);
            for (int i = 0; i < mButtons.Count; ++i)
            {
                if (mButtons[i] != null)
                    mButtons[i].interactable = false;
            }
            SceneLoadManager.Instance.OnSceneLoad -= Init;
            SceneLoadManager.Instance.OnSceneLoad += Init;
        }

        private void OnEnable()
        {
            MessageBroker.Default.Publish(mTutorialMessage);
            SetInteractable();
        }

        protected override void StartInit()
        {
            if (GameManager.Instance.IsTest)
                Init();
        }

        private void SetText()
        {
            if (mIsSpeaking)
            {
                mIsSpeaking = false;
                mDialog.Cancel();
            }

            else
            {
                if (mTutorialData.Count <= 0)
                {
                    mDialog.gameObject.SetActive(false);
                    mButtons[mCount].interactable = false;
                    MessageBroker.Default.Publish(new TaskMessage(ETaskList.GameOver));
                    return;
                }

                var data = mTutorialData.Peek();

                if (data.Story_Group != mGroup)
                {
                    mDialog.gameObject.SetActive(false);
                    mGroup = data.Story_Group;
                    return;
                }

                mPrevData = mCurrentData;
                mCurrentData = data;
                SetTextAsync(mCurrentData).Forget();
            }
        }

        private async UniTask SetTextAsync(StoryData data)
        {
            mDialog.gameObject.SetActive(true);
            if (mPrevData != data)
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
                if (mInfoPanel.gameObject.activeSelf)
                {
                    Debug.Log(mCount);
                    mInfoPanel.CurrentNum = mCount;
                }
            }

            mTutorialMessage.SetValue(mControllerbool[mCount]);
            MessageBroker.Default.Publish(mTutorialMessage);

            ++mCount;

            if (!mTutorialMessage.Value && mButtons[mCount] != null)
                SetInteractable();
        }

        private void Init()
        {
            SceneLoadManager.Instance.OnSceneLoad -= Init;
            //After Summon
            this.ObserveEveryValueChanged(_ => StageManager.Instance.WaveCount)
                .Where(_ => StageManager.Instance.WaveCount == 1)
                .Take(1)
                .Subscribe(_ =>
                {
                    StopWorld();
                })
                .AddTo(this);

            this.ObserveEveryValueChanged(_ => StageManager.Instance.WaveCount)
                .Where(_ => StageManager.Instance.WaveCount == 2)
                .Take(1)
                .Subscribe(_ =>
                {
                    StopWorld();
                })
                .AddTo(this);

            this.ObserveEveryValueChanged(_ => StageManager.Instance.WaveCount)
                .Where(_ => StageManager.Instance.WaveCount == 4)
                .Take(1)
                .Subscribe(_ =>
                {
                    StopWorld();
                })
                .AddTo(this);

            this.ObserveEveryValueChanged(_ => StageManager.Instance.WaveCount)
                .Where(_ => StageManager.Instance.WaveCount == 5)
                .Take(1)
                .Subscribe(_ =>
                {

                    DelayTime().Forget();

                    async UniTask DelayTime()
                    {
                        await UniTask.WaitForSeconds(5);
                        StopWorld(true);
                        mButtons[mCount].interactable = true;
                    }
                })
                .AddTo(this);

            MessageBroker.Default.Receive<TaskMessage>().
                Where(task => task.Task == ETaskList.BossSpawn).
                Take(1).
                Subscribe(_ =>
                {
                    mTutorialMessage.SetValue(false);
                    MessageBroker.Default.Publish(mTutorialMessage);
                    SetText();
                }).AddTo(this);

            for (int i = 1; i < 100; ++i)
            {
                if (!DataBase_Manager.Instance.GetStory.TryGetData_Func($"Script_{i + 1000}", out var data)) break;

                mTutorialData.Enqueue(data);
            }

            mDialog.gameObject.SetActive(true);
            SetTextAsync("안녕 소환사님!\n먼저 학생을 소환해주세모!").Forget();

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonUp(0) && mDialog.gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    if (mIsRewind)
                        --mGroup;
                    SetText();
                }).AddTo(this);

            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (mButtons[mCount] != null && Time.timeScale > 0.1f)
                    {
                        if (mCount < 13)
                        {
                            mCursor.transform.position = mButtons[mCount].transform.position;
                            mCursor.SetActive(mButtons[mCount].interactable && mButtons[mCount].gameObject.activeInHierarchy);
                        }

                        else
                        {
                            mCursor.SetActive(false);
                            mCursor2.SetActive(mButtons[mCount].interactable && mButtons[mCount].gameObject.activeInHierarchy);
                        }
                    }
                }).AddTo(this);

            MessageBroker.Default.Receive<GameMessage<bool>>().
                Where(message => message.Message == EGameMessage.TutorialProgress).
                Subscribe(message =>
                {
                    if (mCount == 11)
                        StopWorld(message.Value, 1.5f, false);
                       
                    else
                        StopWorld(message.Value);
                }).AddTo(this);

            MessageBroker.Default.Receive<EGameMessage>().
                Where(task => task == EGameMessage.BossDeath).
                Take(1).
                Subscribe(_ =>
                {
                    mIsRewind = false;
                    if (mGroup == 10)
                    {
                        mTutorialData.Dequeue();
                        ++mGroup;
                    }
                    StopWorld();
                }).AddTo(this);

            MessageBroker.Default.Receive<GameMessage<bool>>().
               Where(message => message.Message == EGameMessage.TutorialRewind).
               Subscribe(message =>
               {
                   if (mGroup == 10)
                       SetText();
                   else if (mGroup == 11)
                   {
                       mIsRewind = true;
                       SetTextAsync(mPrevData).Forget();
                   }
               }).AddTo(this);

            mButtons[0].onClick.AddListener(Click1);
            mButtons[1].onClick.AddListener(Click2);

            void Click1()
            {
                ++mHardClick;
                if (mHardClick == 5)
                {
                    mButtons[0].onClick.RemoveListener(Click1);
                    StopWorld();
                }
            }

            void Click2()
            {
                ++mHardClick;
                if (mHardClick == 4)
                {
                    mButtons[0].onClick.RemoveListener(Click2);
                    StopWorld();
                }
            }

            void StopWorld(bool isMove = false, float time = 0, bool isInfo = true)
            {
                mTutorialMessage.SetValue(isMove);
                MessageBroker.Default.Publish(mTutorialMessage);
                if (!isMove)
                    SetInteractable();
                if (!isMove && mCount == 11)
                    time = 0;

                delayTime().Forget();

                async UniTask delayTime()
                {
                    while (time > 0)
                    {
                        await UniTask.DelayFrame(1);
                        time -= Time.deltaTime;
                    }
                    SetText();
                    Debug.Log(mCount);
                    if (isInfo)
                        mInfoPanel.CurrentNum = mCount;
                }

            }
        }
    }
}