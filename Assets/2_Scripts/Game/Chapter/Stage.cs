﻿using _2_Scripts.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Chapter
{
    using StageData = BackEndData.Stage.StageData;
    public class Stage : MonoBehaviour
    {
        [SerializeField]
        private Button mButton;
        private StageData mStageData;
        private GameMessage<StageData> mPopUpOpenMessage;
        [SerializeField] 
        private Image[] mStarImage;
        
        public bool IsClear => mStageData.IsClear;
        public void Init(StageData stageData)
        {
            mStageData = stageData;
            mPopUpOpenMessage = new GameMessage<StageData>(EGameMessage.GameStartPopUpOpen, mStageData);
            for (int i = 0; i < stageData.Star; i++)
            {
                mStarImage[i].color = Color.white;
            }
            mButton.interactable = stageData.IsClear;
            mButton.onClick.AddListener(GameStartPopUpOpen);
        }

        public void LastSelectIdx()
        {
            mButton.interactable = true;
        }

        private void GameStartPopUpOpen()
        {
            MessageBroker.Default.Publish(mPopUpOpenMessage);
        }
    }
}