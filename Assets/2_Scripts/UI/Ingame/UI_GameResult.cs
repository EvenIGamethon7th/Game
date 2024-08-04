using System;
using _2_Scripts.Game.Monster;
using _2_Scripts.Game.ScriptableObject.Skill;
using _2_Scripts.Game.Unit;
using _2_Scripts.Trigger;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
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
        [SerializeField] private GameObject mResurrectionButton;
        [SerializeField] private Ressurection mRessurection;

        private int mIsGameOver;
        private float mPrevTimeScale;
        private void Start()
        {
            IngameDataManager.Instance.Subscribe(this, IngameDataManager.EDataType.Hp, hp =>
            {
                if (hp <= 0)
                {
                    /// 0 하면 에러 남 
                    mPrevTimeScale = Time.timeScale;
                    Time.timeScale = 0.00001f;
                    ++mIsGameOver;
                    mBackGroundImage.enabled = true;

                    mDefeat.SetActive(true);
                    mButtons.SetActive(true);

                    if (mIsGameOver == 1)
                    {
                        mResurrectionButton.SetActive(true);
                    }
                    else
                    {
                        mResurrectionButton.SetActive(false);
                    }
                }
            });

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
            BackEndManager.Instance.SaveCharacterData();
        }

        public void OnResurrection()
        {
            mRessurection.CastAttack();
            mDefeat.SetActive(false);
            mButtons.SetActive(false);
            mBackGroundImage.enabled = false;
            Time.timeScale = mPrevTimeScale;
        }

        public void OnRetryGame()
        {

            if (BackEndManager.Instance.UserCurrency[ECurrency.Father].Value <= 0)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("깃털이 부족합니다.", isIgnoreTimeScale: true);
                return;
            }
            BackEndManager.Instance.AddCurrencyData(ECurrency.Father,-1);
            Time.timeScale = 1;
            SaveData();
            SceneLoadManager.Instance.SceneChange("Main"); 
        }
        
        public void OnLobby()
        {
            Time.timeScale = 1;
            SaveData();
            SceneLoadManager.Instance.SceneChange("LobbyScene");
        }
    }
}