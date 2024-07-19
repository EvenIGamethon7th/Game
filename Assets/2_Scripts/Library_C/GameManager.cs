
//  HP , 재화 , 씬 이동 등 여러가지 관리 목적

using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Utils;
using Spine.Unity;
using UniRx;
using UnityEngine;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;
using Random = System.Random;

public class GameManager : Singleton<GameManager>
{

    public ReactiveProperty<float> UserHp { get; private set; } = new ReactiveProperty<float>(100);

    public event Action<float> DamageHp;
    public event Action<float> HealHp;
    // 학년
    public ReactiveProperty<int> UserLevel { get; private set; } = new ReactiveProperty<int>(1);

    public ReactiveProperty<int> UserExp { get; private set; } = new ReactiveProperty<int>(0);

    public ReactiveProperty<int> UserGold { get; private set; } = new ReactiveProperty<int>(1000);

    public ReactiveProperty<int> UserLuckyCoin { get; private set; } = new ReactiveProperty<int>(5);

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
                UserGold.Value += value;
                break;
            case EMoneyType.GoldKey:
                UserLuckyCoin.Value += value;
                break;
        }
    }

    public void UpdateUserHp(float hp)
    {
        if (hp > 0)
            DamageHp?.Invoke(hp);
        else
            HealHp?.Invoke(hp);
        UserHp.Value -= hp;
    }

    public void AddExp(int exp)
    {
        if (UserLevel.Value == 6)
        {
            UserExp.Value = 1;
            return;
        }
        UserExp.Value += exp;
        if (mExpTable[UserLevel.Value] <= UserExp.Value)
        {
            UserExp.Value -= mExpTable[UserLevel.Value];
            UserLevel.Value++;
        }
    }



    public List<CharacterInfo> UserCharacterList { get; private set; } = new List<CharacterInfo>();
    private List<MainCharacterInfo> mMainCharacterList = new List<MainCharacterInfo>();
    public MainCharacterInfo CurrentMainCharacter { get; private set; }

    public readonly Dictionary<int, int> mExpTable = new Dictionary<int, int>
        {
            {1, 10},
            {2, 10},
            {3, 40},
            {4, 60},
            {5, 90},
            {6, 1}
        };

    public int GetMaxExp()
    {
        return mExpTable[UserLevel.Value];
    }
    private readonly Dictionary<int, (int nomal, int rare, int epic)> mGradeRates = new Dictionary<int, (int general, int elite, int legendary)>
        {
            { 1, (100, 0, 0) },
            { 2, (90, 10, 0) },
            { 3, (80, 20, 0) },
            { 4, (70, 25, 5) },
            { 5, (55, 35, 10) },
            { 6, (40, 45, 15) }
        };

    private int GetGradeBasedOnRates()
    {
        if (!mGradeRates.ContainsKey(UserLevel.Value))
        {
            return 1;
        }

        var rates = mGradeRates[UserLevel.Value];
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

    private void Start()
    {
        MessageBroker.Default.Receive<GameMessage<int>>().Where(message => message.Message == EGameMessage.StageChange)
            .Subscribe(message =>
            {
                AddExp(5);
            });

        MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.CharacterDataResourceLoad).Subscribe(
                _ =>
                {
                    foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is CharacterInfo))
                    {
                        UserCharacterList.Add(resource.Value as CharacterInfo);
                    }
                });

        MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.MainCharacterDataResourceLoad).Subscribe(
                _ =>
                {
                    foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is MainCharacterInfo))
                    {
                        mMainCharacterList.Add(resource.Value as MainCharacterInfo);
                    }
                    CurrentMainCharacter = mMainCharacterList[0];
                });
    }
    private Random mRandom = new Random();
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
}
