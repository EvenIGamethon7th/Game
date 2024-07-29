using System.Collections.Generic;
using _2_Scripts.Game.BackEndData.Mission;
using _2_Scripts.Utils.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public enum EStateCard
    {
        Lock,
        Unlock,
        Equip
    }
    public class UI_MissionCharacterCard : FancyGridViewCell<SpawnMission,Context>
    {
        [SerializeField] private TextMeshProUGUI mCharacterName;
        [SerializeField] private TextMeshProUGUI mCharacterInfo;
        [SerializeField] private Image mCharacterImage;
        [SerializeField] private Image mBorderImage;
        [SerializeField] private GameObject mLockIcon;
        [SerializeField] private Dictionary<EStateCard,Sprite> mBorderSprite;
        //TODO Select Initialize add
        public override void UpdateContent(SpawnMission itemData)
        {
            EStateCard cardState = EStateCard.Lock;
            var characterData = DataBase_Manager.Instance.GetCharacter.GetData_Func(itemData.CharacterKey);
            mCharacterImage.sprite = characterData.GetCharacterSprite();
            mCharacterName.text = characterData.GetCharacterName();
            if (itemData.IsGet)
            {
                cardState = itemData.IsEquip ? EStateCard.Equip : EStateCard.Unlock;
            }
            string infoText = "";
            if (itemData.IsGet)
            {
                if (itemData.IsEquip)
                {
                    infoText = "장착 중";
                }
                else
                {
                    infoText = "획득 완료";
                }
            }
            else
            {
                infoText = $"소환 하기 {itemData.SpawnCount}/100";
            }
            mCharacterInfo.text = infoText;
            SelectBorder(cardState);
        }
        public void SelectBorder(EStateCard state)
        {
            bool isLock = state == EStateCard.Lock;
            mBorderImage.sprite = mBorderSprite[state];
            mLockIcon.SetActive(isLock);
        }
    }
}