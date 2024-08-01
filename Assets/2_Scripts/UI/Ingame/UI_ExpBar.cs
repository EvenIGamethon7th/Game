
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
            IngameDataManager.Instance.Subscribe(this, IngameDataManager.EDataType.EXP, exp =>
            {
                var fillAmount = (float)exp / IngameDataManager.Instance.GetMaxExp();
                SetFillAmount(fillAmount);
            });
        }


        private void SetFillAmount(float fillAmount)
        {
            DOTween.To(() => mFillImage.fillAmount, x => mFillImage.fillAmount = x, fillAmount, 0.5f);
        }
    }
}