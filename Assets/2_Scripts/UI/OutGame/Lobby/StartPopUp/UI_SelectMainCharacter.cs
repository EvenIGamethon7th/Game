using _2_Scripts.Game.ScriptableObject.Character;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.UI.Ingame;
using _2_Scripts.UI.OutGame.Lobby.StartPopUp;
using Cargold.FrameWork.BackEnd;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_SelectMainCharacter : MonoBehaviour
    {
        [SerializeField]
        private Image mSelectCharacterImage;

        [SerializeField]
        private TextMeshProUGUI mCharacterName;

        [SerializeField] private GameObject mMainCharacterSlot;
        private void Awake()
        {
            foreach (var mainCharacter in GameManager.Instance.MainCharacterList)
            {
                var slot = Instantiate(mMainCharacterSlot, transform).GetComponent<UI_MainCharacterSlot>();
                slot.Init(mainCharacter,SelectButton);
            }

           var data = BackEndManager.Instance.UserMainCharacterData.Values.FirstOrDefault(data => data.isEquip);
           if (data != null)
           {
               SelectButton(data);
           }
        }

        private void SelectButton(MainCharacterData data)
        {
            GameManager.Instance.SetCurrentMainCharacter(data.key);
            mSelectCharacterImage.sprite = GameManager.Instance.CurrentMainCharacter.CharacterEvolutions[data.rank].GetData.Icon;
            mCharacterName.text = GameManager.Instance.CurrentMainCharacter.CharacterEvolutions[data.rank].GetData.GetCharacterName();
        }
    }
}