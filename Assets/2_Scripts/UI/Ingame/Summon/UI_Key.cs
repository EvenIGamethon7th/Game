using Cargold;
using TMPro;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_Key : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mText;
        [SerializeField] private GameObject mCoinImageGo;
        private void Start()
        {
            mText.text = $"{GameManager.Instance.UserGold:#,0}";
            GameManager.Instance.UserLuckyCoin.Subscribe(key =>
            {
                mText.text = $"{key:#,0}";
                Tween_C.OnPunch_Func(mCoinImageGo.transform);
            }).AddTo(this);
        }
    }
}