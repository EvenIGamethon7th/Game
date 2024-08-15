using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using Cargold;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class Tutorial_EXPBtn : MonoBehaviour
    {
        [SerializeField]
        private Button mExpButton;

        [SerializeField]
        private int mExpPrice = 40;

        private int mPressCount;

        [SerializeField] private TextMeshProUGUI mText;

        private void Awake()
        {
            mExpButton = GetComponent<Button>();
            mExpButton.onClick.AddListener(OnBuyExp);
        }

        private bool IsMaxLevel => IngameDataManager.Instance.CurrentLevel >= Define.MAX_LEVEL;

        private void OnBuyExp()
        {
            if (IngameDataManager.Instance.CurrentGold < mExpPrice || IsMaxLevel)
                return;

            Tween_C.OnPunch_Func(this.transform);
            IngameDataManager.Instance.AddExp(mExpPrice);
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_EXP_Reroll_Touch);
            IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold, -mExpPrice);
            ++mPressCount;
            if (mPressCount == 2)
            {
                MessageBroker.Default.Publish(new GameMessage<bool>(EGameMessage.TutorialProgress, false));
            }
        }
    }
}