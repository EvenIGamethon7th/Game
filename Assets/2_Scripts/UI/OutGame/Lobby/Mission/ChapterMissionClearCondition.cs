using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using System.Linq;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    [CreateAssetMenu(menuName = "ScriptableObject/Mission/ChapterClearCondition",fileName = "Chapter_")]
    public class ChapterMissionClearCondition : MissionClearCondition
    {
        public int ChapterIndex;
        public override bool IsClear()
        {
           var clearStar = BackEndManager.Instance.ChapterDataList[ChapterIndex -1].StageList.Sum(x => x.Star);
           if(clearStar >= BackEndManager.Instance.ChapterDataList[ChapterIndex - 1].StageList.Count * 3)
           {
               return true;
           }

           return false;
        }

        public override int GetCurrentProgress()
        {
          return  BackEndManager.Instance.ChapterDataList[ChapterIndex-1].StageList.Sum(x => x.Star);
        }

        public override int GetMaxProgress()
        {
            return BackEndManager.Instance.ChapterDataList[ChapterIndex-1].StageList.Count * 3;
        }
    }
}