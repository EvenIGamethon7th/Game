using Cargold.FrameWork.BackEnd;
using Cargold;
using Coffee.UIExtensions;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using _2_Scripts.Game.Sound;

namespace _2_Scripts.UI.Ingame
{
    public class Tutorial_Victory : MonoBehaviour
    {
        [SerializeField]
        private RectTransform mVictoryPanel;

        [SerializeField] private GameObject[] mStarts;

        [SerializeField]
        private UIParticle mParticle;

        [SerializeField] private TextMeshProUGUI mRewardText;
        private void Start()
        {
            Vector2 originPos = mVictoryPanel.anchoredPosition;
            mVictoryPanel.DOAnchorPosY(originPos.y + 10, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
            StartCoroutine(StartAnimationCoroutine());
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Win);
        }

        IEnumerator StartAnimationCoroutine()
        {
            Time.timeScale = 1;
            int rank = 3;
            var reward = 100;
            mRewardText.text = $"+{reward}";
            BackEndManager.Instance.AddCurrencyData(ECurrency.Diamond, reward);
            BackEndManager.Instance.IsUserTutorial = true;
            BackEndManager.Instance.SaveCharacterData();
            for (int i = 0; i < rank; i++)
            {
                Tween_C.OnPunch_Func(mStarts[i]);
                yield return new WaitForSecondsRealtime(0.5f);
                SoundManager.Instance.Play2DSound(AddressableTable.Sound_Win_Star);
                mParticle.rectTransform.position = mStarts[i].transform.position;
                mParticle.Play();
                foreach (Transform child in mStarts[i].transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}