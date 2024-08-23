using _2_Scripts.Game.Monster;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class TutorialStageMode : StageManager
{
    protected override void Init()
    {
        TutorialInitAsync().Forget();
    }

    private async UniTask TutorialInitAsync()
    {
        mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
        mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
        MessageBroker.Default.Receive<GameMessage<bool>>().
            Where(message => message.Message == EGameMessage.TutorialRewind)
            .Subscribe(_ =>
            {
                mIsRewind = true;
                mNextStageMessage?.SetValue(mNextStageMessage.Value - 1);
            }).AddTo(this);

        await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger, cancellationToken: mCancellationToken.Token);

        StageInit();
    }

    protected override void StageInit()
    {
        StageInit("Stage_100");
    }

    protected override async UniTaskVoid StartWave()
    {
        await UniTask.WaitForSeconds(3f, cancellationToken: mCancellationToken.Token);
        mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
        MessageBroker.Default.Publish(mNextStageMessage);
        int offset = 0;
        while (true)
        {
            mIsRewind = false;
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
                mWaveTime = 10;

                while (mWaveTime > 0)
                {
                    await UniTask.DelayFrame(1, cancellationToken: mCancellationToken.Token);
                    mWaveTime -= Time.deltaTime;

                    await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger || mIsRewind, cancellationToken: mCancellationToken.Token);
                    if (mIsRewind)
                        break;
                    mWaveTime -= Time.deltaTime;
                }
            }
        }
    }

    protected override async UniTask SpawnMonsters(WaveData waveData, bool isEnd = false)
    {
        await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger || waveData.isBoss, cancellationToken: mCancellationToken.Token);

        int currentWave = mNextStageMessage.Value;

        for (int spawnCount = 0; spawnCount < waveData.spawnCount; spawnCount++)
        {
            var monster = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Monster, mWayPoint.GetWayPointPosition(0)).GetComponent<Monster>();
            WaveStatData waveStateData = DataBase_Manager.Instance.GetWaveStat.GetData_Func(waveData.apply_stat);
            bool isLastBoss = monster.IsLastBoss = isEnd;
            monster.SpawnMonster(waveData.monsterKey, mWayPoint, waveData.isBoss, waveStateData, waveData.weight, isLastBoss);

            MonsterList.Add(monster);
            float time = SPAWN_COOL_TIME;

            while (time > 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCancellationToken.Token);
                time -= Time.deltaTime;
                await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger || mIsRewind, cancellationToken: mCancellationToken.Token);
                time -= Time.deltaTime;
            }

            if (currentWave != mNextStageMessage.Value)
                break;
        }
    }
}