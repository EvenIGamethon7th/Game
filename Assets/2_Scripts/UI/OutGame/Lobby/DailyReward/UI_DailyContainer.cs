using _2_Scripts.Game.Sound;
using Cargold.FrameWork.BackEnd;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.DailyReward
{
    public class UI_DailyContainer : MonoBehaviour,ISortPopUp
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
                 SoundManager.Instance.Play2DSound(AddressableTable.Sound_Button);
                 mTopSlot.OnClick();
                 IsPopUpEnd = true;
                 this.gameObject.SetActive(false);
             });
        }

        public int SortIndex { get; set; } = 3;
        public bool IsPopUpEnd { get; set; } = false;

        public void OnPopUp()
        {
            this.gameObject.SetActive(true);
        }
    }
}