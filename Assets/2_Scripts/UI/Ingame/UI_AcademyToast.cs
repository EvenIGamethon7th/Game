using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

namespace _2_Scripts.UI.Ingame
{
    public class UI_AcademyToast : MonoBehaviour
    {
        [SerializeField]
        private Image mToast;
        [SerializeField]
        private TextMeshProUGUI mToastText;
        [SerializeField]
        private GameObject mOpen;

        private readonly Color mFade = new Color(1, 1, 1, 0);

        private Sequence mSeq;

        public void Init()
        {
            mSeq = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    mToast.gameObject.SetActive(true);
                    mToast.transform.localScale = Vector3.up;
                    mToast.color = Color.white;
                    mToastText.transform.localScale = Vector3.up;
                    mToastText.color = Color.white;
                })
                .Append(mToast.transform.DOScaleX(1, 1).SetEase(Ease.InOutCubic))
                .Join(mToastText.transform.DOScaleX(1, 1).SetEase(Ease.InOutCubic))
                .Append(mToast.DOColor(mFade, 1))
                .Join(mToast.DOColor(mFade, 1))
                .AppendCallback(() => mToast.gameObject.SetActive(false))
                .Pause()
                .SetAutoKill(false);
        }

        public void PlayToast()
        {
            mSeq.Restart();
            mOpen.SetActive(true);
        }

        public void Clear()
        {
            mOpen.SetActive(false);
        }
    }
}