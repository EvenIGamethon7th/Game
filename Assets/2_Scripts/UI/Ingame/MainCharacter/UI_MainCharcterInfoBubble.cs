using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


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

        public void Init(CharacterData data)
        {
            mRectTransform = GetComponent<RectTransform>();

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonDown(0) && gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    if (!global::Utils.IsPosOnUI(mRectTransform, Input.mousePosition, UICamera.Instance.Camera.WorldToScreenPoint(mRectTransform.position)))
                    {
                        gameObject.SetActive(false);
                    }
                }).AddTo(this);
        }
    }
}