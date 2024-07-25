using System;
using _2_Scripts.Game.Chapter;
using _2_Scripts.Utils;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_StageIndicator : MonoBehaviour
    {
        [SerializeField]
        private RectTransform mIndicator;
        public void OnChange(Chapter chapter)
        {
            mIndicator.gameObject.SetActive(chapter.IsLocked);
            transform.position = chapter.LastClearStage.GetRectTransform().position;
        }
    }
}