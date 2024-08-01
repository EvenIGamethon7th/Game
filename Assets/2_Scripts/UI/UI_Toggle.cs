using _2_Scripts.Game.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_Toggle : MonoBehaviour
    {
        UI_ButtonSlider mButton;

        [SerializeField]
        private Sprite[] mSprites;
        [SerializeField]
        private Image mImage;

        [SerializeField]
        private ESettingBoolType mSetting;

        void Start()
        {
            mButton = GetComponentInChildren<UI_ButtonSlider>();
            bool active = mButton.InitType(mSetting);
            if (active)
            {
                mImage.sprite = mSprites[0];
            }

            else
            {
                mImage.sprite = mSprites[1];
            }

            mButton.OnClick += SetToggle;
        }

        private void SetToggle(bool active, ESettingBoolType type)
        {
            SoundManager.Instance.SetBoolTemporary(active, type);

            if (active)
            {
                mImage.sprite = mSprites[0];
            }

            else
            {
                mImage.sprite = mSprites[1];
            }
        }

        private void OnDestroy()
        {
            mButton.OnClick -= SetToggle;
        }
    }
}