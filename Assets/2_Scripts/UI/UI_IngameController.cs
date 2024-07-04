using System;
using _2_Scripts.Utils;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_IngameController : MonoBehaviour
    {
        [SerializeField]
        private GameObject mBottomGo;

        private void Start()
        {
            MessageBroker.Default.Receive<TaskMessage>()
                .Where(message => message.Task == ETaskList.CharacterDataResourceLoad).Subscribe(
                    _ =>
                    {
                        mBottomGo.SetActive(true);
                    }).AddTo(this);
        }
    }
}