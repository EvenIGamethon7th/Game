using _2_Scripts.Game.Unit;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Summon
{
    public class UnitPannel : MonoBehaviour
    {
        private UnitButton[] mButtons;
        [SerializeField]
        private Button mReroll;

        private int mRerollNum;
        private EUnitClass[] mUnitClasses;
        private EUnitRank[] mUnitRanks;

        private int mUnitClassCount;
        private int mUnitRankCount;

        void Start()
        {
            mButtons = GetComponentsInChildren<UnitButton>();

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
            mReroll.onClick.AddListener(Reroll);
            Reroll();
        }

        private void Reroll()
        {
            for (int i = 0; i < mRerollNum; ++i)
            {
                mButtons[i].gameObject.SetActive(true);
                mUnitClasses[i] = (EUnitClass)UnityEngine.Random.Range(1, mUnitClassCount);
                mUnitRanks[i] = (EUnitRank)UnityEngine.Random.Range(1, mUnitRankCount);
                mButtons[i].Text.text = $"{mUnitClasses[i]} {mUnitRanks[i]}";
            }
        }

        private void Summon(int num)
        {
            MapManager.Instance.UnitCreate(mUnitClasses[num], mUnitRanks[num]);
            mButtons[num].gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < mButtons.Length; ++i)
            {
                mButtons[i].onClick.RemoveAllListeners();
            }
            mReroll.onClick.RemoveAllListeners();
        }
    }
}