using System;
using System.Collections.Generic;
using _2_Scripts.Game.Unit;
using Cargold;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

namespace _2_Scripts.UI
{
    public class UI_SummonButton : SerializedMonoBehaviour
    {
        [Flags]
        public enum ESummonButtonState
        {
            Selected,
            Disable
        }
        
        [SerializeField]
        private LocalizeText mCharacterName;
        [SerializeField]
        private TextMeshProUGUI mCharacterCost;
        
        [SerializeField]
        private SkeletonGraphic mCharacterGraphic;
        
        [SerializeField]
        private Dictionary<ESummonButtonState,GameObject> mViewList = new();

        private ESummonButtonState mCurrentSummonButtonState = ESummonButtonState.Selected;


        private CharacterData mCharacterData;

        [SerializeField]
        private Button mButtom;

        private void Start()
        {
            UpdateCharacter();
        }

        public void OnSummonButton()
        {
            if (mCurrentSummonButtonState == ESummonButtonState.Disable)
            {
                return;
            }
            
            if(GameManager.Instance.UserGold < mCharacterData.cost)
            {
                // 차후 Localize로 변경
                UI_Toast_Manager.Instance.Activate_WithContent_Func("돈이 부족합니다");
                return;
            }

            //TODO 돈 뺴는거 넣어야 함
            var isCreateUnit = MapManager.Instance.CreateUnit(mCharacterData);
            if (isCreateUnit)
            {
                GameManager.Instance.UpdateGold(-mCharacterData.cost);
                mCurrentSummonButtonState = ESummonButtonState.Disable;
                Tween_C.OnPunch_Func(this.transform);
                ShowChange();
            }
        }

        public void Reroll()
        {
            mCurrentSummonButtonState = ESummonButtonState.Selected;
            UpdateCharacter();
            ShowChange();
        }

        private void ShowChange()
        {
            mViewList[mCurrentSummonButtonState].SetActive(true);
            mViewList[mCurrentSummonButtonState ^ ESummonButtonState.Disable].SetActive(false);
        }
        
        public void UpdateCharacter()
        {
           CharacterInfo characterInfo = GameManager.Instance.RandomCharacterCardOrNull();
           mCharacterData = GameManager.Instance.GetRandomCharacterData(characterInfo);
           
           mCharacterGraphic.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{mCharacterData.characterPack}_{ELabelNames.SkeletonData}");
           mCharacterGraphic.material = ResourceManager.Instance.Load<Material>($"{mCharacterData.characterPack}_{ELabelNames.UIMaterial}");
           string skinName = mCharacterGraphic.skeletonDataAsset.name;
           mCharacterGraphic.initialSkinName = skinName.Substring(0, skinName.LastIndexOf('_'));

           mCharacterGraphic.Initialize(true);
           mCharacterName.SetLocalizeKey(mCharacterData.nameKey);
           mCharacterCost.text = $"{mCharacterData.cost}$";
        }
        
    }
}