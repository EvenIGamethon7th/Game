using _2_Scripts.Game.Unit;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Summon
{
    public class UnitCanvas : MonoBehaviour
    {
        [SerializeField]
        private GameObject mPanel;
        private UnitButton[] mButtons;
        [SerializeField]
        private Button mResume;

        private int mRerollNum;
        private EUnitClass[] mUnitClasses;
        private EUnitRank[] mUnitRanks;

        private int mUnitClassCount;
        private int mUnitRankCount;

        void Start()
        {
            mButtons = mPanel.GetComponentsInChildren<UnitButton>();

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
            mPanel.SetActive(false);
        }

        private void Resume()
        {
            mResume.gameObject.SetActive(false);
            for (int i = 0; i < mRerollNum; ++i)
            {
                mUnitClasses[i] = (EUnitClass)UnityEngine.Random.Range(1, mUnitClassCount);
                mUnitRanks[i] = (EUnitRank)UnityEngine.Random.Range(1, mUnitRankCount);
                mButtons[i].Text.text = $"{mUnitClasses[i]} {mUnitRanks[i]}";
                mButtons[i].UpdateGraphic(mUnitClasses[i], mUnitRanks[i]);
            }
            mPanel.SetActive(true);
        }

        private void Summon(int num)
        {
            bool bSummon = MapManager.Instance.CreateUnit(mUnitClasses[num], mUnitRanks[num]);
            if (bSummon)
            {
                mPanel.SetActive(false);
                mResume.gameObject.SetActive(true);
            }
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