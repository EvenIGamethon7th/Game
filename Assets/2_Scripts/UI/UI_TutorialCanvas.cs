using _2_Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_TutorialCanvas : UI_Base
    {
        private Queue<StoryData> mTutorialData = new Queue<StoryData>();
        private GameMessage<bool> mTutorialMessage;

        [SerializeField]
        private List<Button> mButtons;
        [SerializeField]
        private List<bool> mControllerbool;

        private int mCount;

        private void Awake()
        {
            mTutorialMessage = new GameMessage<bool>(EGameMessage.Tutorial, false);
            for (int i = 0; i < mButtons.Count; ++i)
            {
                if (mButtons[i] != null)
                    mButtons[i].interactable = false;
            }
            MessageBroker.Default.Receive<GameMessage<bool>>().
                Where(message => (int)message.Message > (int)EGameMessage.Tutorial).
                Subscribe(message => { if (!message.Value) SetInteractable(); }).AddTo(this);
            this.ObserveEveryValueChanged(_ => IngameDataManager.Instance.CurrentGold)
                .Where(_ => IngameDataManager.Instance.CurrentGold > 30)
                .Take(1)
                .Subscribe(_ => {
                    mTutorialMessage.SetValue(false);
                    MessageBroker.Default.Publish(mTutorialMessage);
                    SetInteractable(); 
                    //Stop World
                })
                .AddTo(this);
        }

        private void OnEnable()
        {
            MessageBroker.Default.Publish(mTutorialMessage);
            SetInteractable();
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

        }
    }
}