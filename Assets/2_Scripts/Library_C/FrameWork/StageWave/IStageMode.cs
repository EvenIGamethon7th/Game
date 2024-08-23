using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;

namespace Cargold.FrameWork.StageWave
{
    public interface IStageMode
    {
        public void AwakeInit(StageManager stageManager,GameMessage<int> nextStageMessage);

        public void StageInit();

        public UniTaskVoid StartWave();
        
        public UniTaskVoid SpawnMonster(WaveData waveData, bool isEnd = false);
    }
}