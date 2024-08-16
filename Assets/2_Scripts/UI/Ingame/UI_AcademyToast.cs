using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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
        private readonly Vector3 mStart = new Vector3(0, 1, 1);

        private Sequence mSeq;

        public void Init()
        {
            mSeq = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    mToast.gameObject.SetActive(true);
                    mToast.transform.localScale = mStart;
                    mToast.color = Color.white;
                    mToastText.transform.localScale = mStart;
                    mToastText.color = Color.white;
                })
                .Append(mToast.transform.DOScaleX(1, 2).SetEase(Ease.InOutCubic))
                .Join(mToastText.transform.DOScaleX(1, 2).SetEase(Ease.InOutCubic))
                .AppendInterval(1)
                .Append(mToast.DOColor(mFade, 2))
                .Join(mToastText.DOColor(mFade, 2))
                .AppendCallback(() => {
                    mToast.gameObject.SetActive(false);
                    mToast.transform.localScale = mStart;
                    mToastText.transform.localScale = mStart;
                })
                .Pause()
                .SetAutoKill(false);
        }

        public void PlayToast(string text, bool isOpen = true)
        {
            mToastText.text = text;
            mSeq.Restart();
            mOpen.SetActive(isOpen);
        }

        public void Clear()
        {
            mOpen.SetActive(false);
        }
    }
}