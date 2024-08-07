using System;
using _2_Scripts.Game.Chapter;
using _2_Scripts.Utils;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_StageIndicator : MonoBehaviour
    {
        [SerializeField]
        private RectTransform mIndicator;
        private Tween moveTween;

        public void Start()
        {
            StartMoveAnimation();
        }

        private void StartMoveAnimation()
        {
            moveTween?.Kill(); // 기존 애니메이션이 있으면 종료
            moveTween = mIndicator.DOAnchorPosY(mIndicator.anchoredPosition.y + 50, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void OnChange(Chapter chapter)
        {
            mIndicator.gameObject.SetActive(chapter.IsLocked);
            mIndicator.anchoredPosition = chapter.LastClearStage.GetRectTransform().anchoredPosition;
            StartMoveAnimation(); // 위치가 변경되었으므로 애니메이션을 재시작
        }
    }
}