using _2_Scripts.Game.Map;
using _2_Scripts.Game.Monster;
using _2_Scripts.Utils;
using Cargold.FrameWork.StageWave;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;

    public class NormalStageMode : IStageMode
    {
        public int DeathBossCount { get; private set; }
        public WaveData CurrentWaveData { get; private set; }
        
        public StageData CurrentStageData { get;private set; }
        public List<WaveData> WaveList { get; private set; } = new();
        
        private CancellationTokenSource mCancellationToken = new CancellationTokenSource();
        
        private GameMessage<int> mNextStageMessage;
        
        private WayPoint mWayPoint;

        private List<Monster> mMonsterList;
        
        private TaskMessage mBossSpawnMessage;

        private StageManager mStageManager;
        public void AwakeInit(StageManager stageManager,GameMessage<int> nextStageMessage)
        {
            mCancellationToken = stageManager.mCancellationToken;
            mNextStageMessage = nextStageMessage;
            mWayPoint = stageManager.GetWayPoint;
            mMonsterList = stageManager.MonsterList;
            mStageManager = stageManager;
            
            ObjectPoolManager.Instance.RegisterPoolingObject("Monster", 100);
            MessageBroker.Default.Receive<TaskMessage>()
                .Subscribe(message =>
                {
                    switch (message.Task)
                    {
                        case ETaskList.BossDeath:
                            if (DeathBossCount == CurrentWaveData.spawnCount)
                            {
                                DeathBossCount = 0;
                                StartWave().Forget();
                            }
                            else
                            {
                                DeathBossCount++;
                            }
                            break;
                    }
                }).AddTo(stageManager.gameObject);
        }

        public void StageInit()
        {
            var currentStage = GameManager.Instance.CurrentStageData;
            string stageKey = null;
            if (currentStage == null)
            {
                var exception = new Exception();
                exception.Data.Add("Error", "CurrentStageData is null");
                throw exception;
            }
            stageKey = $"Stage_{(currentStage.ChapterNumber - 1) * 5 + (currentStage.StageNumber - 1)}";
            CurrentStageData = DataBase_Manager.Instance.GetStage.GetData_Func(stageKey);
            foreach (var wave in CurrentStageData.waveList)
            {
                var waveData = DataBase_Manager.Instance.GetWave.GetData_Func(wave);
                WaveList.Add(waveData);
            }

            StartWave().Forget();
        }

        public async UniTaskVoid StartWave()
        {
            await UniTask.WaitForSeconds(3f, cancellationToken: mCancellationToken.Token);
            mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
            MessageBroker.Default.Publish(mNextStageMessage);
            int offset = 0;
            while (true)
            {
                CurrentWaveData = WaveList[mNextStageMessage.Value - 1 + offset];
                SpawnMonster(CurrentWaveData, WaveList.Count == mNextStageMessage.Value + offset).Forget();
                if (CurrentWaveData.isIceMonster)
                {
                    ++offset;
                    continue;
                }

                if (CurrentWaveData.isBoss)
                {
                    MessageBroker.Default.Publish(mBossSpawnMessage);
                    mStageManager.SetBossWave();
                }

                if (WaveList.Count == mNextStageMessage.Value + offset)
                {
                    await UniTask.WaitForSeconds(Define.NEXT_WAVE_TIME, cancellationToken: mCancellationToken.Token);
                    break;
                }
                await UniTask.WaitForSeconds(Define.NEXT_WAVE_TIME, cancellationToken: mCancellationToken.Token);
                mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
                MessageBroker.Default.Publish(mNextStageMessage);
            }
        }

        public async UniTaskVoid SpawnMonster(WaveData waveData, bool isEnd = false)
        {
            int currentWave = mNextStageMessage.Value;
            for (int spawnCount = 0; spawnCount < waveData.spawnCount; spawnCount++)
            {
                var monster = ObjectPoolManager.Instance
                    .CreatePoolingObject(AddressableTable.Default_Monster, mWayPoint.GetWayPointPosition(0))
                    .GetComponent<Monster>();
                WaveStatData waveStateData = DataBase_Manager.Instance.GetWaveStat.GetData_Func(waveData.apply_stat);
                bool isLastBoss =  monster.IsLastBoss = isEnd;
                monster.SpawnMonster(waveData.monsterKey, mWayPoint, waveData.isBoss, waveStateData,waveData.weight,isLastBoss);
                mMonsterList.Add(monster);
                await UniTask.WaitForSeconds(Define.SPAWN_COOL_TIME, cancellationToken: mCancellationToken.Token);
            }
        }
    }
