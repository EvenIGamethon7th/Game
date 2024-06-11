using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
#if DoTween_C
using DG.Tweening; 
#endif

namespace Cargold.UI
{
#if DoTween_C
    #region BaseGaugeControl
    [System.Serializable]
    public abstract class BaseGaugeControl
    {
        [ReadOnly, ShowInInspector] private Tween gaugeTwn;

        public void OnFillAmount_Func(int _value, int _max, bool _isTwnZero = false, float _time = .25f, TwnType _twnType = TwnType.Twn, float _previewPer = -1f, bool _isTest = false)
        {
            this.OnFillAmount_Func((float)_value, (float)_max, _isTwnZero, _time, _twnType, _previewPer, _isTest);
        }
        public void OnFillAmount_Func(float _value, float _max, bool _isTwnZero = false, float _time = .25f, TwnType _twnType = TwnType.Twn, float _previewPer = -1f, bool _isTest = false)
        {
            float _per = _value / _max;
            this.OnFillAmount_Func(_per, _isTwnZero, _time, _twnType, _previewPer, _isTest);
        }
        public void OnFillAmount_Func(float _per, bool _isTwnZero = false, float _time = .25f, TwnType _twnType = TwnType.Twn, float _previewPer = -1f, bool _isTest = false)
        {
            _previewPer = _previewPer < 0f ? _per : _previewPer;
            this.OnPreviewGauge_Func(_previewPer);

            if (_twnType == TwnType.Twn)
            {
                if (this.gaugeTwn != null)
                {
                    this.gaugeTwn.Kill();
                    this.gaugeTwn = null;
                }

                if (0f < _per || _isTwnZero == true)
                {
                    this.gaugeTwn = this.OnFillAmount_Func(_per, _time);
                }
                else
                {
                    this.OnTwnGauge_Func(0f);
                }
            }
            else if (_twnType == TwnType.Immediately)
            {
                this.OnTwnGauge_Func(_per);
            }
        }
        protected abstract void OnPreviewGauge_Func(float _per);
        protected abstract Tween OnFillAmount_Func(float _per, float _time);
        protected abstract void OnTwnGauge_Func(float _per);

        public enum TwnType
        {
            Twn = 0,
            Immediately,
            None,
        }
    }
    #endregion
    #region GaugeControl_C
    [System.Serializable]
    public class GaugeControl_C : BaseGaugeControl
    {
        [LabelText("먼저 바뀌는 거"), SerializeField] private Image previewGaugeImg;
        [LabelText("나중에 바뀌는 거"), SerializeField] private Image twnGaugeImg;

        protected override void OnPreviewGauge_Func(float _per)
        {
            this.previewGaugeImg.fillAmount = _per;
        }
        protected override Tween OnFillAmount_Func(float _per, float _time)
        {
            return DOTweenModuleUI.DOFillAmount(this.twnGaugeImg, _per, _time);
        }
        protected override void OnTwnGauge_Func(float _per)
        {
            this.twnGaugeImg.fillAmount = _per;
        }
    }
    #endregion
    #region GaugeControl_Slice
    [System.Serializable]
    public class GaugeControl_Slice : BaseGaugeControl
    {
        [LabelText("미리보기 게이지"), SerializeField] private UI_ImageFillAmount_Script previewGaugeImg;
        [LabelText("트윈 게이지"), SerializeField] private UI_ImageFillAmount_Script twnGaugeImg;

        protected override void OnPreviewGauge_Func(float _per)
        {
            this.previewGaugeImg.OnFillAmount_Func(_per);
        }
        protected override Tween OnFillAmount_Func(float _per, float _time)
        {
            return this.twnGaugeImg.OnFillAmount_Func(_per, _time);
        }
        protected override void OnTwnGauge_Func(float _per)
        {
            this.twnGaugeImg.OnFillAmount_Func(_per);
        }
    }
    #endregion
    #region GaugeControl_SFI
    [System.Serializable]
    public class GaugeControl_SFI : BaseGaugeControl
    {
        [LabelText("먼저 바뀌는 거"), SerializeField] private SlicedFilledImage previewGaugeSFImg;
        [LabelText("나중에 바뀌는 거"), SerializeField] private SlicedFilledImage twnGaugeSFImg;

        public SlicedFilledImage GetTwnGaugeSFImg => this.twnGaugeSFImg;

        protected override void OnPreviewGauge_Func(float _per)
        {
            this.previewGaugeSFImg.fillAmount = _per;
        }
        protected override Tween OnFillAmount_Func(float _per, float _time)
        {
            return DOTween.To(() => this.twnGaugeSFImg.fillAmount, _v => this.twnGaugeSFImg.fillAmount = _v, _per, _time);
        }
        protected override void OnTwnGauge_Func(float _per)
        {
            this.twnGaugeSFImg.fillAmount = _per;
        }
    }
    #endregion
#endif
}