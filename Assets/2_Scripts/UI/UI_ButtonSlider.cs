using _2_Scripts.Game.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_ButtonSlider : Slider
    {
        private bool mIsEnabled;
        private ESettingBoolType mType;
        public event Action<bool, ESettingBoolType> OnClick;

        public bool InitType(ESettingBoolType type)
        {
            mType = type;
            switch (type)
            {
                case ESettingBoolType.Vibe:
                    mIsEnabled = SoundManager.Instance.IsUseVibe;
                    break;

                case ESettingBoolType.Sound:
                    mIsEnabled = SoundManager.Instance.IsUseSound;
                    break;
            }
            value = mIsEnabled ? maxValue : minValue;
            return mIsEnabled;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            mIsEnabled = !mIsEnabled;
            value = mIsEnabled ? maxValue : minValue;
            OnClick?.Invoke(mIsEnabled, mType);
        }
    }
}