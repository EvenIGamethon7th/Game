using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;

namespace Cargold.FrameWork.StageWave
{
    public class TutorialStageMode : IStageMode
    {
        public void AwakeInit(StageManager stageManager, GameMessage<int> nextStageMessage)
        {
            throw new System.NotImplementedException();
        }

        public void StageInit()
        {
            throw new System.NotImplementedException();
        }

        public UniTaskVoid StartWave()
        {
            throw new System.NotImplementedException();
        }

        public UniTaskVoid SpawnMonster(WaveData waveData, bool isEnd = false)
        {
            throw new System.NotImplementedException();
        }

        public UniTaskVoid SpawnMonster()
        {
            throw new System.NotImplementedException();
        }
    }
}