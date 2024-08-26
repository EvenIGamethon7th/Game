using Cargold.FrameWork.BackEnd;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_ChallengePopUp : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI mWaveCountText;
        [SerializeField]
        private TextMeshProUGUI mRewardCountText;

        private Vector2 mOriginPos;
        private RectTransform mRect;

        private Sequence mSeq;

        private void Awake()
        {
            mRect = GetComponent<RectTransform>();
            mOriginPos = mRect.anchoredPosition;
            mSeq = DOTween
                .Sequence()
                .Append(mRect.DOAnchorPosY(-911, 1f).SetEase(Ease.InOutCubic))
                .Pause()
                .SetAutoKill(false);
        }

        private void OnEnable()
        {
            mSeq.Restart();
            mWaveCountText.text = BackEndManager.Instance.UserServiceMission.maxWaveCount.ToString();
            mRewardCountText.text = $"<color=#FCCA23>{Mathf.Min(BackEndManager.Instance.UserServiceMission.surviveCount, SurviveMission.MAX_SURVIVE_COUNT)}</color>/{SurviveMission.MAX_SURVIVE_COUNT}";
        }

        private void OnDisable()
        {
            mRect.anchoredPosition = mOriginPos;
        }
    }
}