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

        private int mIsGameOver;
        private float mPrevTimeScale;
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

        private void SaveData()
        {
            GameManager.Instance.CurrentDialog = 0;
            string data = JsonUtility.ToJson(true);
            string path = Path.Combine(Application.persistentDataPath, "IsPlayTutorial");
            File.WriteAllText(path, data);
        }

        public void OnLobby()
        {
            Time.timeScale = 1;
            SaveData();
            SceneLoadManager.Instance.SceneChange("LobbyScene");
        }
    }
}