using System;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.StartPopUp
{
    public class UI_SelectItem : MonoBehaviour
    {
        [SerializeField] private Image mItemImage;
        [SerializeField] private Button mSelectButton;
        [SerializeField] private GameObject selectImage;
        public bool IsUseItem { get; private set; } = false;
        public StoreItem StoreItem { get; private set; }
        public void SetItem(StoreItem storeItem,Action<UI_SelectItem> onClick)
        {
            StoreItem = storeItem;
            mSelectButton.onClick.AddListener(() => onClick(this));
        }
        
        public bool SetSelect()
        {
            IsUseItem = !IsUseItem;
            selectImage.SetActive(IsUseItem);
            return IsUseItem;
        }
    }
}