﻿using System;
using System.Collections.Generic;

namespace _2_Scripts.Game.BackEndData.Stage
{
    [Serializable]
    public class ChapterData
    {
        public int ChapterNumber;
        public int Star;
        public List<StageData> StageList;
        
    }
}