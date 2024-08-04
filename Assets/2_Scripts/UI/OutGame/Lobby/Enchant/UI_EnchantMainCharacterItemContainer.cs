using _2_Scripts.UI.OutGame.Enchant;
using Cargold.FrameWork.BackEnd;
using System;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby.Enchant
{
    public class UI_EnchantMainCharacterItemContainer : MonoBehaviour
    {
        [SerializeField]
        private GameObject mContent;
        [SerializeField]
        private GameObject mItemPrefab;

        public void Start()
        {
            foreach (var mainCharacter in GameManager.Instance.MainCharacterList)
            {
              var item = Instantiate(mItemPrefab, mContent.transform).GetComponent<UI_EnchantMainCharacterItem>();
              item.InitItem(mainCharacter);
            }
        }
    }
}