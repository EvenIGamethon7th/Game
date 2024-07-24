using System;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_ChapterButton : MonoBehaviour
    {
        [SerializeField] private Button mButton;

        public void Init(bool isClear,Action action)
        {
            mButton.interactable = isClear;
            mButton.onClick.AddListener(action.Invoke);
        }
    }
}