﻿using System;
using System.Collections;
using Cargold;
using Cargold.FrameWork.BackEnd;
using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;

namespace _2_Scripts.UI.Ingame
{
    public class UI_Victory : MonoBehaviour
    {
        [SerializeField]
        private RectTransform mVictoryPanel;

        [SerializeField] private GameObject[] mStarts;

        [SerializeField]
        private UIParticle mParticle;

        private void Start()
        {
            Vector2 originPos = mVictoryPanel.anchoredPosition;
            mVictoryPanel.DOAnchorPosY(originPos.y + 10,1f).SetEase(Ease.InOutQuad).SetLoops(-1,LoopType.Yoyo);
            StartCoroutine(StartAnimationCoroutine());
        }

        IEnumerator StartAnimationCoroutine()
        {
            int rank = RankCalculator(GameManager.Instance.UserHp.Value);
            var stageData = GameManager.Instance.CurrentStageData;
            stageData.Star = Math.Max(stageData.Star,rank);
            stageData.IsClear = true;
            BackEndManager.Instance.SaveChapterData();
            
            for (int i = 0; i < rank; i++)
            {
                Tween_C.OnPunch_Func(mStarts[i]);
                yield return new WaitForSeconds(0.5f);
                mParticle.rectTransform.position = mStarts[i].transform.position;
                mParticle.Play();
                foreach (Transform child in mStarts[i].transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
        
        private  int RankCalculator(float hp)
        {
            return hp switch
            {
                >= 70 => 3,
                >= 40 => 2,
                _ => 1,
            };
        }
    }
}