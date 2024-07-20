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
        [SerializeField]
        private TextMeshProUGUI mName;

        public void SetStatus(CUnit data)
        {
            mName.text = data.CharacterDatas.GetCharacterName();
            mTexts[0].text = $"{(int)data.CharacterDatas.alumniAtk}";
            mTexts[1].text = data.CharacterDatas.alumniAtkSpeed.ToString("F1");
            mTexts[2].text = $"{(int)data.CharacterDatas.alumniMatk}";

            mSliders[0].value = data.CharacterDatas.alumniAtk;
            mSliders[1].value = data.CharacterDatas.alumniAtkSpeed;
            mSliders[2].value = data.CharacterDatas.alumniMatk;
            UpdateGraphic(data);
        }

        public void SetStatus(CharacterData data)
        {
            mTexts[0].text = $"{(int)data.alumniAtk}";
            mTexts[1].text = data.alumniAtkSpeed.ToString("F1");
            mTexts[2].text = $"{(int)data.alumniMatk}";

            mSliders[0].value = data.alumniAtk;
            mSliders[1].value = data.alumniAtkSpeed;
            mSliders[2].value = data.alumniMatk;
        }

        private void UpdateGraphic(CUnit data)
        {
            mGraphic.gameObject.SetActive(true);
            mGraphic.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{data.CharacterDatas.characterPack}_{ELabelNames.SkeletonData}");

            mGraphic.material = ResourceManager.Instance.Load<Material>($"{data.CharacterDatas.characterPack}_{ELabelNames.UIMaterial}");
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
            mName.text = "Empty";

            mGraphic.gameObject.SetActive(false);
        }
    }
}