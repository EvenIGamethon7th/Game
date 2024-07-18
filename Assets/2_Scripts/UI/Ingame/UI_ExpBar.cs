
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_ExpBar : MonoBehaviour
    {
        [SerializeField]
        private Image mFillImage;

        private void Start()
        {
            GameManager.Instance.UserExp.Subscribe(exp =>
            {
                var fillAmount = (float)exp / GameManager.Instance.GetMaxExp();
                SetFillAmount(fillAmount);
            }).AddTo(this);
        }


        private void SetFillAmount(float fillAmount)
        {
            DOTween.To(() => mFillImage.fillAmount, x => mFillImage.fillAmount = x, fillAmount, 0.5f);
        }
    }
}