using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharacter : MonoBehaviour, IPointerUpHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        public event Action<Vector2> Drag;
        public event Action<Vector2> EndDrag;
        public event Action PointerUp;
        public event Action<bool> OnImage;
        private RectTransform mRectTransform;

        private Vector2 mSizeDelta;

        [SerializeField]
        private Material mMaterial;
        [SerializeField]
        private TextMeshProUGUI mCoolTimeText;

        private void Awake()
        {
            mRectTransform = GetComponent<RectTransform>();
            mMaterial = GetComponent<Image>().material;
            mSizeDelta = mRectTransform.sizeDelta;
            SetCoolTime(0);
        }

        public void Init(float coolTime)
        {
            mMaterial.SetFloat("_CoolTime", coolTime);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 imagePos = UICamera.Instance.Camera.WorldToScreenPoint(mRectTransform.position);
            Vector2 mousePos = eventData.position;

            OnImage?.Invoke(IsMouseOnImage(imagePos, mousePos));
            Drag?.Invoke(Camera.main.ScreenToWorldPoint(eventData.position));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke();
        }

        public void SetCoolTime(float time)
        {
            mMaterial.SetFloat("_CurrentCoolTime", time);

            if (time <= 0)
            {
                mCoolTimeText.text = $"사용 가능";
            }

            else if (time < 1)
            {
                mCoolTimeText.text = $"{time.ToString("F1")}초";
            }

            else
            {
                mCoolTimeText.text = $"{(int)time}초";
            }
        }

        private bool IsMouseOnImage(Vector2 imagePos, Vector2 mousePos)
        {
            if (imagePos.x - mSizeDelta.x > mousePos.x || imagePos.x + mSizeDelta.x < mousePos.x) return false;
            if (imagePos.y - mSizeDelta.y > mousePos.y || imagePos.y + mSizeDelta.y < mousePos.y) return false;

            return true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDrag?.Invoke(Camera.main.ScreenToWorldPoint(eventData.position));
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }
    }
}