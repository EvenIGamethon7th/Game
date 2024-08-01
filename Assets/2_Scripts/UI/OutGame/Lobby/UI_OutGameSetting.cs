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

        private void Start()
        {
            mBGMSlider.value = SoundManager.Instance.BGMVolumeIgnorebool;
            mEffectSlider.value = SoundManager.Instance.EffectVolumeIgnorebool;

            mEffectSlider.onValueChanged.AddListener((value) => SetSound(value, ESound.Effect));
            mBGMSlider.onValueChanged.AddListener((value) => SetSound(value, ESound.Bgm));
            mExitButton.onClick.AddListener(ExitSetting);
        }

        private void SetSound(float vol, ESound type)
        {
            SoundManager.Instance.SetVolumeTemporary(vol, type);
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
            mExitButton.onClick.RemoveAllListeners();
        }
    }
}