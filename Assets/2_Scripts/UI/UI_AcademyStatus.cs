using _2_Scripts.Game.Unit;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_AcademyStatus : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI[] mTexts;
        [SerializeField]
        private Slider[] mSliders;
        [SerializeField]
        private SkeletonGraphic mGraphic;

        public void SetStatus(CUnit data)
        {
            mTexts[0].text = ((int)data.CharacterDatas.atk).ToString();
            mTexts[1].text = ((int)data.CharacterDatas.atkSpeed).ToString();
            mTexts[2].text = ((int)data.CharacterDatas.matk).ToString();

            mSliders[0].value = data.CharacterDatas.atk;
            mSliders[1].value = data.CharacterDatas.atkSpeed;
            mSliders[2].value = data.CharacterDatas.matk;
        }

        private void UpdateGraphic(EUnitClass unitClass, EUnitRank unitRank)
        {
            mGraphic.gameObject.SetActive(true);
            mGraphic.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{unitClass}_{unitRank}_{ELabelNames.SkeletonData}");

            mGraphic.material = ResourceManager.Instance.Load<Material>($"{unitClass}_{unitRank}_{ELabelNames.Material}");
            string skinName = mGraphic.skeletonDataAsset.name;
            mGraphic.initialSkinName = skinName.Substring(0, skinName.LastIndexOf('_'));

            mGraphic.Initialize(true);
            mGraphic.AnimationState.SetAnimation(0, "Idle_1", true);
        }

        public void Clear()
        {
            for (int i = 0; i < mTexts.Length; ++i)
            {
                mTexts[i].text = "0";
                mSliders[i].value = 0;
            }

            mGraphic.gameObject.SetActive(false);
        }
    }
}