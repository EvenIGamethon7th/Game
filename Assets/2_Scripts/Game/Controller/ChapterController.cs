﻿using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Handler;
using _2_Scripts.UI.OutGame.Lobby;
using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        private List<UI_ChapterButton> mChapterButtonList = new List<UI_ChapterButton>();
        
        private GameMessage<Chapter> mChapterMessage = new GameMessage<Chapter>(EGameMessage.ChapterChange,null);
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

                var i1 = i+1;
                mChapterButtonList[i].Init(()=>OnDrawMap(i1));
                if (chapterData.isClear && i < mChapterList.Count-1)
                {
                    mChapterList[chapterData.ChapterNumber+1].OnDrawChapter();
                }
            }
            mChapterList[1].OnDrawChapter();
            mChapterButtonList[0].Init(()=>OnDrawMap(1));
        }

        private void OnDrawMap(int idx)
        {
            for (int i = 1; i <= mChapterList.Count; i++)
            {
                mChapterList[i].gameObject.SetActive(false);
            }
            mChapterList[idx].gameObject.SetActive(true);
            mChapterMessage.SetValue(mChapterList[idx]);
            MessageBroker.Default.Publish(mChapterMessage);
        }

        private void LastChapterEnable()
        {
            var chapterIdx = mChapterHandler.GetLastChapter();
            mChapterList[chapterIdx].gameObject.SetActive(true);
            OnDrawMap(chapterIdx);
        }
    }
}