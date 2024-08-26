using _2_Scripts.Game.Map;
using _2_Scripts.Game.Monster;
using _2_Scripts.Utils;
using Cargold.FrameWork.StageWave;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

public class ChallengeStageMode : StageManager
{
    private WaveStatData mTempStat;
    protected override void Init()
    {
        mTempStat = new();
        mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
        mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
        ObjectPoolManager.Instance.RegisterPoolingObject("Monster", 100);
        MessageBroker.Default.Receive<TaskMessage>()
            .Subscribe(message =>
            {
                switch (message.Task)
                {
                    case ETaskList.BossDeath:
                        if (mDeathBossCount == mCurrentWaveData.spawnCount)
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
            }).AddTo(this);
        SceneLoadManager.Instance.OnSceneLoad -= StageInit;
        SceneLoadManager.Instance.OnSceneLoad += StageInit;
    }

    protected override void StageInit()
    {
        SceneLoadManager.Instance.OnSceneLoad -= StageInit;
        StageInit("Stage_1000");
    }

    protected override async UniTaskVoid StartWave()
    {
        await UniTask.WaitForSeconds(3f, cancellationToken: mCancellationToken.Token);
        mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
        MessageBroker.Default.Publish(mNextStageMessage);
        int offset = 0;
        while (true)
        {
            mCurrentWaveData = mWaveList[mNextStageMessage.Value - 1 + offset];
            SpawnMonsters(mCurrentWaveData, mWaveList.Count == mNextStageMessage.Value + offset).Forget();
            if (mCurrentWaveData.isIceMonster)
            {
                ++offset;
                continue;
            }

            if (mCurrentWaveData.isBoss)
            {
                MessageBroker.Default.Publish(mBossSpawnMessage);
                mBossWave = mNextStageMessage.Value;
            }

            if (mWaveList.Count == mNextStageMessage.Value + offset)
            {
                await WaitAsync();
                break;
            }

            await WaitAsync();

            mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
            MessageBroker.Default.Publish(mNextStageMessage);

            async UniTask WaitAsync()
            {
                mWaveTime = NEXT_WAVE_TIME;

                while (mWaveTime > 0)
                {
                    await UniTask.DelayFrame(1, cancellationToken: mCancellationToken.Token);
                    mWaveTime -= Time.deltaTime;
                }
            }
        }

    }

    protected override async UniTask SpawnMonsters(WaveData waveData, bool isEnd = false)
    {
        int currentWave = mNextStageMessage.Value;

        SetWaveStat(waveData);

        for (int spawnCount = 0; spawnCount < waveData.spawnCount; spawnCount++)
        {
            var monster = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Monster, mWayPoint.GetWayPointPosition(0)).GetComponent<Monster>();
            
            bool isLastBoss = monster.IsLastBoss = isEnd;
            monster.SpawnMonster(waveData.monsterKey, mWayPoint, waveData.isBoss, mTempStat, 1, isLastBoss);

            MonsterList.Add(monster);
            float time = SPAWN_COOL_TIME;

            while (time > 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCancellationToken.Token);

                time -= Time.deltaTime;
            }

            if (currentWave != mNextStageMessage.Value)
                break;
        }
    }

    private void SetWaveStat(WaveData waveData)
    {
        var tempStat = DataBase_Manager.Instance.GetWaveStat.GetData_Func(waveData.apply_stat);

        mTempStat.reward_count1 = tempStat.reward_count1;
        mTempStat.reward_type1 = tempStat.reward_type1;
        mTempStat.reward_type2 = tempStat.reward_type2;
        mTempStat.reward_count2 = tempStat.reward_count2;
        mTempStat.hp = waveData.weight;
        mTempStat.def = 10 * Mathf.FloorToInt(mNextStageMessage.Value / 40);
        mTempStat.mdef = 10 * (Mathf.FloorToInt(mNextStageMessage.Value / 40) - 1);
        mTempStat.speed = tempStat.speed;
        mTempStat.atk = tempStat.atk;
    }
}
