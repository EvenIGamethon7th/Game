using _2_Scripts.Game.Sound;
using Cargold.FrameWork.BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_IngameSetting : UI_Base
    {
        private float mRecvSpeed;

        [SerializeField]
        private Button mExitButton;

        private void Awake()
        {
            mExitButton.onClick.AddListener(Exit);
        }

        private void OnEnable()
        {
            mRecvSpeed = Time.timeScale;
            Time.timeScale = 0.00001f;
        }

        private void OnDisable()
        {
            SoundManager.Instance.SaveSound();
            Time.timeScale = mRecvSpeed;
        }

        private void Exit()
        {
            SoundManager.Instance.SaveSound();
            Time.timeScale = mRecvSpeed;
            BackEndManager.Instance.SaveCharacterData();
            SceneLoadManager.Instance.SceneChange("LobbyScene");
            gameObject.SetActive(false);
        }
    }
}