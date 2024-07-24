using _2_Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharacterButton : MonoBehaviour, IPointerUpHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private RectTransform mRectTransform;

        private Vector2 mSizeDelta;

        [SerializeField]
        private Material mMaterial;

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

        public void Init(Sprite sprite, float coolTime)
        {
            mMaterial.SetTexture("_MainTex", sprite.texture);
            mMaterial.SetFloat("_CoolTime", coolTime);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 imagePos = UICamera.Instance.Camera.WorldToScreenPoint(mRectTransform.position);
            Vector2 mousePos = eventData.position;
            if (!IsMouseOnImage(imagePos, mousePos))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(eventData.position), Vector2.zero);
            }

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