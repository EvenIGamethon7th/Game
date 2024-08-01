using _2_Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _2_Scripts.Game.Sound
{
    public enum EBGMType
    {

    }
    public class BGMManager : Singleton<BGMManager>
    {
        private CAudio mAudio;

        private Dictionary<EBGMType, AudioClip> mClipDict = new Dictionary<EBGMType, AudioClip>();

        protected override void ChangeSceneInit(Scene prev, Scene next)
        {

        }

        private void Start()
        {
            mAudio = GetComponent<CAudio>();
            MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.SoundResourceLoad).Subscribe(
                _ =>
                {
                    //mAudio.PlaySound();
                }).AddTo(this);
        }
    }
}