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
        public void OnPopUp(List<Define.RewardEvent> dataValue)
        {
            foreach (var data in dataValue)
            {
                data.rewardEvent.Invoke();
            }
            OnPopUpAsync(dataValue).Forget();
        }
        private async UniTask OnPopUpAsync(List<Define.RewardEvent> dataValue)
        {
            for (int i = 0 ; i < dataValue.Count; i++)
            {
                await UniTask.WaitForSeconds(0.5f);
                mRewardSlots[i].gameObject.SetActive(true);
                mRewardSlots[i].UpdateSlot(dataValue[i]);
            }
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