using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Dialog
{
    public class UI_DialogCanvas : UI_Base
    {
        [SerializeField]
        private UI_FadeImage mFadePanel;
        [SerializeField]
        private UI_ChoosPanel mChoose;
        [SerializeField]
        private UI_DialogShadow mDialogShadow;

        private Queue<StoryData> mDatas = new Queue<StoryData>();

        private Queue<StoryData> mPrevData = new Queue<StoryData>();

        private StoryData mCurrentData;

        [SerializeField]
        private Image mBackGround;
        [SerializeField]
        private Sprite[] mBackGrounds;
        [SerializeField]
        private Button mSkipButton;
        [SerializeField]
        private Button mAutoButton;

        private UI_DialogPanel mDialog;
        private bool mIsSpeaking = false;
        private bool mIsAuto = false;

        private int mGroup = 0;

        private CancellationTokenSource mCts = new CancellationTokenSource();

        protected override void StartInit()
        {
            base.StartInit();

            for (int i = 1; i < 100; ++i)
            {
                if (!DataBase_Manager.Instance.GetStory.TryGetData_Func($"Script_{i}", out var data)) break;

                mDatas.Enqueue(data);
            }
            mDialog = GetComponentInChildren<UI_DialogPanel>(true);
            mSkipButton.onClick.AddListener(SceneChange);
            mDialog.GetComponent<Button>().onClick.AddListener(SetText);
            mDialog.GetComponent<Button>().onClick.AddListener(() => mIsAuto = false);
            mAutoButton.onClick.AddListener(IsAuto);
            SetText();
            AutoText().Forget();
        }

        private async UniTask AutoText()
        {
            while (mDatas.Count > 0)
            {
                await UniTask.WhenAll(UniTask.WaitUntil(() => mIsAuto, cancellationToken: mCts.Token), UniTask.WaitUntil(() => !mIsSpeaking, cancellationToken: mCts.Token));
                SetText();
                float duration = mCurrentData != null ? mCurrentData.Duration : 0;
                if (duration > 100)
                {
                    mIsAuto = false;
                    duration = 0;
                }
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: mCts.Token);
            }
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
                if (mDatas.Count <= 0)
                {
                    SceneChange();
                    return;
                }

                mCurrentData = mDatas.Peek();

                if (!mCurrentData.IsChoice)
                {
                    mCurrentData = mDatas.Dequeue();
                    SetTextAsync(mCurrentData).Forget();
                    mDialogShadow.Setalpha(mCurrentData.shadow).Forget();
                    return;
                }

                List<StoryData> datas = new List<StoryData>();
               
                mDatas.TryPeek(out var data);
                mCurrentData = data;

                while (data != null && data.IsChoice)
                {
                    datas.Add(data);
                    mDatas.TryDequeue(out _);
                    mDatas.TryPeek(out data);
                }

                mChoose.SetChoiceCard(datas, Skip);

                void Skip(int count)
                {
                    for (int i = 0; i < datas.Count; ++i)
                    {
                        mCurrentData = mDatas.Dequeue();
                        if (count == i)
                        {
                            SetTextAsync(mCurrentData).Forget();
                        }
                    }
                }
            }
        }

        private async UniTask SetTextAsync(StoryData data)
        {
            mIsSpeaking = true;

            if (mGroup != data.Group)
            {
                mBackGround.sprite = mBackGrounds[mGroup];
                mGroup = data.Group;
            }
            await mDialog.SetTextAsync(data.Speaker, data.Description);
            mIsSpeaking = false;
        }

        private void IsAuto()
        {
            mIsAuto = !mIsAuto;
        }

        private void SceneChange()
        {
            mDialog.GetComponent<Button>().onClick.RemoveAllListeners();
            mSkipButton.onClick.RemoveAllListeners();
            mCts.Cancel();
            mCts.Dispose();

            AfterFadeOut().Forget();

            async UniTask AfterFadeOut()
            {
                await mFadePanel.OnlyFadeIn(1);
                SceneLoadManager.Instance.SceneChange("LobbyScene");
            }
        }
    }
}