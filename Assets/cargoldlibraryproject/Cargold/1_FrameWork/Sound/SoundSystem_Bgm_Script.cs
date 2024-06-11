using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Cargold.FrameWork.Sound_C;

namespace Cargold.FrameWork
{
    public class SoundSystem_Bgm_Script : SoundSystem_Script
    {
        private BgmType bgmType;
        private float volume;
        private CoroutineData fadeCorData;

        public BgmType GetBgmType => this.bgmType;

        public void Activate_Func(BgmData _bgmData, bool _isLooping = true)
        {
            this.bgmType = _bgmData.bgmType;
            this.volume = _bgmData.volume;

            base.audioSource.clip = _bgmData.clip;
            base.audioSource.loop = _isLooping;

            this.Activate_Func();
        }
        public void Activate_Func()
        {
            base.audioSource.Play();

            this.fadeCorData.StartCoroutine_Func(this.FadeIn_Cor(this.volume));
        }
        private IEnumerator FadeIn_Cor(float _originVolume)
        {
            base.audioSource.volume = 0f;

            while (base.audioSource.volume < _originVolume)
            {
                base.audioSource.volume += (Time.deltaTime * DataBase_Manager.Instance.GetSound_C.bgmFadeSpeed);

                yield return null;
            }

            base.audioSource.volume = _originVolume;

            this.fadeCorData.StopCorountine_Func();

            yield break;
        }

        public override void Deactivate_Func(bool _isInit = false)
        {
            if(_isInit == false)
            {
                this.fadeCorData.StopCorountine_Func();

                this.fadeCorData.StartCoroutine_Func(this.FadeOut_Cor());
            }
        }
        private IEnumerator FadeOut_Cor()
        {
            while(0f < base.audioSource.volume)
            {
                float _value = Time.deltaTime * DataBase_Manager.Instance.GetSound_C.bgmFadeSpeed;
                base.audioSource.volume -= _value;

                yield return null;
            }

            base.audioSource.volume = 0f;

            this.fadeCorData.StopCorountine_Func();

            yield break;
        }
    }
}
