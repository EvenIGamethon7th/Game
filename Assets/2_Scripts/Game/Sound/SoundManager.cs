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

    public class SoundManager : Singleton<SoundManager>
    {
        public struct SoundVolume
        {
            public float BGM;
            public float Effect;

            public SoundVolume(float bgm = 0.5f, float effect = 0.5f)
            {
                BGM = bgm;
                Effect = effect;
            }
        }

        public SoundVolume Volume { get => _volume; }
        private SoundVolume _volume;
        protected override void ChangeSceneInit(Scene prev, Scene next)
        {
            
        }

        public Dictionary<ESFX, AudioClip> SFXs { get => _sfxs; }
        private Dictionary<ESFX, AudioClip> _sfxs = new Dictionary<ESFX, AudioClip>();

        public event Action<float> BGMAction;
        public event Action<float> EffectAction;

        protected override void Awake()
        {
            base.Awake();
            _volume = LoadVolume(Path.Combine(Application.persistentDataPath, "volume"));
            BGMAction?.Invoke(_volume.BGM);
            EffectAction?.Invoke(_volume.Effect);
            AudioClip[] sfx = Resources.LoadAll<AudioClip>("Sound/SFX");
            var sfxs = Enum.GetValues(typeof(ESFX)) as ESFX[];

            for (int i = 0; i < sfx.Length; ++i)
                _sfxs.Add(sfxs[i], sfx[i]);
        }

        public void Play2DSound(ESFX type)
        {
            AudioSource.PlayClipAtPoint(_sfxs[type], Vector3.zero, Volume.Effect);
        }

        public void Play2DSound(AudioClip clip)
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, Volume.Effect);
        }

        public void SaveSound(SoundVolume vol)
        {
            string voldata = JsonUtility.ToJson(vol);
            string path = Path.Combine(Application.persistentDataPath, "volume");
            File.WriteAllText(path, voldata);
            _volume = vol;
            BGMAction?.Invoke(_volume.BGM);
            EffectAction?.Invoke(_volume.Effect);
        }

        public void SetVolumeTemporary(float vol, Sound type)
        {
            if (type == Sound.Bgm)
                BGMAction?.Invoke(vol);
            else
                EffectAction?.Invoke(vol);
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
    }
}