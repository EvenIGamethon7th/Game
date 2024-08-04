using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharcterInfoBubble : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI mCharacterNameText;

        [SerializeField]
        private TextMeshProUGUI mSkillNameText;

        [SerializeField]
        private TextMeshProUGUI mLevelText;

        [SerializeField]
        private TextMeshProUGUI mDescriptionText;

        private RectTransform mRectTransform;

        private Image mImage;

        public void Init(CharacterData data, Sprite sprite)
        {
            mImage = GetComponent<Image>();
            mRectTransform = GetComponent<RectTransform>();

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonUp(0) && gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    if (!global::Utils.IsPosOnUI(mRectTransform, Input.mousePosition, CameraManager.Instance.WorldToScreenPoint(mRectTransform.position, CameraManager.ECameraType.UI)))
                    {
                        gameObject.SetActive(false);
                    }
                }).AddTo(this);

            if (data != null)
            {
                mCharacterNameText.text = data.GetCharacterName();
                var skillData = DataBase_Manager.Instance.GetSkill.GetData_Func(data.Skill1);
                mSkillNameText.text = skillData.Name;
                mLevelText.text = $"LV. {data.rank}";
                mDescriptionText.text = skillData.Description;
                mImage.sprite = sprite;
            }
        }
    }
}