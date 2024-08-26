using _2_Scripts.Game.Sound;
using Cargold.FrameWork.BackEnd;
using Cargold;
using Coffee.UIExtensions;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

namespace _2_Scripts.UI.Ingame
{
    public class UI_ChallengeVictory : MonoBehaviour
    {
        [SerializeField]
        private RectTransform mVictoryPanel;

        [SerializeField] private TextMeshProUGUI mRewardText;
        [SerializeField] private TextMeshProUGUI mWaveCountText;
        [SerializeField] private GameObject mRewardGo;
        [SerializeField] private TextMeshProUGUI mMaxText;

        private void Start()
        {
            Vector2 originPos = mVictoryPanel.anchoredPosition;
            mVictoryPanel.DOAnchorPosY(originPos.y + 10, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Win);
            SetResult();
        }

        private void SetResult()
        {
            BackEndManager.Instance.UserServiceMission.SetMaxWaveCount(StageManager.Instance.WaveCount);
            mWaveCountText.text = StageManager.Instance.WaveCount.ToString();

            if (!BackEndManager.Instance.UserServiceMission.IsAdmissionSurviveMission())
            {
                var temp = DataBase_Manager.Instance.GetReward_CH.GetData_Func(Mathf.Max((StageManager.Instance.WaveCount - 1) / 10, 0));
                int reward = temp.Reward_Count;
                mRewardText.text = $"+{reward}";
                BackEndManager.Instance.AddCurrencyData(ECurrency.Diamond, reward);
                mRewardGo.SetActive(true);
                mMaxText.gameObject.SetActive(false);
            }

            else
            {
                mRewardGo.SetActive(false);
                mRewardText.gameObject.SetActive(false);
                mMaxText.gameObject.SetActive(true);
            }

            BackEndManager.Instance.SaveCharacterData();
        }
    }
}