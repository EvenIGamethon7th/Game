using System;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_GameResult : MonoBehaviour
    {
        [SerializeField] private GameObject mVictory;
        [SerializeField] private GameObject mDefeat;
        [SerializeField] private Image mBackGroundImage;
        [SerializeField] private GameObject mButtons;
        private void Start()
        {
            GameManager.Instance.UserHp.Subscribe(hp =>
            {
                if (hp <= 0)
                {
                    /// 0 하면 에러 남 
                    Time.timeScale = 0.00001f;
                    mBackGroundImage.enabled = true;
                    mDefeat.SetActive(true);
                    mButtons.SetActive(true);
                }
            }).AddTo(this);

            MessageBroker.Default.Receive<TaskMessage>().Where(message =>
                message.Task == ETaskList.GameOver)
                .Subscribe(_ =>
                {
                    mBackGroundImage.enabled = true;
                    mVictory.SetActive(true);
                    mButtons.SetActive(true);
                }).AddTo(this);
        }

        public void OnRetryGame()
        {
            SceneManager.LoadScene("1_Scenes/Main");
        }
    }
}