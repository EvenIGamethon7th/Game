
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Encyclopedia
{
    public class UI_EncyclopediaItem : SerializedMonoBehaviour
    {
        [SerializeField] private Image mCharacterBorderContainer;
        [SerializeField] private Image mCharacterImage;
        [SerializeField] private TextMeshProUGUI mCharacterNameText;

        [SerializeField] private TextMeshProUGUI[] mSkillTextList;
        [SerializeField] private Dictionary<EEnchantClassType,Sprite> mBorderContainerSprite = new Dictionary<EEnchantClassType, Sprite>();
        public void SetItem(CharacterData characterData,EEnchantClassType classType)
        {
            mCharacterBorderContainer.sprite = mBorderContainerSprite[classType];
            mCharacterImage.sprite = characterData.Icon;
            mCharacterNameText.text = characterData.GetCharacterName();
            mSkillTextList[0].text = characterData.GetSkillDataLoc(1);
            mSkillTextList[1].text = characterData.GetSkillDataLoc(2);
        }
    }
}