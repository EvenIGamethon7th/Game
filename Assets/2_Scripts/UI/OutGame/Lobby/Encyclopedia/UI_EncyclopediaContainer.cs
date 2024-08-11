using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby.Encyclopedia
{
    public class UI_EncyclopediaContainer : SerializedMonoBehaviour
    {
        [SerializeField]
        private GameObject mItemPrefab;
        [SerializeField] 
        private GameObject mContent;
        [SerializeField]
        private EEnchantClassType mEnchantClassType;

        public void Start()
        {
            List<CharacterData> characterDataList = new List<CharacterData>();
            GameManager.Instance.UserCharacterList.ForEach(characterData =>
            {
                var data = characterData.CharacterEvolutions[3].GetData;
                if (data.GetCharacterClass() == mEnchantClassType)
                {
                    characterDataList.Add(characterData.CharacterEvolutions[3].GetData);
                }
            });
            mContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, characterDataList.Count * 440);
            foreach (var characterData in characterDataList)
            {
                var itemObject = Instantiate(mItemPrefab, mContent.transform).GetComponent<UI_EncyclopediaItem>();
                itemObject.SetItem(characterData, mEnchantClassType);
            }
        }
    }
}