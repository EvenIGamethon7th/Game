
using System;
using System.Collections.Generic;
using System.Threading;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.Monster;
using _2_Scripts.Utils;
using Cargold;
using Cysharp.Threading.Tasks;
using Rito.Attributes;
using UniRx;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{

    [GetComponent] private WayPoint mWayPoint;
    
    private StageData mCurrentStageData;
    private Queue<WaveData> mWaveQueue = new Queue<WaveData>();
    private WaveData mCurrentWaveData;
    
    private const float SPAWN_COOL_TIME = 1.0f;
    private const float NEXT_WAVE_TIME = 3.0f;


    private CancellationTokenSource bossDefeatedCancellationTokenSource = new ();
    private ETaskList mGameOverMessage = ETaskList.GameOver;
    /// <summary>
    ///  테스트용 스테이지 시작 코드
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Start()
    {
        MessageBroker.Default.Receive<TaskMessage>()
            .Subscribe(message =>
            {
                switch (message.Task)
                {
                    case ETaskList.DefaultResourceLoad:
                        ObjectPoolManager.Instance.RegisterPoolingObject("Monster", 100);
                        StageInit(TableDataKey_C.Stage_Stage_0);
                        break;
                    case ETaskList.BossDeath:
                        bossDefeatedCancellationTokenSource.Cancel();
                        StartWave().Forget();
                        break;
                }
            });
    }

    public void StageInit(string stageKey)
    {
        mCurrentStageData = DataBase_Manager.Instance.GetStage.GetData_Func(stageKey);
        foreach (var wave in mCurrentStageData.waveList)
        {
            var waveData = DataBase_Manager.Instance.GetWave.GetData_Func(wave);
            mWaveQueue.Enqueue(waveData);
        }

        StartWave().Forget();
    }

    private async UniTaskVoid StartWave()
    {
        while (mWaveQueue.Count > 0)
        {
            mCurrentWaveData = mWaveQueue.Dequeue();
            SpawnMonsters(mCurrentWaveData).Forget();
            if (mCurrentWaveData.isBoss)
            {
                bossDefeatedCancellationTokenSource = new CancellationTokenSource();
                await StartBossWave(mCurrentWaveData.limitTime);
            }
            await UniTask.WaitForSeconds(NEXT_WAVE_TIME);
        }
    }

    private async UniTaskVoid SpawnMonsters(WaveData waveData)
    {
        for (int spawnCount = 0; spawnCount < waveData.spawnCount; spawnCount++)
        {
            var monster = ObjectPoolManager.Instance.CreatePoolingObject("Monster", mWayPoint.GetWayPointPosition(0)).GetComponent<Monster>();
            monster.SpawnMonster(waveData.monsterKey, mWayPoint);
            await UniTask.WaitForSeconds(SPAWN_COOL_TIME);
        }
    }
    
    private async UniTask StartBossWave(float limitTime)
    {
        await UniTask.WaitForSeconds(limitTime, cancellationToken: bossDefeatedCancellationTokenSource.Token);
        if (bossDefeatedCancellationTokenSource.IsCancellationRequested)
        {
            return;
        }
        MessageBroker.Default.Publish(mGameOverMessage);
    }
    
    
}