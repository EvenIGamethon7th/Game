using _2_Scripts.Game.Unit;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_UnitButton : Button
    {
        private SkeletonGraphic mGraphic;
        private TextMeshProUGUI mText;
        public string CharacterName { get; private set; }
        private CharacterData mData;

        protected override void Awake()
        {
            base.Awake();
            mGraphic = GetComponentInChildren<SkeletonGraphic>();
            mText = GetComponentInChildren<TextMeshProUGUI>();
            mGraphic.maskable = true;
        }

        public void UpdateGraphic(CharacterData data)
        {
            mData = data; 
            onClick.RemoveAllListeners();
            onClick.AddListener(CreateCharacter);

            mGraphic.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{data.characterPack}_{ELabelNames.SkeletonData}");

            mGraphic.material = ResourceManager.Instance.Load<Material>($"{data.characterPack}_{ELabelNames.UIMaterial}");
            string skinName = mGraphic.skeletonDataAsset.name;
            mGraphic.initialSkinName = skinName.Substring(0, skinName.LastIndexOf('_'));
            mText.text = mGraphic.initialSkinName;
            CharacterName = mGraphic.initialSkinName;
            mGraphic.Initialize(true);
            mGraphic.AnimationState.SetAnimation(0, "Idle_1", true);
        }

        private void CreateCharacter()
        {
            MapManager.Instance.CreateUnit(mData);
        }
    }
}