
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
using Unity.VisualScripting;
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

    protected int mOffset;

    private bool mTestWave = false;

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

#if UNITY_EDITOR
        if (GameManager.Instance.IsTest)
        {
            EditInit();
        }

        else
        {
            Init();
        }
#else
        Init();
#endif
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

    private void Clear()
    {
        SceneLoadManager.Instance.SceneClear -= Clear;
        CancelAndDisposeToken();
    }

    protected abstract UniTask SpawnMonsters(WaveData waveData, bool isEnd = false);

    public void RemoveMonster(Monster monster)
    {
        if (!MonsterList.Contains(monster))
        {
            return;
        }
        MonsterList.Remove(monster);
    }

    #region Edit
    private void EditInit()
    {
        mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
        mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
        MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.DefaultResourceLoad)
            .Subscribe(message =>
            {
                mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
                mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
                ObjectPoolManager.Instance.RegisterPoolingObject("Monster", 100);
            }).AddTo(this);
        MessageBroker.Default.Receive<EditMessage<int, int>>().Subscribe(message =>
        {
            if (message.Value2 < 0)
                return;
            if (!mTestWave)
            {
                GetStageAndWaveData(message.Value1, message.Value2);
            }
        }).AddTo(this);
    }

    private void GetStageAndWaveData(int stage, int wave)
    {
        mTestWave = true;
        mCurrentStageData = DataBase_Manager.Instance.GetStage.GetData_Func($"Stage_{stage}");
    
        WaveData waveData;
        int tempWaveCount = wave;

        for (int i = 0; i < wave; ++i)
        {
            waveData = DataBase_Manager.Instance.GetWave.GetData_Func(mCurrentStageData.waveList[i]);
            if (waveData.isIceMonster)
            {
                ++mOffset;
                ++wave;
            }
            mWaveList.Add(waveData);
        }

        for (int i = wave; i < mCurrentStageData.waveList.Length; ++i)
        {
            waveData = DataBase_Manager.Instance.GetWave.GetData_Func(mCurrentStageData.waveList[i]);
            mWaveList.Add(waveData);
        }
        mNextStageMessage.SetValue(tempWaveCount);
        //if (wave != -1)
        //{
        //    waveData = DataBase_Manager.Instance.GetWave.GetData_Func(mCurrentStageData.waveList[wave]);
        //    mWaveList.Add(waveData);
        //}
        //
        //else
        //{
        //    foreach (var waveDatas in mCurrentStageData.waveList)
        //    {
        //        waveData = DataBase_Manager.Instance.GetWave.GetData_Func(waveDatas);
        //        mWaveList.Add(waveData);
        //    }
        //}
    
        StartWave().Forget();
    }

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