using _2_Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _2_Scripts.Game.Sound
{
    public class BGMManager : Singleton<BGMManager>
    {
        public enum EBGMType
        {
            Chapter1,
            Chapter2,
            Chapter3,
            Chapter4,
            God,
            Astar,
            Title,
            Prologue,
            Lobby
        }

        private CAudio mAudio;

        private Dictionary<EBGMType, AudioClip> mClipDict = new Dictionary<EBGMType, AudioClip>();

        protected override void ChangeSceneInit(Scene prev, Scene next)
        {
            switch (next.name)
            {
                case "LobbyScene":
                    mAudio.PlaySound(mClipDict[EBGMType.Lobby]);
                    break;

                case "Main":
                    PlaySound($"Chapter{GameManager.Instance.CurrentStageData.ChapterNumber}");
                    break;
            }
        }

        private void Start()
        {
            mAudio = GetComponent<CAudio>();
            MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.SoundResourceLoad).Subscribe(
                _ =>
                {
                    EBGMType[] arr = Enum.GetValues(typeof(EBGMType)) as EBGMType[];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        mClipDict.Add(arr[i], ResourceManager.Instance.Load<AudioClip>(arr[i].ToString()));
                    }
                    mAudio.PlaySound(mClipDict[EBGMType.Title]);
                }).AddTo(this);
        }

        public void PlaySound(EBGMType type, bool isFade = false, float time = 0.5f)
        {
            if (!isFade)
                mAudio.PlaySound(mClipDict[type]);

            else
            {
                mAudio.PlaySound(mClipDict[type]);
                mAudio.StartSoundFade(time);
            }
        }

        public void StopSound(bool isFade = false, float time = 1.5f)
        {
            if (!isFade)
            {
                mAudio.StopSound();
            }
            else
            {
                mAudio.StopSoundFade(time);
            }
        }

        public void PlaySound(string name, bool isFade = false, float time = 1.5f)
        {
            EBGMType[] arr = Enum.GetValues(typeof(EBGMType)) as EBGMType[];
            foreach (var item in arr)
            {
                if (item.ToString().CompareTo(name) == 0)
                {
                    PlaySound(item, isFade, time);
                    break;
                }
            }
        }
    }
}