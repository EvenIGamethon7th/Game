
using System;
using System.Collections.Generic;
using _2_Scripts.Game.Monster;
using _2_Scripts.Utils;
using Cargold;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{

    [SerializeField] private Vector2 mSpawnPoint;
    
    private StageData mCurrentStageData;
    private Queue<WaveData> mWaveQueue = new Queue<WaveData>();
    private WaveData mCurrentWaveData;
    
    private const float SPAWN_COOL_TIME = 1.0f;
    private const float NEXT_WAVE_TIME = 3.0f;


    /// <summary>
    ///  테스트용 스테이지 시작 코드
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Start()
    {
        MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.ResourceLoad).Subscribe(
            _ =>
            {
                StageInit(TableDataKey_C.Stage_Stage_0);
                
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
        while (mWaveQueue.Count != 0)
        {
            mCurrentWaveData = mWaveQueue.Dequeue();
            int spawnCount = 0;
            while (spawnCount < mCurrentWaveData.spawnCount)
            {
                Monster monster =  ObjectPoolManager.Instance.CreatePoolingObject("Monster",mSpawnPoint).GetComponent<Monster>();
                monster.SpawnMonster(mCurrentWaveData.monsterKey);
                spawnCount++;
                await UniTask.WaitForSeconds(SPAWN_COOL_TIME);
            }
            await UniTask.WaitForSeconds(NEXT_WAVE_TIME);
            Debug_C.Log_Func($"다음 Wave 시작 {mCurrentWaveData.Key}");
        }
    }
    
    
}