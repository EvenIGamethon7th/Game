using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.Handler;
using _2_Scripts.UI.OutGame.Lobby;
using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using TMPro;
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
        
        [SerializeField]
        private UI_StageIndicator mStageIndicator;
        [SerializeField]
        private TextMeshProUGUI mChapterClearStarText;

        [SerializeField] private Button mStoryButton;
        
        private GameMessage<Chapter> mChapterMessage = new GameMessage<Chapter>(EGameMessage.ChapterChange,null);
        
        private void Start()
        {
            ChapterDataInit();
            LastChapterEnable();
            mStoryButton.onClick.AddListener(OnStoryBookClick);
        }

        private void ChapterDataInit()
        {
            for (int i = 1; i <= mChapterList.Count; i++)
            {
                var chapterData = mChapterHandler.ChapterDataLoad(i);
                mChapterList[chapterData.ChapterNumber].Init(chapterData);

                int i1 = i;
                mChapterButtonList[i-1].Init(()=>OnDrawMap(i1),i1);
                if (chapterData.isClear && i < mChapterList.Count)
                {
                    mChapterList[chapterData.ChapterNumber+1].OnDrawChapter();
                }
            }
            mChapterList[1].OnDrawChapter();
            // mChapterButtonList[0].Init(()=>OnDrawMap(1),1);
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
            mStageIndicator.OnChange(mChapterList[idx]);
            mChapterClearStarText.text = $"{mChapterList[idx].ChapterClearStar}/{mChapterList[idx].ChapterAllStar}";
            
            
        }

        private void OnStoryBookClick()
        {
            if (mChapterMessage.Value.ChapterNumber != 1)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("준비중 입니다.");
                return;
            }else if (mChapterList[mChapterMessage.Value.ChapterNumber].ChapterClearStar != mChapterList[mChapterMessage.Value.ChapterNumber].ChapterAllStar)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("별 15개 획득 시 \n 스토리 오픈!");
                return;
            }

            GameManager.Instance.CurrentDialog = mChapterMessage.Value.ChapterNumber;
            SceneLoadManager.Instance.SceneChange("DialogScene");
        }

        private void LastChapterEnable()
        {
            var chapterIdx = mChapterHandler.GetLastChapter();
            mChapterList[chapterIdx].gameObject.SetActive(true);
            OnDrawMap(chapterIdx);
        }
    }
}