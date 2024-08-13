using _2_Scripts.Utils;
using _2_Scripts.Utils.Structure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PopUpSortController : SerializedMonoBehaviour
    {
        private PriorityQueue<ISortPopUp> sortPopup = new ();
        private bool mbIsAlready = false;
        private ISortPopUp currentPopUp;

        public void Awake()
        {
            MessageBroker.Default.Receive<GameMessage<ISortPopUp>>()
                .Where(x=>x.Message == EGameMessage.SortPopUp)
                .Subscribe(x=>
                {
                    Debug.Log(x.Value.SortIndex);
                    sortPopup.Enqueue(x.Value);
                })
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
            if(currentPopUp is { IsPopUpEnd: true })
            {
                mbIsAlready = false;
            }
        }
    }
}