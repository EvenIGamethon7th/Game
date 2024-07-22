using _2_Scripts.Game.Unit;
using _2_Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_UnitCanvas : MonoBehaviour
    {
        [SerializeField]
        private GameObject mPanel;
        private UI_UnitButton[] mButtons;
        [SerializeField]
        private Button mResume;
        [SerializeField]
        private Button mReroll;

        private int mRerollNum;
        private EUnitClass[] mUnitClasses;
        private EUnitRank[] mUnitRanks;

        private int mUnitClassCount;
        private int mUnitRankCount;

        void Start()
        {
            mButtons = mPanel.GetComponentsInChildren<UI_UnitButton>();

            mRerollNum = mButtons.Length;
            mUnitClasses = new EUnitClass[mRerollNum];
            mUnitRanks = new EUnitRank[mRerollNum];
            mUnitClassCount = Enum.GetNames(typeof(EUnitClass)).Length;
            mUnitRankCount = Enum.GetNames(typeof(EUnitRank)).Length;

            for (int i = 0; i < mRerollNum; ++i)
            {
                int num = i;
                mButtons[i].onClick.AddListener(() => Summon(num));
            }

            mResume.onClick.AddListener(Resume);
            mReroll.onClick.AddListener(Reroll);
            mPanel.SetActive(false);
            mReroll.gameObject.SetActive(false);

            if (GameManager.Instance.IsTest)
            {
                MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.DefaultResourceLoad).Subscribe(
                    _ =>
                    {
                        Init();
                    }).AddTo(this);
            }
            else
            {
                Init();
            }
  
        }

        private void Init()
        {
            for (int i = 0; i < mRerollNum; ++i)
            {
                mUnitClasses[i] = (EUnitClass)UnityEngine.Random.Range(1, mUnitClassCount);
                mUnitRanks[i] = (EUnitRank)UnityEngine.Random.Range(1, mUnitRankCount);
                //mButtons[i].UpdateGraphic(mUnitClasses[i], mUnitRanks[i]);
            }
            gameObject.SetActive(false);
            
        }
        private void Resume()
        {
            bool b = !mPanel.gameObject.activeSelf;
            //mResume.gameObject.SetActive(false);
            mPanel.SetActive(b);
            mReroll.gameObject.SetActive(b);
        }

        private void Reroll()
        {
            for (int i = 0; i < mRerollNum; ++i)
            {
                mUnitClasses[i] = (EUnitClass)UnityEngine.Random.Range(1, mUnitClassCount);
                mUnitRanks[i] = (EUnitRank)UnityEngine.Random.Range(1, mUnitRankCount);
                //mButtons[i].UpdateGraphic(mUnitClasses[i], mUnitRanks[i]);
                mButtons[i].gameObject.SetActive(true);
            }
        }

        private void Summon(int num)
        {
            // bool bSummon = MapManager.Instance.CreateUnit(mUnitClasses[num], mUnitRanks[num]);
            // if (bSummon)
            // {
            //     mButtons[num].gameObject.SetActive(false);
            // }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < mButtons.Length; ++i)
            {
                mButtons[i].onClick.RemoveAllListeners();
            }
            mResume.onClick.RemoveAllListeners();
        }
    }
}