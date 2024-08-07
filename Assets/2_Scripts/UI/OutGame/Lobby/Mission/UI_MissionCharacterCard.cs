using System;
using System.Collections.Generic;
using _2_Scripts.Game.BackEndData.Mission;
using _2_Scripts.Utils;
using _2_Scripts.Utils.Components;
using TMPro;
using UniRx;
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
        [SerializeField] private Button mButton;

        private GameMessage<SpawnMission> mSelectCharacterCardMessage;
        
        public SpawnMission MissionData { get; private set; }
        //TODO Select Initialize add
        private void Start()
        {
            mSelectCharacterCardMessage = new GameMessage<SpawnMission>(EGameMessage.SelectCharacter, MissionData);
            mButton.onClick.AddListener(OnClickCard);
        }

        private void OnClickCard()
        {
            if (MissionData.IsGet == false)
                return;
            MessageBroker.Default.Publish(mSelectCharacterCardMessage);
        }
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
         
            MissionData = itemData;
            mSelectCharacterCardMessage?.SetValue(itemData);
            SelectBorder(cardState);
        }

        private void SelectBorder(EStateCard state)
        {
            bool isLock = state == EStateCard.Lock;
            MissionData.IsEquip = EStateCard.Equip == state;
            mBorderImage.sprite = mBorderSprite[state];
            
            string infoText = "";
            if (MissionData.IsGet)
            {
                if (MissionData.IsEquip)
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
                infoText = $"소환 하기 {MissionData.SpawnCount}/100";
            }
            mCharacterInfo.text = infoText;
            
            mLockIcon.SetActive(isLock);
        }
    }
}