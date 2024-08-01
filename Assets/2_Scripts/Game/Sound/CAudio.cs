using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace _2_Scripts.Game.Sound
{
    public enum ESound
    {
        Effect,
        Bgm
    }

    public class CAudio : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField]
        private ESound _type;

        private void InitVolume(Scene prev, Scene next)
        {
            if (_audioSource != null)
                switch (_type)
                {
                    case ESound.Effect:
                        //º¼·ý ÃÊ±âÈ­
                        SetVolume(SoundManager.Instance.EffectVolume);
                        break;

                    case ESound.Bgm:
                        //º¼·ý ÃÊ±âÈ­
                        SetVolume(SoundManager.Instance.BGMVolume);
                        break;
                }
            else
                Clear();
        }

        private void SetVolume(float value)
        {
            _audioSource.volume = value;
            _audioSource.pitch = 1;
        }

        public void SetPitch(float value)
        {
            _audioSource.pitch = value;
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            switch (_type)
            {
                case ESound.Effect:
                    SoundManager.Instance.EffectAction -= SetVolume;
                    SoundManager.Instance.EffectAction += SetVolume;
                    SetVolume(SoundManager.Instance.EffectVolume);
                    break;

                case ESound.Bgm:
                    SoundManager.Instance.BGMAction -= SetVolume;
                    SoundManager.Instance.BGMAction += SetVolume;
                    SetVolume(SoundManager.Instance.BGMVolume);
                    break;
            }

            SceneManager.activeSceneChanged -= InitVolume;
            SceneManager.activeSceneChanged += InitVolume;
            PlaySound(_audioSource.clip);
        }

        public void PlaySound(AudioClip clip, float pitch = 1f)
        {
            if (clip != null)
            {
                PlaySound(clip, _type, pitch);
            }

            else
            {
                SetPitch(pitch);
            }
        }

        public void PlaySound(AudioClip clip, ESound type, float pitch = 1f)
        {

            if (_audioSource.isPlaying)
            {
                if (_audioSource.clip != clip)
                    _audioSource.Stop();
                else
                    return;
            }

            _audioSource.pitch = pitch;

            if (type == ESound.Bgm)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }

            else
                _audioSource.PlayOneShot(clip);
        }

        public void StopSoundFade(float time = 1.5f)
        {
            _audioSource.DOFade(0, time);
        }

        public void StopSound()
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();
        }

        public void Clear()
        {
            SceneManager.activeSceneChanged -= InitVolume;
            SoundManager.Instance.EffectAction -= SetVolume;
            SoundManager.Instance.BGMAction -= SetVolume;
            _audioSource = null;
        }
    }
}