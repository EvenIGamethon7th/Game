using _2_Scripts.Utils;
using Cargold;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_ExpButton : MonoBehaviour
    {
        [SerializeField]
        private Button mExpButton;

        [SerializeField]
        private int mExpPrice = 10;

        [SerializeField] private TextMeshProUGUI mText;
        
        private void Start()
        {
            mExpButton.onClick.AddListener(OnBuyExp);
            GameManager.Instance.UserGold.Subscribe(gold =>
            {
                if (gold < mExpPrice || IsMaxLevel)
                {
                    mExpButton.interactable = false;
                    mText.color = Color.red;
                }
                else
                {
                    mExpButton.interactable = true;
                    mText.color = Color.white;
                }
            }).AddTo(this);
        }


        private bool IsMaxLevel=> GameManager.Instance.UserLevel.Value >= Define.MAX_LEVEL;
        
        private void OnBuyExp()
        {
            if (GameManager.Instance.UserGold.Value < mExpPrice || IsMaxLevel )
                return;

            Tween_C.OnPunch_Func(this.transform);
            GameManager.Instance.AddExp(mExpPrice);
            GameManager.Instance.UpdateGold(-mExpPrice);
        }

    }
}