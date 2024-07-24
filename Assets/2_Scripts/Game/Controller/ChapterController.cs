using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Handler;
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace _2_Scripts.Game.Controller
{
    using Chapter = Chapter.Chapter;
    public class ChapterController : SerializedMonoBehaviour
    {
        ChapterHandler mChapterHandler = new ChapterHandler();
        [SerializeField]
        [Tooltip("챕터 넘버, 챕터 오브젝트")] 
        private Dictionary<int, Chapter> 
        mChapterList = new Dictionary<int, Chapter>();

        private void Start()
        {
            ChapterDataInit();
            LastChapterEnable();
  
        }

        private void ChapterDataInit()
        {
            for (int i = 1; i <= mChapterList.Count; i++)
            {
                var chapterData = mChapterHandler.ChapterDataLoad(i);
                mChapterList[chapterData.ChapterNumber].Init(chapterData);
                
            }
        }

        private void LastChapterEnable()
        {
            var chapterIdx = mChapterHandler.GetLastChapter();
            mChapterList[chapterIdx].gameObject.SetActive(true);
        }
    }
}