using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using UnityEngine.UI;
using System;
using static Cargold.Loading.UI_Loading_Transition_Script;
#if DoTween_C
using DG.Tweening;
#endif

namespace Cargold.UI
{
    public class UI_Tiling_Script : MonoBehaviour, IDirectionComponent
    {
        [LabelText("타일링 이미지"), SerializeField, OnValueChanged("CallEdit_ImgChanged_Func")] private Image targetImg;
        [LabelText("타일링 속도"), SerializeField] private Vector2 tilingSpeed = Vector2.one;
        [LabelText("페이드인 시간"), SerializeField] private float enterTime = 1f;
        [LabelText("페이드아웃 시간"), SerializeField] private float exitTime = 1f;
        [LabelText("알파값"), ReadOnly, SerializeField] private float alphaValue;
        [LabelText("타일링 Rtrf"), ReadOnly, SerializeField] private RectTransform targetRtrf;
        [LabelText("타일링 사이즈"), ReadOnly, SerializeField] private Vector2 imgSize;
        private Action deactiveDoneDel;

        private CoroutineData onTilingCorData;

        public void Init_Func()
        {
            if (this.targetImg is null == true)
                return;

            if (this.targetImg.sprite is null == true)
                return;

            this.imgSize = this.targetImg.sprite.rect.size / this.targetImg.pixelsPerUnitMultiplier;

            this.alphaValue = this.targetImg.color.a;

            RectTransform _rtrf = this.targetImg.rectTransform;
            _rtrf.anchorMin = Vector2.one * -2f;
            _rtrf.anchorMax = Vector2.one * 3f;
            _rtrf.anchoredPosition = Vector2.zero;

            this.targetRtrf = _rtrf;
        }

        [Button]
        public void Activate_Func(Action _doneDel = null)
        {
            this.targetRtrf.anchoredPosition = Vector2.zero;

            this.onTilingCorData.StartCoroutine_Func(OnTiling_Cor());

            if (0f < this.enterTime)
            {
#if DoTween_C
                this.targetImg.DOFade(this.alphaValue, this.enterTime).SetEase(Ease.Linear);
#endif
            }

            if(_doneDel != null)
                Coroutine_C.Invoke_Func(_doneDel);
        }
        private IEnumerator OnTiling_Cor()
        {
            Vector2 _resetPos = this.imgSize;

            while (true)
            {
                Vector2 _tilingValue = this.tilingSpeed * Time.deltaTime;
                _resetPos = this.SetTiling_Func(_resetPos, _tilingValue);

                yield return null;
            }
        }

        private Vector2 SetTiling_Func(Vector2 _resetPos, Vector2 _tilingValue)
        {
            this.targetRtrf.anchoredPosition += _tilingValue;

            _resetPos -= new Vector2(Mathf.Abs(_tilingValue.x), Mathf.Abs(_tilingValue.y));

            if (_resetPos.x <= 0f)
            {
                this.targetRtrf.anchoredPosition = new Vector2(0f, this.targetRtrf.anchoredPosition.y);

                _resetPos.x = this.imgSize.x;
            }

            if (_resetPos.y <= 0f)
            {
                this.targetRtrf.anchoredPosition = new Vector2(this.targetRtrf.anchoredPosition.x, 0f);

                _resetPos.y = this.imgSize.y;
            }

            return _resetPos;
        }

        public bool IsActivate_Func()
        {
            return false;
        }

        public void Deactivate_Func(Action _doneDel)
        {
            this.deactiveDoneDel = _doneDel;

            if (this.exitTime <= 0f)
            {
                this.DeactivateDone_Func();
            }
            else
            {
#if DoTween_C
                this.targetImg.DOFade(0f, this.exitTime).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        this.DeactivateDone_Func();
                    });
#endif
            }
        }
        public void DeactivateDone_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }

            this.onTilingCorData.StopCorountine_Func();

            this.deactiveDoneDel?.Invoke();
        }

#if UNITY_EDITOR
        private void CallEdit_ImgChanged_Func()
        {
            this.Init_Func();
        }
        [ShowInInspector, FoldoutGroup("Editor"), LabelText("흔들어보세용"), OnValueChanged("CallEdit_Func")] private float asd;
        [ShowInInspector, FoldoutGroup("Editor"), LabelText("초기화 Pos"), ReadOnly] private Vector2 editResetPos;
        private void CallEdit_Func()
        {
            if (this.editResetPos == default)
            {
                this.targetRtrf.anchoredPosition = Vector2.zero;
                this.editResetPos = this.targetImg.sprite.rect.size / this.targetImg.pixelsPerUnitMultiplier;
            }

            Vector2 _tilingValue = this.tilingSpeed;
            this.editResetPos = this.SetTiling_Func(this.editResetPos, _tilingValue);
        }
#endif
    }
}