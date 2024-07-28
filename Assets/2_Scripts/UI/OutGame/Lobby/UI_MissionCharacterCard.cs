using _2_Scripts.Game.BackEndData.Mission;
using _2_Scripts.Utils.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_MissionCharacterCard : FancyGridViewCell<SpawnMission,Context>
    {
        [SerializeField] private TextMeshProUGUI mCharacterName;
        [SerializeField] private TextMeshProUGUI mCharacterInfo;
        [SerializeField] private Image mCharacterImage;
        
        //TODO Select Initialize add
        public override void UpdateContent(SpawnMission itemData)
        {
            var characterData = DataBase_Manager.Instance.GetCharacter.GetData_Func(itemData.CharacterKey);
            mCharacterImage.sprite = characterData.GetCharacterSprite();
            mCharacterName.text = characterData.GetCharacterName();
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
        }
        
        protected override void UpdatePosition(float normalizedPosition, float localPosition)
        {
            base.UpdatePosition(normalizedPosition, localPosition);
            
        }
    }
}