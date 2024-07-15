
using System;
using System.Collections.Generic;
using System.Linq;
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
    
    private const float SPAWN_COOL_TIME = 1.5f;

    private event Action<int> mWaveStart;
    private const float NEXT_WAVE_TIME = 1.0f;
    
    private int mDeathBossCount = 0;

    public int WaveCount => mNextStageMessage.Value;
    
    private GameMessage<int> mNextStageMessage;
    public List<Monster> MonsterList = new List<Monster>();

    /// <summary>
    ///  테스트용 스테이지 시작 코드
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Start()
    {
        mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 1);
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
                        if(mDeathBossCount == mCurrentWaveData.spawnCount)
                        {
                            mDeathBossCount = 0;
                            StartWave().Forget();
                        }
                        else
                        {
                            mDeathBossCount++;
                        }
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

    public void SubscribeWaveStart(Action<int> action)
    {
        mWaveStart += action;
    }

    public void UnSubscribeWaveStart(Action<int> action)
    {
        mWaveStart -= action;
    }

    private async UniTaskVoid StartWave()
    {
        while (mWaveQueue.Count > 0)
        {
            mCurrentWaveData = mWaveQueue.Dequeue();
            await SpawnMonsters(mCurrentWaveData);
            if (mCurrentWaveData.isBoss)
            {
                continue;
            }
            await UniTask.WaitForSeconds(NEXT_WAVE_TIME);
            mNextStageMessage.SetValue(mNextStageMessage.Value + 1);
            MessageBroker.Default.Publish(mNextStageMessage);
        }
    }

    private async UniTask SpawnMonsters(WaveData waveData)
    {
        for (int spawnCount = 0; spawnCount < waveData.spawnCount; spawnCount++)
        {
            var monster = ObjectPoolManager.Instance.CreatePoolingObject("Monster", mWayPoint.GetWayPointPosition(0)).GetComponent<Monster>();;
            monster.SpawnMonster(waveData.monsterKey, mWayPoint, waveData.isBoss);
            MonsterList.Add(monster);
            // List를 비워줘야함
            await UniTask.WaitForSeconds(SPAWN_COOL_TIME);
            
        }
    }
    
    
    
}