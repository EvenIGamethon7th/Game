using Cargold.FrameWork.BackEnd;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.DailyReward
{
    public class UI_DailyContainer : MonoBehaviour
    {
        [SerializeField]
        private UI_DailyRewardSlot[] mDailyRewardSlots;

        [SerializeField] private UI_DailyRewardSlot mTopSlot;

        [SerializeField] private Button mAquireButton;
        private void Start()
        {
            var currentDailyReward =  mDailyRewardSlots.FirstOrDefault(x => x.CurrentDaily == BackEndManager.Instance.UserDailyReward + 1);
             mTopSlot.SetItemKey(currentDailyReward.ItemKey,currentDailyReward.ItemAcquisition,currentDailyReward.mAmount);
             mTopSlot.UpdateSlot();
             mAquireButton.onClick.AddListener(()=>
             {
                 mTopSlot.OnClick();
                 this.gameObject.SetActive(false);
             });
        }
    }
}