using Cargold.FrameWork.BackEnd;
using Cysharp.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Enchant
{
    public class UI_FeatherTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mTimerText;
        private DateTime mNextFreeTicket = new DateTime();
        private bool mbStaminaCapped;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            UpdateTimerAsync(_cancellationTokenSource.Token).Forget();
        }

        private void UpdateFeatherTimer(DateTime nextFreeTicket, bool staminaCapped)
        {
            mNextFreeTicket = nextFreeTicket;
            mbStaminaCapped = staminaCapped;
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async UniTaskVoid UpdateTimerAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!mbStaminaCapped)
                {
                    TimeSpan timeSpan = mNextFreeTicket.Subtract(DateTime.Now);
                    if (timeSpan.TotalSeconds <= 0)
                    {
                        mTimerText.text = "";
                        await BackEndManager.Instance.GetFeatherTimer(UpdateFeatherTimer);
                    }
                    else
                    {
                        mTimerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                    }
                }
                await UniTask.Delay(1000, cancellationToken: cancellationToken);
            }
        }
    }
}