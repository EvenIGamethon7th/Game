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
            mTexts[0].text = $"{(int)data.CharacterDatas.atk} + {(int)data.CharacterDatas.alumniAtk}";
            mTexts[1].text = $"{data.CharacterDatas.atkSpeed.ToString("F1")} + {data.CharacterDatas.alumniAtkSpeed.ToString("F1")}";
            mTexts[2].text = $"{(int)data.CharacterDatas.matk} + {(int)data.CharacterDatas.alumniMatk}";

            mSliders[0].value = data.CharacterDatas.atk + data.CharacterDatas.alumniAtk;
            mSliders[1].value = data.CharacterDatas.atkSpeed + data.CharacterDatas.alumniAtkSpeed;
            mSliders[2].value = data.CharacterDatas.matk + data.CharacterDatas.alumniMatk;
            UpdateGraphic(data);
        }

        public void SetStatus(CharacterData data)
        {
            mTexts[0].text = $"{(int)data.atk} + {(int)data.alumniAtk}";
            mTexts[1].text = $"{data.atkSpeed.ToString("F1")} + {data.alumniAtkSpeed.ToString("F1")}";
            mTexts[2].text = $"{(int)data.matk} + {(int)data.alumniMatk}";

            mSliders[0].value = data.atk + data.alumniAtk;
            mSliders[1].value = data.atkSpeed + data.alumniAtkSpeed;
            mSliders[2].value = data.matk + data.alumniMatk;
        }

        private void UpdateGraphic(CUnit data)
        {
            mGraphic.gameObject.SetActive(true);
            mGraphic.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{data.CharacterDatas.characterPack}_{ELabelNames.SkeletonData}");

            mGraphic.material = ResourceManager.Instance.Load<Material>($"{data.CharacterDatas.characterPack}_{ELabelNames.Material}");
            string skinName = mGraphic.skeletonDataAsset.name;
            mGraphic.initialSkinName = skinName.Substring(0, skinName.LastIndexOf('_'));

            mGraphic.Initialize(true);
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