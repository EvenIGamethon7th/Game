using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _2_Scripts.Game.Sound
{
    public enum ESFX
    {

    }

    public enum ESettingBoolType
    {
        Vibe,
        Sound,
        Push
    }

    public class SoundManager : Singleton<SoundManager>
    {
        public struct SoundVolume
        {
            public float BGM;
            public float Effect;
            public bool IsUseSound;
            public bool IsUseVibe;

            public SoundVolume(float bgm = 0.5f, float effect = 0.5f, bool isUseSound = true, bool isUseVibe = true)
            {
                BGM = bgm;
                Effect = effect;
                IsUseSound = isUseSound;
                IsUseVibe = isUseVibe;
            }

            public SoundVolume(SoundVolume vol)
            {
                BGM = vol.BGM;
                Effect = vol.Effect;
                IsUseSound = vol.IsUseSound;
                IsUseVibe = vol.IsUseVibe;
            }

            public void Clear()
            {
                BGM = 0;
                Effect = 0;
            }
        }

        public float EffectVolume => _volume.IsUseSound ? _volume.Effect : 0;
        public float BGMVolume => _volume.IsUseSound ? _volume.BGM : 0;
        public float EffectVolumeIgnorebool => _volume.Effect;
        public float BGMVolumeIgnorebool => _volume.BGM;
        public bool IsUseVibe => _volume.IsUseVibe;
        public bool IsUseSound => _volume.IsUseSound;
        private SoundVolume _volume;
        private SoundVolume _tempVolume;

        protected override void ChangeSceneInit(Scene prev, Scene next)
        {
            
        }

        public Dictionary<ESFX, AudioClip> SFXs { get => _sfxs; }
        private Dictionary<ESFX, AudioClip> _sfxs = new Dictionary<ESFX, AudioClip>();

        public event Action<float> BGMAction;
        public event Action<float> EffectAction;

        public void Play2DSound(ESFX type)
        {
            AudioSource.PlayClipAtPoint(_sfxs[type], Vector3.zero, EffectVolume);
        }

        public void Play2DSound(AudioClip clip)
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, EffectVolume);
        }

        public void SaveSound(SoundVolume vol)
        {
            string voldata = JsonUtility.ToJson(vol);
            string path = Path.Combine(Application.persistentDataPath, "volume");
            File.WriteAllText(path, voldata);
            _volume = vol;
            if (_volume.IsUseSound)
            {
                BGMAction?.Invoke(_volume.BGM);
                EffectAction?.Invoke(_volume.Effect);
            }

            else
            {
                BGMAction?.Invoke(0);
                EffectAction?.Invoke(0);
            }

            _tempVolume = new SoundVolume(_volume);
        }

        public void SaveSound()
        {
            string voldata = JsonUtility.ToJson(_tempVolume);
            string path = Path.Combine(Application.persistentDataPath, "volume");
            File.WriteAllText(path, voldata);
            _volume = _tempVolume;
            if (_volume.IsUseSound)
            {
                BGMAction?.Invoke(_volume.BGM);
                EffectAction?.Invoke(_volume.Effect);
            }

            else
            {
                BGMAction?.Invoke(0);
                EffectAction?.Invoke(0);
            }
        }

        public void SetVolumeTemporary(float vol, ESound type)
        {
            if (type == ESound.Bgm)
            {
                _tempVolume.BGM = vol;
                BGMAction?.Invoke(vol);
            }
            else
            {
                _tempVolume.Effect = vol;
                EffectAction?.Invoke(vol);
            }
        }

        public void SetBoolTemporary(bool b, ESettingBoolType type)
        {
            if (type == ESettingBoolType.Sound)
            {
                _tempVolume.IsUseSound = b;

                if (!b)
                {
                    BGMAction?.Invoke(0);
                    EffectAction?.Invoke(0);
                }

                else
                {
                    BGMAction?.Invoke(_tempVolume.BGM);
                    EffectAction?.Invoke(_tempVolume.Effect);
                }
            }

            else 
            {
                _tempVolume.IsUseVibe = b;
                if (b)
                    Handheld.Vibrate(); 
            }
        }

        public void Vibrate()
        {
            if (_volume.IsUseVibe) Handheld.Vibrate();
        }

        private SoundVolume LoadVolume(string path)
        {
            if (File.Exists(path))
            {
                string voldata = File.ReadAllText(path);
                return JsonUtility.FromJson<SoundVolume>(voldata);
            }
            else
            {
                return new SoundVolume(0.3f, 0.3f);
            }
        }

        protected override void AwakeInit()
        {
            _volume = LoadVolume(Path.Combine(Application.persistentDataPath, "volume"));
            BGMAction?.Invoke(_volume.BGM);
            EffectAction?.Invoke(_volume.Effect);
            //AudioClip[] sfx = Resources.LoadAll<AudioClip>("Sound/SFX");
            //var sfxs = Enum.GetValues(typeof(ESFX)) as ESFX[];
            _tempVolume = _volume;
            //for (int i = 0; i < sfx.Length; ++i)
            //    _sfxs.Add(sfxs[i], sfx[i]);
        }
    }
}