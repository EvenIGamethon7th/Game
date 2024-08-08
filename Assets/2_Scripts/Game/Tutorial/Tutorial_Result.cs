using _2_Scripts.Game.ScriptableObject.Skill;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class Tutorial_Result : MonoBehaviour
    {
        [SerializeField] private GameObject mVictory;
        [SerializeField] private Image mBackGroundImage;
        [SerializeField] private GameObject mButtons;

        private void Start()
        {
            MessageBroker.Default.Receive<TaskMessage>().Where(message =>
                message.Task == ETaskList.GameOver)
                .Subscribe(_ =>
                {
                    mBackGroundImage.enabled = true;
                    mVictory.SetActive(true);
                    mButtons.SetActive(true);
                }).AddTo(this);
        }

        public void OnLobby()
        {
            Time.timeScale = 1;
            SceneLoadManager.Instance.SceneChange("LobbyScene");
        }
    }
}