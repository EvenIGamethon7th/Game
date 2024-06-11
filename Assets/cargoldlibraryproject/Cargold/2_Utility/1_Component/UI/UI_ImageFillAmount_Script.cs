using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using UnityEngine.UI;
#if DoTween_C
using DG.Tweening; 
#endif

namespace Cargold.UI
{
#if DoTween_C
    public class UI_ImageFillAmount_Script : MonoBehaviour
    {
        [LabelText("FillAmount"), SerializeField, OnValueChanged("CallEdit_Func"), PropertyRange(0d, 1d)] private float fillAmount = 1f;
        [FoldoutGroup("캐싱"), ReadOnly, SerializeField] private Image thisImg = null;
        [FoldoutGroup("캐싱"), ReadOnly, SerializeField] private RectTransform thisRtrf = null;
        [FoldoutGroup("캐싱"), ReadOnly, SerializeField] private RectTransform parentRtrf = null;

        public Image GetImg => this.thisImg;
        public RectTransform GetThisRtrf => this.thisRtrf;

        [FoldoutGroup("캐싱"), Button("캐싱 ㄱㄱ~")]
        public void Init_Func()
        {
            if (this.TryGetComponent(out RectTransform _rtrf) == true)
            {
                this.thisRtrf = _rtrf;

                _rtrf.anchorMin = new Vector2(0f, _rtrf.anchorMin.y);
                _rtrf.anchorMax = new Vector2(0f, _rtrf.anchorMax.y);
                _rtrf.pivot = new Vector2(0f, _rtrf.pivot.y);

                this.parentRtrf = _rtrf.parent as RectTransform;

                if (this.TryGetComponent(out Image _img) == true)
                {
                    this.thisImg = _img;

                    this.thisImg.type = Image.Type.Sliced;
                }
            }
        }

        public Tween OnFillAmount_Func(float _value, float _twnTime)
        {
            this.fillAmount = _value;

            float _maxSizeX = this.parentRtrf.sizeDelta.x;
            Vector2 _sizeValue = new Vector2(_maxSizeX * _value, this.thisRtrf.sizeDelta.y);
            return this.thisRtrf.DOSizeDelta(_sizeValue, _twnTime);
        }
        public void OnFillAmount_Func(float _value)
        {
            this.fillAmount = _value;

            this.OnFillAmountResult_Func(_value);
        }
        private void OnFillAmountResult_Func(float _value)
        {
            float _maxSizeX = this.parentRtrf.sizeDelta.x;
            this.thisRtrf.sizeDelta = new Vector2(_maxSizeX * _value, this.thisRtrf.sizeDelta.y);
        }

        private void CallEdit_Func()
        {
            this.OnFillAmountResult_Func(this.fillAmount);
        }

        private void Reset()
        {
            this.Init_Func();
        }
    } 
#endif
}