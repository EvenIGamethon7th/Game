using _2_Scripts.Game.Sound;
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

        private readonly int mOnceLimitCount = 3;

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
                return;
            }
            
            mBackPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }

        private bool mbRewardDelay = false;
        private Queue<Define.RewardEvent> mRewardEvents = new Queue<Define.RewardEvent>();
        public void OnPopUp(List<Define.RewardEvent> dataValue)
        {
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Button);
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
            mbRewardDelay = false;
            int count = Mathf.Min(mRewardEvents.Count, mOnceLimitCount);
            for (int i = 0 ; i < count; i++)
            {
                mRewardSlots[i].gameObject.SetActive(true);
                mRewardSlots[i].UpdateSlot(mRewardEvents.Dequeue());
                await UniTask.WaitForSeconds(0.25f);
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