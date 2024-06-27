
using System.Collections.Generic;
using _2_Scripts.Game.Monster;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{

    [SerializeField] private Vector2 mSpawnPoint;
    
    private StageData mCurrentStageData;
    private Queue<WaveData> mWaveQueue = new Queue<WaveData>();
    private WaveData mCurrentWaveData;
    
    private const float SPAWN_COOL_TIME = 1.0f;
    private const float NEXT_WAVE_TIME = 3.0f;
    public void StageInit(int stageKey)
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
        while (true)
        {
            if(mWaveQueue.Count == 0)
            {
                await UniTask.CompletedTask;
            }
            mCurrentWaveData = mWaveQueue.Dequeue();
            int spawnCount = 0;
            while (spawnCount >= mCurrentWaveData.spawnCount)
            {
                Monster monster =  ObjectPoolManager.Instance.CreatePoolingObject(mCurrentWaveData.monsterKey,mSpawnPoint).GetComponent<Monster>();
                monster.SpawnMonster(mCurrentWaveData.monsterKey);
                await UniTask.WaitForSeconds(SPAWN_COOL_TIME);
            }
            await UniTask.WaitForSeconds(NEXT_WAVE_TIME);
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    
}