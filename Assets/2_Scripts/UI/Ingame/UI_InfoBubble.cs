using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace _2_Scripts.UI.Ingame
{
    public class UI_InfoBubble : MonoBehaviour
    {
        private RectTransform mRectTransform;

        private void Awake()
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
            gameObject.SetActive(false);
        }
    }
}