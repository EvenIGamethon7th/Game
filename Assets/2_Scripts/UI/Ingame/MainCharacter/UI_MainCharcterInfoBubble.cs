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
        private Vector2 mSizeDelta;

        public void Init(CharacterData data)
        {
            mRectTransform = GetComponent<RectTransform>();
            mSizeDelta = mRectTransform.sizeDelta;

            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonDown(0) && gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    if (!IsMouseOnImage(Input.mousePosition, UICamera.Instance.Camera.WorldToScreenPoint(mRectTransform.position)))
                    {
                        gameObject.SetActive(false);
                    }
                });
        }

        private bool IsMouseOnImage(Vector2 imagePos, Vector2 mousePos)
        {
            if (imagePos.x - mSizeDelta.x > mousePos.x || imagePos.x + mSizeDelta.x < mousePos.x) return false;
            if (imagePos.y - mSizeDelta.y > mousePos.y || imagePos.y + mSizeDelta.y < mousePos.y) return false;

            return true;
        }
    }
}