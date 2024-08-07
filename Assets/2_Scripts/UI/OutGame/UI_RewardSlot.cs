using _2_Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Enchant
{
    public class UI_RewardSlot : MonoBehaviour
    {
        [SerializeField] private Image mRewardImage;
        [SerializeField] private TextMeshProUGUI mRewardNameText;
        [SerializeField] private TextMeshProUGUI mCountText;
        public void UpdateSlot(Define.RewardEvent rewardEvent)
        {
            mRewardImage.sprite = rewardEvent.sprite;
            mRewardNameText.text = rewardEvent.name;
            mCountText.text = $"X{rewardEvent.count}";
        }
    }
}