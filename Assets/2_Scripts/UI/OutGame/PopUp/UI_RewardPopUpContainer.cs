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
            
            if (mRewardEvents.Count != 0)
            {
                ClearSlot();
                OnPopUpAsync().Forget();
            }
            
            mBackPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }

        private bool mbRewardDelay = false;
        private Queue<Define.RewardEvent> mRewardEvents = new Queue<Define.RewardEvent>();
        public void OnPopUp(List<Define.RewardEvent> dataValue)
        {
            mbRewardDelay = false;
            mRewardEvents = new Queue<Define.RewardEvent>(dataValue);
            foreach (var data in dataValue)
            {
                data.rewardEvent?.Invoke();
            }
            OnPopUpAsync().Forget();
      
        }
        public void OnPopUp(Define.RewardEvent dataValue)
        {
            dataValue?.rewardEvent?.Invoke();
            mRewardEvents.Enqueue(dataValue);
            OnPopUpAsync().Forget();
        }
        
        private async UniTask OnPopUpAsync()
        {
            for (int i = 0 ; i < mRewardEvents.Count || i < 3; i++)
            {
                mRewardSlots[i].gameObject.SetActive(true);
                mRewardSlots[i].UpdateSlot(mRewardEvents.Dequeue());
                await UniTask.WaitForSeconds(0.5f);
            }

            mbRewardDelay = true;
        }

        public void OnDisable()
        {
            ClearSlot();
        }

        private void ClearSlot()
        {
            foreach (var slot in mRewardSlots)
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
}