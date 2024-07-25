﻿using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.BackEndData.Stage;
using UnityEngine;
namespace _2_Scripts.Game.Chapter
{
    using  StageData = BackEndData.Stage.StageData;
    public class Chapter : MonoBehaviour
    {
        [SerializeField]
        private List<Stage> mStageList = new List<Stage>();
        
        private List<StageData> mStageDataList = new ();
        
        [SerializeField]
        private GameObject mLockObject;

        public int ChapterClearStar => mStageDataList.Sum(data => data.Star);
        public int ChapterAllStar => mStageDataList.Count * 3;
        public void Init(ChapterData chapterData)
        {
            // 초기 데이터 만들어줌
            for (int i = 0; i < mStageList.Count; i++)
            {
                if (i < chapterData.StageList.Count && chapterData.StageList[i] != null)
                {
                    mStageDataList.Add(chapterData.StageList[i]);
                    continue;
                }
                
                StageData stageData = new StageData
                {
                    ChapterNumber = chapterData.ChapterNumber,
                    StageNumber = i+1,
                    Star = 0,
                    IsClear = false,
                    IsLastStage = i+1 == mStageList.Count 
                };
                mStageDataList.Add(stageData);
            }
            chapterData.StageList = mStageDataList;
          
            // 스테이지 초기화
          
            for (int i = 0; i < mStageDataList.Count; i++)
            {
                mStageList[i].Init(mStageDataList[i]);
            }
            var lastIdx = mStageList.FirstOrDefault(data => data.IsClear == false);
            if (lastIdx == null)
            {
                chapterData.isClear = true;
            }
            lastIdx?.LastSelectIdx();
        }

        public void OnDrawChapter()
        {
            mLockObject.SetActive(false);
        }
    }
}