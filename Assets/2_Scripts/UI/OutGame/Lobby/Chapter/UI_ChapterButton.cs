using System;
using System.Collections.Generic;
using _2_Scripts.Game.Chapter;
using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_ChapterButton : SerializedMonoBehaviour
    {
        [SerializeField] private Button mButton;
        [SerializeField] private Image mImage;
        [SerializeField] private Dictionary<bool,Sprite> onOffSprite;
        private int mChpaternum;
        public void Init(Action action, int chapterNum)
        {
            mChpaternum = chapterNum;
            mButton.onClick.AddListener(action.Invoke);
            MessageBroker.Default.Receive<GameMessage<Chapter>>()
                .Where(message => message.Message == EGameMessage.ChapterChange).Subscribe(
                    data =>
                    {
                        var onOff = data.Value.ChapterNumber == mChpaternum;
                        mImage.sprite = onOffSprite[onOff];
                    }).AddTo(this);
        }
        
    }
}