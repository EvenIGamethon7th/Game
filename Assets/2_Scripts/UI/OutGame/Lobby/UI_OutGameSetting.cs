using _2_Scripts.Game.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_OutGameSetting : MonoBehaviour
    {
        [SerializeField]
        private Button mExitButton;

        [SerializeField]
        private Slider mEffectSlider;

        [SerializeField]
        private Slider mBGMSlider;

        [SerializeField]
        private Toggle mVibe;

        private void Start()
        {
            mEffectSlider.onValueChanged.AddListener((value) => SetSound(value, ESound.Effect));
            mBGMSlider.onValueChanged.AddListener((value) => SetSound(value, ESound.Bgm));
            mVibe.onValueChanged.AddListener(SetVibe);
            mExitButton.onClick.AddListener(ExitSetting);
        }

        private void SetSound(float vol, ESound type)
        {
            SoundManager.Instance.SetVolumeTemporary(vol, type);
        }

        private void SetVibe(bool vibe)
        {
            SoundManager.Instance.SetBoolTemporary(vibe, ESettingBoolType.Vibe);
        }

        private void ExitSetting()
        {
            gameObject.SetActive(false);
            SoundManager.Instance.SaveSound();
        }

        private void OnDestroy()
        {
            mEffectSlider.onValueChanged.RemoveAllListeners();
            mBGMSlider.onValueChanged.RemoveAllListeners();
            mVibe.onValueChanged.RemoveAllListeners();
            mExitButton.onClick.RemoveAllListeners();
        }
    }
}