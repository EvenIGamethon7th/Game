
using System;
using System.Collections.Generic;
using System.Threading;
using _2_Scripts.Game.Map;
using _2_Scripts.Game.Monster;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using Cysharp.Threading.Tasks;
using Rito.Attributes;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class StageManager : Singleton<StageManager>
{
    [GetComponent] protected WayPoint mWayPoint;

    public StageData mCurrentStageData { get; protected set; }
    protected List<WaveData> mWaveList = new List<WaveData>();
    protected WaveData mCurrentWaveData;

    protected const float SPAWN_COOL_TIME = 1.5f;
    protected const float NEXT_WAVE_TIME = 20.0f;

    protected int mDeathBossCount = 0;

    public int WaveCount => mNextStageMessage.Value;

    protected GameMessage<int> mNextStageMessage;
    public List<Monster> MonsterList = new List<Monster>();

    protected CancellationTokenSource mCancellationToken = new CancellationTokenSource();
    protected TaskMessage mBossSpawnMessage;

    public int MaxStageCount { get; protected set; }
    protected bool mIsTutorial = false;
    protected float mAfterBossKillRemainTime = 3;
    protected int mBossWave;

    protected float mWaveTime;
    protected bool mIsRewind;

    protected override sealed void AwakeInit()
    {
        SceneLoadManager.Instance.SceneClear += Clear;
        MessageBroker.Default.Receive<EGameMessage>().
            Where(message => message == EGameMessage.BossDeath)
            .Subscribe(_ =>
            {
                if (mBossWave == mNextStageMessage.Value)
                {
                    mWaveTime = mAfterBossKillRemainTime;
                }
            }).AddTo(this);

        Init();
    }

    protected abstract void Init();

    protected abstract void StageInit();

    protected void StageInit(string stageKey)
    {
        mCurrentStageData = DataBase_Manager.Instance.GetStage.GetData_Func(stageKey);
        foreach (var wave in mCurrentStageData.waveList)
        {
            var waveData = DataBase_Manager.Instance.GetWave.GetData_Func(wave);
            mWaveList.Add(waveData);
        }

        MaxStageCount = mWaveList.Count;
        StartWave().Forget();
    }

    protected abstract UniTaskVoid StartWave();
    //{
    //    await UniTask.WaitForSeconds(3f, cancellationToken: mCancellationToken.Token);
    //    mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
    //    MessageBroker.Default.Publish(mNextStageMessage);
    //    int offset = 0;
    //    while (true)
    //    {
    //        mIsRewind = false;
    //        mCurrentWaveData = mWaveList[mNextStageMessage.Value - 1 + offset];
    //        SpawnMonsters(mCurrentWaveData, mWaveList.Count == mNextStageMessage.Value + offset).Forget();
    //        if (mCurrentWaveData.isIceMonster)
    //        {
    //            ++offset;
    //            continue;
    //        }
    //
    //        if (mCurrentWaveData.isBoss)
    //        {
    //            MessageBroker.Default.Publish(mBossSpawnMessage);
    //            mBossWave = mNextStageMessage.Value;
    //        }
    //
    //        if (mWaveList.Count == mNextStageMessage.Value + offset)
    //        {
    //            await WaitAsync();
    //            break;
    //        }
    //
    //        await WaitAsync();
    //
    //        mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
    //        MessageBroker.Default.Publish(mNextStageMessage);
    //
    //        async UniTask WaitAsync()
    //        {
    //            mWaveTime = mIsTutorial ? 10 : NEXT_WAVE_TIME;
    //
    //            while (mWaveTime > 0)
    //            {
    //                await UniTask.DelayFrame(1, cancellationToken: mCancellationToken.Token);
    //                if (mIsTutorial)
    //                {
    //                    await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger || mIsRewind, cancellationToken: mCancellationToken.Token);
    //                    if (mIsRewind)
    //                        break;
    //                    mWaveTime -= Time.deltaTime;
    //                }
    //
    //                mWaveTime -= Time.deltaTime;
    //            }
    //        }
    //    }
    //
    //}

    private void Clear()
    {
        SceneLoadManager.Instance.SceneClear -= Clear;
        CancelAndDisposeToken();
    }

    protected abstract UniTask SpawnMonsters(WaveData waveData, bool isEnd = false);
    //{
    //    if (mIsTutorial)
    //        await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger || waveData.isBoss, cancellationToken: mCancellationToken.Token);
    //
    //    int currentWave = mNextStageMessage.Value;
    //
    //    for (int spawnCount = 0; spawnCount < waveData.spawnCount; spawnCount++)
    //    {
    //        var monster = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Monster, mWayPoint.GetWayPointPosition(0)).GetComponent<Monster>();
    //        WaveStatData waveStateData = DataBase_Manager.Instance.GetWaveStat.GetData_Func(waveData.apply_stat);
    //        bool isLastBoss = monster.IsLastBoss = isEnd;
    //        monster.SpawnMonster(waveData.monsterKey, mWayPoint, waveData.isBoss, waveStateData, waveData.weight, isLastBoss);
    //
    //        MonsterList.Add(monster);
    //        float time = SPAWN_COOL_TIME;
    //
    //        while (time > 0)
    //        {
    //            await UniTask.DelayFrame(1, cancellationToken: mCancellationToken.Token);
    //            if (mIsTutorial)
    //            {
    //                await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger || mIsRewind, cancellationToken: mCancellationToken.Token);
    //                time -= Time.deltaTime;
    //            }
    //
    //            time -= Time.deltaTime;
    //        }
    //
    //        if (currentWave != mNextStageMessage.Value)
    //            break;
    //    }
    //}

    public void RemoveMonster(Monster monster)
    {
        if (!MonsterList.Contains(monster))
        {
            return;
        }
        MonsterList.Remove(monster);
    }

    #region Edit

    //private void EditInit()
    //{
    //    mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
    //    mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
    //    MessageBroker.Default.Receive<TaskMessage>()
    //        .Where(message => message.Task == ETaskList.DefaultResourceLoad)
    //        .Subscribe(message =>
    //        {
    //            mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
    //            mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
    //            ObjectPoolManager.Instance.RegisterPoolingObject("Monster", 100);
    //        }).AddTo(this);
    //    MessageBroker.Default.Receive<EditMessage<int, int>>().Subscribe(message =>
    //    {
    //        GetStageAndWaveData(message.Value1, message.Value2);
    //    }).AddTo(this);
    //}

    //private void GetStageAndWaveData(int stage, int wave)
    //{
    //    mCurrentStageData = DataBase_Manager.Instance.GetStage.GetData_Func($"Stage_{stage}");
    //
    //    WaveData waveData;
    //    if (wave != -1)
    //    {
    //        waveData = DataBase_Manager.Instance.GetWave.GetData_Func(mCurrentStageData.waveList[wave]);
    //        mWaveList.Add(waveData);
    //    }
    //
    //    else
    //    {
    //        foreach (var waveDatas in mCurrentStageData.waveList)
    //        {
    //            waveData = DataBase_Manager.Instance.GetWave.GetData_Func(waveDatas);
    //            mWaveList.Add(waveData);
    //        }
    //    }
    //
    //    StartWave().Forget();
    //}

    #endregion

    private void CancelAndDisposeToken()
    {
        if (mCancellationToken != null)
        {
            if (!mCancellationToken.IsCancellationRequested)
            {
                mCancellationToken.Cancel();
            }
            mCancellationToken.Dispose();
            mCancellationToken = null;
        }
        else
        {
            mCancellationToken = new CancellationTokenSource();
        }
    }
}