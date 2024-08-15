using System;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Shop
{
    public class UI_TobMenuShopBtn : MonoBehaviour
    {
        [SerializeField]
        private UI_ShopButtonController mShopButtonController;

        [SerializeField] private EShopTab tab;
        private Button mButton;

        private void Start()
        {
            mButton = GetComponent<Button>();
            mButton.onClick.AddListener(()=>mShopButtonController.OpenTopOpenUI(tab));
        }
    }
}