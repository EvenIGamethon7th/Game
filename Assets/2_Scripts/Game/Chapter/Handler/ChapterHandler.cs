using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.BackEndData.Stage;
using Cargold.FrameWork.BackEnd;
using Sirenix.Utilities;

namespace _2_Scripts.Game.Handler
{
    public class ChapterHandler
    {
        public int GetLastChapter()
        {
            var chapterData = BackEndManager.Instance.ChapterDataList
                .DefaultIfEmpty(null).FirstOrDefault(data => data.isClear == false);
            if (chapterData == null)
            {
                return 1;
            }
            return chapterData.ChapterNumber;
        }

        public ChapterData ChapterDataLoad(int chapterNum)
        {
            var chapterData = BackEndManager.Instance.
                ChapterDataList.Where(data => data.ChapterNumber == chapterNum)
                .DefaultIfEmpty(null)
                .LastOrDefault();
            if (chapterData == null)
            {
                var newChapter = new ChapterData
                {
                    ChapterNumber = chapterNum,
                    Star = 0,
                    StageList = new List<BackEndData.Stage.StageData>()
                };
                BackEndManager.Instance.ChapterDataSync(newChapter);
                return newChapter;
            }

            return chapterData;
        }
        
    }
}