using _2_Scripts.Game.Chapter;
using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PlayMissionItem : SerializedMonoBehaviour
    {
        enum ETextType
        {
            SubTitle,
            Title,
            Star,
            RewardAmount,
            ButtonText
        }

        enum EButtonType
        {
            GoToChapter,
            AcquireReward,
            AlreadyAcquired,
        }
        [SerializeField] private Dictionary<ETextType,TextMeshProUGUI> mTexts;
        [SerializeField] private Slider mSlider;
        [SerializeField] private Button mButton;
        [SerializeField] private Image mRewardItemImage;
        [SerializeField] private Dictionary<EButtonType,Sprite> mButtonSprites;
        private PlayMission mPlayMission;
        private GameMessage<int> mChapterMessage = new GameMessage<int>(EGameMessage.ChapterChange,0);
        public void InitItem(string missionKey,SO_PlayMission playMission)
        {
            mPlayMission = BackEndManager.Instance.UserPlayMission[missionKey];
            mTexts[ETextType.SubTitle].text = mPlayMission.MissionSubName;
            mTexts[ETextType.Title].text = mPlayMission.MissionName;
            mTexts[ETextType.RewardAmount].text = $"{playMission.Amount}";
            mRewardItemImage.sprite = playMission.ItemKey.GetData.Icon;
            StarTextChange(playMission.GetCurrentProgress(),playMission.GetMaxProgress());
            if (!playMission.IsClearMission())
            {
                if (playMission.ClearConditions[0] is ChapterMissionClearCondition)
                {
                    ChapterMissionClearCondition chapterMissionClearCondition = (ChapterMissionClearCondition) playMission.ClearConditions[0];
                    mChapterMessage.SetValue(chapterMissionClearCondition.ChapterIndex);
                }
                mButton.image.sprite = mButtonSprites[EButtonType.GoToChapter];
                mTexts[ETextType.ButtonText].text = "바로가기";      
                mButton.onClick.AddListener(() =>
                {
                    MessageBroker.Default.Publish(mChapterMessage);
                });
            }
            else if(mPlayMission.IsClear)
            {
                mTexts[ETextType.ButtonText].text = "획득완료";
                mButton.image.sprite = mButtonSprites[EButtonType.AlreadyAcquired];
                mButton.onClick.AddListener(() =>
                {
                    UI_Toast_Manager.Instance.Activate_WithContent_Func("미션 보상을 이미 획득했습니다.");
                });
            }
            else
            {
                mTexts[ETextType.ButtonText].text = "보상받기";      
                mButton.image.sprite = mButtonSprites[EButtonType.AcquireReward];
                mButton.onClick.AddListener(() =>
                {
                    if (playMission.ShouldGrantReward())
                    {
                        SoundManager.Instance.Play2DSound(AddressableTable.Sound_Get_Item);
                        UI_Toast_Manager.Instance.Activate_WithContent_Func("미션 보상을 획득했습니다!");
                        mTexts[ETextType.ButtonText].text = "획득완료";
                        mButton.image.sprite = mButtonSprites[EButtonType.AlreadyAcquired];
                        
                    }
                    else
                    {
                        UI_Toast_Manager.Instance.Activate_WithContent_Func("미션 보상을 이미 획득했습니다.");
                    }
                });
            
            }
        }
        
        private void StarTextChange(int currentStar, int maxStar)
        {
            mSlider.value = currentStar/(float)maxStar;
            mTexts[ETextType.Star].text = $"({currentStar}/{maxStar})";
        }
    }
}