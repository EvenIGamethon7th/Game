using System;

namespace _2_Scripts.Game.BackEndData.Stage
{
    [Serializable]
    public class StageData
    {
        public int ChapterNumber;
        public int StageNumber;
        public int Star;
        public bool IsClear;
        public bool IsLastStage;
    }
}