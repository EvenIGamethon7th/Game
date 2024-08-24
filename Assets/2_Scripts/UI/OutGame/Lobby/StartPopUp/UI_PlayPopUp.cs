﻿using _2_Scripts.Game.BackEndData.Stage;
using System;
using _2_Scripts.UI.OutGame.Lobby.StartPopUp;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    using StageData = _2_Scripts.Game.BackEndData.Stage.StageData;
    public class UI_PlayPopUp : MonoBehaviour
    {
        [SerializeField]
        private Button mPlayButton;
        [SerializeField]
        private GameObject mPopUpGo;
        private StageData mStageData;
        [SerializeField]
        private UI_SelectItemContainer mSelectItemContainer;

        private bool mbIsStart = false;
        public void Start()
        {
            mbIsStart = false;
            mPlayButton.onClick.AddListener(OnClickPlay);
            MessageBroker.Default.Receive<GameMessage<StageData>>()
                .Where(message => message.Message == EGameMessage.GameStartPopUpOpen).Subscribe(
                    data =>
                    {
                        mStageData = data.Value;
                        mPopUpGo.gameObject.SetActive(true);
                    }).AddTo(this);
        }

        private void OnClickPlay()
        {
            if (mbIsStart)
                return;
            #if !UNITY_EDITOR
            int featherConsum = mStageData.StageType == StageType.Survive ? 3 : 1;
            if (BackEndManager.Instance.UserCurrency[ECurrency.Father].Value <= featherConsum)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("깃털이 부족합니다.");
                return;
            }
            BackEndManager.Instance.AddCurrencyData(ECurrency.Father,-featherConsum);
             #endif
            mbIsStart = true;
            mSelectItemContainer.UseItems();
            GameManager.Instance.SetCurrentStageData(mStageData);
            SceneLoadManager.Instance.SceneChange("Main");
        }
    }
}