using _2_Scripts.Utils;
using Cargold;
using Cargold.FrameWork.BackEnd;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class UI_Timer : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI text;

        [SerializeField]
        private Color mOriginColor;

        private int mCount;

        private int mBossWave;

        private float mAfterBossKillRemainTime = 3;

        private float mWaveTime;

        private CancellationTokenSource mCts = new();

        private bool mTest;

        private void Start()
        {
            mTest = !GameManager.Instance.IsTest && !BackEndManager.Instance.IsUserTutorial;
            SceneLoadManager.Instance.SceneClear += Clear;
            MessageBroker.Default.Receive<TaskMessage>().
                Where(task => task.Task == ETaskList.BossSpawn).
                Subscribe(_ => mBossWave = StageManager.Instance.WaveCount).AddTo(this);

            MessageBroker.Default.Receive<EGameMessage>().
                Where(message => message == EGameMessage.BossDeath)
                .Subscribe(_ =>
                {
                    if (mBossWave == StageManager.Instance.WaveCount)
                    {
                        mWaveTime = mAfterBossKillRemainTime;
                    }
                }).AddTo(this);

            MessageBroker.Default.Receive<GameMessage<int>>()
                .Where(m => m.Message == EGameMessage.StageChange)
                .Subscribe(m =>
                {
                    int val = mTest ? 10 : 20;
                    StartTimerAsync(val).Forget();
                })
                .AddTo(this);

            if (mTest)
                StartTimerAsync(3).Forget();

            else
            {
                SceneLoadManager.Instance.OnSceneLoad += StartTimer;

                void StartTimer(){
                    SceneLoadManager.Instance.OnSceneLoad -= StartTimer;
                    StartTimerAsync(3).Forget();
                }
            }
        }

        private void Clear()
        {
            SceneLoadManager.Instance.SceneClear -= Clear;
            mCts.Cancel();
            mCts.Dispose();
        }

        private async UniTask StartTimerAsync(int val)
        {
            ++mCount;
            float count = val;
            mWaveTime = val;
            int standard = val - 1;
            text.text = $"00:{val}";

            Tween_C.OnPunch_Func(this);
            while (mWaveTime > 0)
            {
                await UniTask.DelayFrame(1);
                if (mCount > 1 && count < 15) break;

                if (text == null) break;

                if (mTest)
                {
                    await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger, cancellationToken: mCts.Token);
                    mWaveTime -= Time.deltaTime;
                    count -= Time.deltaTime;
                }

                mWaveTime -= Time.deltaTime;
                count -= Time.deltaTime;
                text.text = $"00:{(int)mWaveTime}";

                if (mWaveTime < 5)
                {
                    text.color = Color.red;
                }
                else
                {
                    text.color = mOriginColor;
                }

                if ((int)mWaveTime != standard)
                {
                    standard = (int)mWaveTime;
                    Tween_C.OnPunch_Func(this);
                }
            }

            --mCount;
        }
    }
}