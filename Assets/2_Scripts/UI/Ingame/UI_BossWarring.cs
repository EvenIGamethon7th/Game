using System;
using System.Collections;
using _2_Scripts.Utils;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.Ingame
{
    public class UI_BossWarring : MonoBehaviour
    {
        [SerializeField] private GameObject mWarringGo;
        [SerializeField] private Animator mAnimator;

        private void Start()
        {
            MessageBroker.Default.Receive<TaskMessage>().Where(message=>message.Task == ETaskList.BossSpawn)
                .Subscribe(_ =>
                {
                    OnBossSpawnUI();
                }).AddTo(this);
        }

        private void OnBossSpawnUI()
        {
            mWarringGo.SetActive(true);
            mAnimator.Play(0);
            StartCoroutine(DisableCoroutine());
        }
        
        IEnumerator DisableCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
            mWarringGo.SetActive(false);
        }
    }
}