using _2_Scripts.Game.ScriptableObject.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

public class IngameDataManager : Singleton<IngameDataManager>
{
    public enum EDataType
    {
        Hp,
        Level,
        EXP,
        Gold,
        LuckyCoin
    }
    private ReactiveProperty<int> mUserHp;

    private int mMaxHp = 100;

    public event Action<int> DamageHp;
    public event Action<int> HealHp;

    private ReactiveProperty<int> mUserLevel = new ReactiveProperty<int>(1);
    private ReactiveProperty<int> mUserExp = new ReactiveProperty<int>(0);
    private ReactiveProperty<int> mUserGold = new ReactiveProperty<int>(30);
    private ReactiveProperty<int> mUserLuckyCoin = new ReactiveProperty<int>(0);
    private System.Random mRandom = new System.Random();

    public List<CharacterInfo> UserCharacterList { get; private set; } = new List<CharacterInfo>();
    private List<MainCharacterInfo> mMainCharacterList = new List<MainCharacterInfo>();
    public MainCharacterInfo CurrentMainCharacter { get; set; }

    public readonly Dictionary<int, int> mExpTable = new Dictionary<int, int>
        {
            {1, 20},
            {2, 80},
            {3, 150},
            {4, 280},
            {5, 550},
            {6, 1}
        };

    public int GetMaxExp()
    {
        return mExpTable[mUserLevel.Value];
    }
    private readonly Dictionary<int, (int nomal, int rare, int epic)> mGradeRates = new Dictionary<int, (int general, int elite, int legendary)>
        {
            { 1, (100, 0, 0) },
            { 2, (95, 5, 0) },
            { 3, (90, 10, 0) },
            { 4, (80, 18, 2) },
            { 5, (70, 25, 5) },
            { 6, (60, 30, 10) }
        };

    private int GetGradeBasedOnRates()
    {
        if (!mGradeRates.ContainsKey(mUserLevel.Value))
        {
            return 1;
        }

        var rates = mGradeRates[mUserLevel.Value];
        int totalWeight = rates.nomal + rates.rare + rates.epic;
        int randomWeight = mRandom.Next(0, totalWeight);
        if (randomWeight < rates.nomal)
        {
            return 1;
        }
        if (randomWeight < rates.nomal + rates.rare)
        {
            return 2;
        }

        return 3;
    }

    private readonly Dictionary<int, int> mInterestTable = new()
    {
        {100,20},
        {300,30}
    };

    private const int ROUND_BONUS_GOLD_STAGE = 10;

    #region Item Manage
    private HashSet<EItemType> mIngameItem = new HashSet<EItemType>();

    public void UseItem(EItemType type)
    {
        mIngameItem.Add(type);
    }

    public void RemoveItem(EItemType type)
    {
        mIngameItem.Remove(type);
    }

    public bool IsUseItem(EItemType type)
    {
        return mIngameItem.Contains(type);
    }
    #endregion

    public void Subscribe<T>(T component, EDataType type, Action<int> action) where T : notnull, Component
    {
        switch (type)
        {
            case EDataType.Hp:
                mUserHp.Subscribe(hp => action(hp)).AddTo(component);
                break;

            case EDataType.Level:
                mUserLevel.Subscribe(level => action(level)).AddTo(component);
                break;

            case EDataType.EXP:
                mUserExp.Subscribe(exp => action(exp)).AddTo(component);
                break;

            case EDataType.Gold:
                mUserGold.Subscribe(gold => action(gold)).AddTo(component);
                break;

            case EDataType.LuckyCoin:
                mUserLuckyCoin.Subscribe(luckyCoin => action(luckyCoin)).AddTo(component);
                break;
        }
    }

    public void InitHp(int hp)
    {
        mMaxHp = hp;
        mUserHp = new ReactiveProperty<int>(hp);
    }

    public void UpdateMoney(string moneyKey, int value)
    {
        MoneyData money = DataBase_Manager.Instance.GetMoney.GetData_Func(moneyKey);
        UpdateMoney(money.Type, value);
    }

    public void UpdateMoney(EMoneyType moneyType, int value)
    {
        switch (moneyType)
        {
            case EMoneyType.Gold:
                mUserGold.Value += value;
                break;
            case EMoneyType.GoldKey:
                mUserLuckyCoin.Value += value;
                break;
        }
    }

    public void UpdateUserHp(int hp)
    {
        if (hp > 0)
            DamageHp?.Invoke(hp);
        else
            HealHp?.Invoke(hp);

        mUserHp.Value = mUserHp.Value - hp <= mMaxHp ? mUserHp.Value - hp : mMaxHp;
    }

    public void AddExp(int exp)
    {
        if (mUserLevel.Value == 6)
        {
            mUserExp.Value = 1;
            return;
        }
        mUserExp.Value += exp;
        if (mExpTable[mUserLevel.Value] <= mUserExp.Value)
        {
            mUserExp.Value -= mExpTable[mUserLevel.Value];
            mUserLevel.Value++;
        }
    }

    public CharacterInfo RandomCharacterCardOrNull()
    {
        if (UserCharacterList.Count == 0)
        {
            return null;
        }

        int randomIdx = mRandom.Next(UserCharacterList.Count);
        return UserCharacterList[randomIdx];
    }

    public CharacterData GetRandomCharacterData(CharacterInfo characterInfo)
    {
        int grade = GetGradeBasedOnRates();
        return characterInfo.CharacterEvolutions[grade].GetData;
    }

    protected override void ChangeSceneInit(Scene prev, Scene next)
    {
        mUserLevel.Value = 1;
        mUserGold.Value = 30;
        mUserLuckyCoin.Value = 0;
        mUserExp.Value = 0;
    }
}
