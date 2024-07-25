using System;
using _2_Scripts.Game.Chapter;
using _2_Scripts.Utils;
using TMPro;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_ChpaterClearStarText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mStarText;
        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<Chapter>>().Where(message => message.Message == EGameMessage.ChapterChange)
                .Subscribe(data =>
                {
                  var chapter =  data.Value;
                    mStarText.text = $"{chapter.ChapterClearStar}/{chapter.ChapterAllStar}";
                }).AddTo(this);
        }
    }
}