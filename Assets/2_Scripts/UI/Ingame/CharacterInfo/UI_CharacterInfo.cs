using System;
using _2_Scripts.Game.Unit;
using _2_Scripts.Utils;
using Spine.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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
        
        
        [SerializeField] private TextMeshProUGUI mCharacterSkillNameText;
        [SerializeField] private TextMeshProUGUI mCharacterSkillTypeText;
        [SerializeField] private TextMeshProUGUI mCharacterSkillDescText;
        
        [SerializeField] private Button mCharacterRareSkillButton;
        [SerializeField] private Button mCharacterEpicSkillButton;
        [SerializeField] private Sprite mBlinkImage;
        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<UnitGroup>>()
                .Where(message => message.Message == EGameMessage.SelectCharacter)
                .Subscribe(data =>
                {
                    if (data.Value == null)
                    { 
                        mCharacterInfoGo.SetActive(false);
                        return;
                    }
                    mCharacterInfoGo.SetActive(true);
                  UpdateCharacterInfoData(data.Value.GetCharacterData());
                }).AddTo(this);
            mCharacterRareSkillButton.onClick.AddListener(() =>
            {
                if (mCharacterData.rank < 2)
                    return;
                SetSkillDescText(mCharacterData,1);
            });
            mCharacterEpicSkillButton.onClick.AddListener(() =>
            {
                if (mCharacterData.rank < 3)
                    return;
                SetSkillDescText(mCharacterData,2);
            });
        }

        private CharacterData mCharacterData;
        private void UpdateCharacterInfoData(CharacterData data)
        {
            mCharacterRareSkillButton.GetComponent<Image>().sprite = data.GetSkillIconOrNull(data.Skill1) != null ? data.GetSkillIconOrNull(data.Skill1) : mBlinkImage;
            mCharacterEpicSkillButton.GetComponent<Image>().sprite = data.GetSkillIconOrNull(data.Skill2) != null ? data.GetSkillIconOrNull(data.Skill2) : mBlinkImage;
            mCharacterNameText.text = data.GetCharacterName();
            mCharacterStatusAtkText.text = $"{data.GetTotalAtk()}";
            mCharacterStatusAtkSpeedText.text = $"{data.GetTotalAtkSpeed()}";
            mCharacterStatusMAtkText.text = $"{data.GetTotalMAtk()}";
            mCharacterClassText.text = $"{data.GetCharacterClassName()}";
            mCharacterData = data;
            if (data.rank > 1)
            {
                SetSkillDescText(data,1);
            }
            else
            {
                mCharacterSkillNameText.text = "";
                mCharacterSkillTypeText.text = "";
                mCharacterSkillDescText.text = "스킬 미 획득";
            }
            global::Utils.CharacterSkeletonInit(modelGraphic,data.characterPack);
        }

        private void SetSkillDescText(CharacterData data,int rank)
        {
            data.SetSkillDataLoc(rank);
            mCharacterSkillNameText.text = data.SkillName;
            mCharacterSkillTypeText.text = data.SkillType;
            mCharacterSkillDescText.text = data.SkillDesc;
        }
        
    }
}