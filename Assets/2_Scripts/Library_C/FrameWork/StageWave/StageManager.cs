
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

public class StageManager : Singleton<StageManager>
{
    [GetComponent] private WayPoint mWayPoint;
    
    public StageData mCurrentStageData { get;private set; }
    private List<WaveData> mWaveList = new List<WaveData>();
    private WaveData mCurrentWaveData;
    
    private const float SPAWN_COOL_TIME = 1.5f;
    private const float NEXT_WAVE_TIME = 20.0f;
    
    private int mDeathBossCount = 0;

    public int WaveCount => mNextStageMessage.Value;
    
    private GameMessage<int> mNextStageMessage;
    public List<Monster> MonsterList = new List<Monster>();
    

    private CancellationTokenSource mCancellationToken;
    private TaskMessage mBossSpawnMessage;

    public int MaxStageCount { get; private set; }
    private bool mIsTutorial = false;
    private float mAfterBossKillRemainTime = 3;
    private int mBossWave;

    private float mWaveTime;

    /// <summary>
    ///  테스트용 스테이지 시작 코드
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Start()
    {
        //Time.timeScale = 1.5f;
        
    }

    protected override void AwakeInit()
    {
        MessageBroker.Default.Receive<EGameMessage>().
            Where(message => message == EGameMessage.BossDeath)
            .Subscribe(_ =>
            {
                if (mBossWave == mNextStageMessage.Value)
                {
                    mWaveTime = mAfterBossKillRemainTime;
                }
            }).AddTo(this);
        if (GameManager.Instance.IsTest && BackEndManager.Instance.IsUserTutorial)
        {
            EditInit();
        }

        else if (!BackEndManager.Instance.IsUserTutorial)
        {
            mIsTutorial = true;
            TutorialInitAsync().Forget();
        }

        else
        {
            Init();
        }
    }

    private void Init()
    {
        mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
        mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
        ObjectPoolManager.Instance.RegisterPoolingObject("Monster", 100);
        StageInit();
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
    }

    private void StageInit()
    {
        var currentStage = GameManager.Instance.CurrentStageData;
        string stageKey;
        if (currentStage != null)
        {
            stageKey = $"Stage_{(currentStage.ChapterNumber - 1) * 5 + (currentStage.StageNumber - 1)}";
        }
        else
        {
            stageKey = "Stage_100";
        }
        mCurrentStageData = DataBase_Manager.Instance.GetStage.GetData_Func(stageKey);
        foreach (var wave in mCurrentStageData.waveList)
        {
            var waveData = DataBase_Manager.Instance.GetWave.GetData_Func(wave);
            mWaveList.Add(waveData);
        }

        MaxStageCount = mWaveList.Count;
        StartWave().Forget();
    }

    private async UniTaskVoid StartWave()
    {
        await UniTask.WaitForSeconds(3f);
        mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
        MessageBroker.Default.Publish(mNextStageMessage);
        while (true)
        {
            mCurrentWaveData = mWaveList[mNextStageMessage.Value - 1];
            Debug.Log(mCurrentWaveData.Key);
            SpawnMonsters(mCurrentWaveData).Forget();
            if (mCurrentWaveData.isIceMonster)
            {
                continue;
            }

            if (mCurrentWaveData.isBoss)
            {
                MessageBroker.Default.Publish(mBossSpawnMessage);
                mBossWave = mNextStageMessage.Value;
            }

            if (mWaveList.Count == mNextStageMessage.Value)
            {
                await WaitAsync();
                break;
            }

            await WaitAsync();

            mNextStageMessage?.SetValue(mNextStageMessage.Value + 1);
            MessageBroker.Default.Publish(mNextStageMessage);

            async UniTask WaitAsync()
            {
                mWaveTime = mIsTutorial ? 15 : NEXT_WAVE_TIME;

                while (mWaveTime > 0)
                {
                    await UniTask.DelayFrame(1, cancellationToken: mCancellationToken.Token);
                    if (mIsTutorial)
                    {
                        await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger);
                    }

                    mWaveTime -= Time.deltaTime;
                }
            }
        }

    }

    private async UniTask SpawnMonsters(WaveData waveData)
    {
        if (mIsTutorial)
            await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger);

        int currentWave = mNextStageMessage.Value;

        for (int spawnCount = 0; spawnCount < waveData.spawnCount; spawnCount++)
        {
            var monster = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Monster, mWayPoint.GetWayPointPosition(0)).GetComponent<Monster>();;
            WaveStatData waveStateData = DataBase_Manager.Instance.GetWaveStat.GetData_Func(waveData.apply_stat);
            bool isLastBoss =  monster.IsLastBoss = mWaveList.Count == mNextStageMessage.Value;
            monster.SpawnMonster(waveData.monsterKey, mWayPoint, waveData.isBoss, waveStateData,waveData.weight,isLastBoss);
  
            MonsterList.Add(monster);
            float time = SPAWN_COOL_TIME;

            while (time > 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCancellationToken.Token);
                if (mIsTutorial)
                {
                    await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger);
                }

                time -= Time.deltaTime;
            }

            if (currentWave != mNextStageMessage.Value)
                break;
        }
    }
    
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
        //Time.timeScale = 2.5f;
        MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.DefaultResourceLoad)
            .Subscribe(message =>
            {
                mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
                mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
                ObjectPoolManager.Instance.RegisterPoolingObject("Monster", 100);
                StageInit();
            }).AddTo(this);
        MessageBroker.Default.Receive<EditMessage<int, int>>().Subscribe(message =>
        {
            GetStageAndWaveData(message.Value1, message.Value2);
        }).AddTo(this);
    }

    private void GetStageAndWaveData(int stage, int wave)
    {
        var currentStageData = DataBase_Manager.Instance.GetStage.GetData_Func($"Stage_{stage}");
        
        var waveData = DataBase_Manager.Instance.GetWave.GetData_Func(currentStageData.waveList[wave]);
        mWaveList.Add(waveData);

        StartWave().Forget();
    }

    #endregion

    protected override void ChangeSceneInit(Scene prev, Scene next)
    {
        CancelAndDisposeToken();
    }
    
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

    private async UniTask TutorialInitAsync()
    {
        mNextStageMessage = new GameMessage<int>(EGameMessage.StageChange, 0);
        mBossSpawnMessage = new TaskMessage(ETaskList.BossSpawn);
        MessageBroker.Default.Receive<GameMessage<bool>>().
            Where(message => message.Message == EGameMessage.TutorialRewind)
            .Subscribe(_ =>
            {
                mNextStageMessage?.SetValue(mNextStageMessage.Value - 1);
            }).AddTo(this);

        await UniTask.WaitUntil(() => IngameDataManager.Instance.TutorialTrigger);

        StageInit();
    }
}