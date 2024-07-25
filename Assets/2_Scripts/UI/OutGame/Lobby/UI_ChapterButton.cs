using System;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_ChapterButton : MonoBehaviour
    {
        [SerializeField] private Button mButton;

        public void Init(Action action)
        {
            mButton.onClick.AddListener(action.Invoke);
        }
    }
}