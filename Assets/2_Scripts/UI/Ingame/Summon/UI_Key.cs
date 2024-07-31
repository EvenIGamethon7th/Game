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
            mText.text = $"{IngameDataManager.Instance.CurrentGold:#,0}";
            IngameDataManager.Instance.Subscribe(this, IngameDataManager.EDataType.LuckyCoin, key =>
            {
                mText.text = $"{key:#,0}";
                Tween_C.OnPunch_Func(mCoinImageGo.transform);
            });
        }
    }
}