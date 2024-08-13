using _2_Scripts.Utils;
using _2_Scripts.Utils.Structure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UniRx;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PopUpSortController : SerializedMonoBehaviour
    {
        private PriorityQueue<ISortPopUp> sortPopup = new ();
        private bool mbIsAlready = false;
        private ISortPopUp currentPopUp;

        public void Start()
        {
            MessageBroker.Default.Receive<GameMessage<ISortPopUp>>()
                .Where(x=>x.Message == EGameMessage.SortPopUp)
                .Subscribe(x=>sortPopup.Enqueue(x.Value))
                .AddTo(this);
        }

        public void Update()
        {
            
            if (sortPopup.Count != 0 && !mbIsAlready)
            {
                mbIsAlready = true;
                currentPopUp = sortPopup.Dequeue();
                currentPopUp.OnPopUp();
            }
            if(currentPopUp.IsPopUpEnd)
            {
                mbIsAlready = false;
            }
        }
    }
}