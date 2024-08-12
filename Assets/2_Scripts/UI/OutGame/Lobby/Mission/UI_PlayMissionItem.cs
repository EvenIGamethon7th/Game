using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private Dictionary<ETextType,TextMeshProUGUI> mTexts;
        [SerializeField] private Slider mSlider;
        [SerializeField] private Button mButton;
        [SerializeField] private Image mRewardItemImage;

        private PlayMission mPlayMission;
        public void InitItem(string missionKey,SO_PlayMission playMission)
        {
            mPlayMission = BackEndManager.Instance.UserPlayMission[missionKey];
            mTexts[ETextType.SubTitle].text = mPlayMission.MissionSubName;
            mTexts[ETextType.Title].text = mPlayMission.MissionName;
            mTexts[ETextType.RewardAmount].text = $"{playMission.Amount}";
            mRewardItemImage.sprite = playMission.ItemKey.GetData.Icon;
            StarTextChange(playMission.GetCurrentProgress(),playMission.GetMaxProgress());
        }
        
        private void StarTextChange(int currentStar, int maxStar)
        {
            mSlider.value = currentStar/(float)maxStar;
            mTexts[ETextType.Star].text = $"({currentStar}/{maxStar})";
        }
    }
}