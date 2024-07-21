using _2_Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharacter : MonoBehaviour, IPointerUpHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private RectTransform mRectTransform;

        private Vector2 mSizeDelta;

        [SerializeField]
        private Material mMaterial;
        [SerializeField]
        private TextMeshProUGUI mCoolTimeText;

        private GameMessage<Vector2> mWorldPosMessage;
        private GameMessage<bool> mOnImageMessage;

        private GameMessage<Vector2> mUseMessage;
        private GameMessage<bool> mPointerUpMessage;

        private void Awake()
        {
            mRectTransform = GetComponent<RectTransform>();
            mMaterial = GetComponent<Image>().material;
            mSizeDelta = mRectTransform.sizeDelta;
            SetCoolTime(0);
            mWorldPosMessage = new GameMessage<Vector2>(EGameMessage.MainCharacterSkillDuring, Vector2.zero);
            mOnImageMessage = new GameMessage<bool>(EGameMessage.MainCharacterSkillDuring, false);
            mUseMessage = new GameMessage<Vector2>(EGameMessage.MainCharacterSkillUse, Vector2.zero);
            mPointerUpMessage = new GameMessage<bool>(EGameMessage.MainCharacterSkillUse, false);
            MessageBroker.Default.Receive<GameMessage<float>>()
                .Where(message => message.Message == EGameMessage.MainCharacterCoolTime)
                .Subscribe(message =>
                {
                    SetCoolTime(message.Value);
                }).AddTo(this);
        }

        public void Init(float coolTime)
        {
            mMaterial.SetFloat("_CoolTime", coolTime);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 imagePos = UICamera.Instance.Camera.WorldToScreenPoint(mRectTransform.position);
            Vector2 mousePos = eventData.position;

            mOnImageMessage.SetValue(IsMouseOnImage(imagePos, mousePos));
            MessageBroker.Default.Publish(mOnImageMessage);

            mWorldPosMessage.SetValue(Camera.main.ScreenToWorldPoint(eventData.position));
            MessageBroker.Default.Publish(mWorldPosMessage);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            mPointerUpMessage.SetValue(true);
            MessageBroker.Default.Publish(mPointerUpMessage);
        }

        private void SetCoolTime(float time)
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
            mUseMessage.SetValue(Camera.main.ScreenToWorldPoint(eventData.position));
            MessageBroker.Default.Publish(mUseMessage);
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }
    }
}