using _2_Scripts.Game.Unit;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Summon
{
    public class UnitButton : Button
    {
        public TextMeshProUGUI Text { get; private set; }
        public SkeletonGraphic Graphic { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Text = GetComponentInChildren<TextMeshProUGUI>();
            Graphic = GetComponentInChildren<SkeletonGraphic>();
        }

        public void UpdateGraphic(EUnitClass unitClass, EUnitRank unitRank)
        {
            Graphic.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{unitClass}_{unitRank}_{ELabelNames.SkeletonData}");


            Graphic.material = ResourceManager.Instance.Load<Material>($"{unitClass}_{unitRank}_{ELabelNames.Material}");
            string skinName = Graphic.skeletonDataAsset.name;
            Graphic.initialSkinName = skinName.Substring(0, skinName.LastIndexOf('_'));

            Graphic.Initialize(true);
            Graphic.AnimationState.SetAnimation(0, "Idle_1", true);
        }
    }
}