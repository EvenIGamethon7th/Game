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
        public TextMeshProUGUI Text { get; private set; }
        private SkeletonGraphic mGraphic;

        protected override void Awake()
        {
            base.Awake();
            Text = GetComponentInChildren<TextMeshProUGUI>();
            mGraphic = GetComponentInChildren<SkeletonGraphic>();
        }

        public void UpdateGraphic(EUnitClass unitClass, EUnitRank unitRank)
        {
            mGraphic.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{unitClass}_{unitRank}_{ELabelNames.SkeletonData}");

            mGraphic.material = ResourceManager.Instance.Load<Material>($"{unitClass}_{unitRank}_{ELabelNames.Material}");
            string skinName = mGraphic.skeletonDataAsset.name;
            mGraphic.initialSkinName = skinName.Substring(0, skinName.LastIndexOf('_'));

            mGraphic.Initialize(true);
            mGraphic.AnimationState.SetAnimation(0, "Idle_1", true);
        }
    }
}