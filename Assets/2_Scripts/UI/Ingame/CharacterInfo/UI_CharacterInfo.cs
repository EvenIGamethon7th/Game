using System;
using _2_Scripts.Utils;
using Spine.Unity;
using TMPro;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.Ingame.CharacterInfo
{
    public class UI_CharacterInfo : MonoBehaviour
    {
        [SerializeField] private GameObject mCharacterInfoGo;
        [SerializeField] private TextMeshProUGUI mCharacterNameText;
        [SerializeField] private TextMeshProUGUI mCharacterStatusAtkText;
        [SerializeField] private TextMeshProUGUI mCharacterStatusAtkSpeedText;
        [SerializeField] private TextMeshProUGUI mCharacterStatusMAtkText;
        [SerializeField] private TextMeshProUGUI mCharacterClassText;
        [SerializeField] private SkeletonGraphic modelGraphic;
        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<CharacterData>>()
                .Where(message => message.Message == EGameMessage.SelectCharacter)
                .Subscribe(data =>
                {
                    if (data.Value == null)
                    { 
                        mCharacterInfoGo.SetActive(false);
                        return;
                    }
                    mCharacterInfoGo.SetActive(true);
                  UpdateCharacterInfoData(data.Value);
                }).AddTo(this);
        }

        private void UpdateCharacterInfoData(CharacterData data)
        {
            mCharacterNameText.text = data.GetCharacterName();
            mCharacterStatusAtkText.text = $"{data.GetTotalAtk()}";
            mCharacterStatusAtkSpeedText.text = $"{data.GetTotalAtkSpeed()}";
            mCharacterStatusMAtkText.text = $"{data.GetTotalMAtk()}";
            global::Utils.CharacterSkeletonInit(modelGraphic,data.characterPack);
        }
    }
}