using _2_Scripts.UI.OutGame.Enchant;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_RewardPopUpContainer : MonoBehaviour
    {
        // [SerializeField] private Image mRewardImage;
        // [SerializeField] private TextMeshProUGUI mRewardNameText;
        [SerializeField] private UI_RewardSlot[] mRewardSlots;
        [SerializeField] private Button mCloseButton;
        [SerializeField] private GameObject mBackPanel;

        public void Start()
        {
            mCloseButton.onClick.AddListener(OnTouchClose);
        }

        private void OnTouchClose()
        {
            if (mbRewardDelay == false)
                return;
            mBackPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }

        private bool mbRewardDelay = false;
        public void OnPopUp(List<Define.RewardEvent> dataValue)
        {
            mbRewardDelay = false;
            foreach (var data in dataValue)
            {
                data.rewardEvent?.Invoke();
            }
            OnPopUpAsync(dataValue).Forget();
      
        }
        public void OnPopUp(Define.RewardEvent dataValue)
        {
            dataValue?.rewardEvent?.Invoke();
            OnPopUpAsync(new List<Define.RewardEvent> { dataValue }).Forget();
        }
        
        private async UniTask OnPopUpAsync(List<Define.RewardEvent> dataValue)
        {
            for (int i = 0 ; i < dataValue.Count; i++)
            {
                mRewardSlots[i].gameObject.SetActive(true);
                mRewardSlots[i].UpdateSlot(dataValue[i]);
                await UniTask.WaitForSeconds(0.5f);
            }

            mbRewardDelay = true;
        }

        public void OnDisable()
        {
            foreach (var slot in mRewardSlots)
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
}