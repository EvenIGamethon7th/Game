using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

[DefaultExecutionOrder(-10)]
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

    public int MaxHp { get; private set; } = 100;

    public event Action<int> DamageHp;
    public event Action<int> HealHp;

    private ReactiveProperty<int> mUserLevel = new ReactiveProperty<int>(1);
    private ReactiveProperty<int> mUserExp = new ReactiveProperty<int>(0);
    private ReactiveProperty<int> mUserGold = new ReactiveProperty<int>(30);
    private ReactiveProperty<int> mUserLuckyCoin = new ReactiveProperty<int>(0);

    public int CurrentHp => mUserHp.Value;
    public int CurrentLevel => mUserLevel.Value;
    public int CurrentGold => mUserGold.Value;
    public int CurrentLuckyCoin => mUserLuckyCoin.Value;

    private System.Random mRandom = new System.Random();

    public List<CharacterInfo> UserCharacterList { get; private set; } = new List<CharacterInfo>();
    public bool TutorialTrigger { get; private set; } = false;

    public readonly Dictionary<int, int> mExpTable = new Dictionary<int, int>
        {
            {1, 60},
            {2, 80},
            {3, 120},
            {4, 200},
            {5, 300},
            {6, 1}
        };

    private readonly Dictionary<int, (int nomal, int rare, int epic)> mGradeRates = new Dictionary<int, (int general, int elite, int legendary)>
        {
            { 1, (100, 0, 0) },
            { 2, (95, 5, 0) },
            { 3, (90, 10, 0) },
            { 4, (85, 13, 2) },
            { 5, (80, 15, 5) },
            { 6, (70, 20, 10) }
        };

    protected override void AwakeInit()
    {
        IsDontDestroy = false;
        if (!GameManager.Instance.IsTest)
        {
            foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is CharacterInfo))
            {
                UserCharacterList.Add(resource.Value as CharacterInfo);
            }
        }

        else
        {
            MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.CharacterDataResourceLoad).Subscribe(
                _ =>
                {
                    foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is CharacterInfo))
                    {
                        UserCharacterList.Add(resource.Value as CharacterInfo);
                    }
                }).AddTo(this);
        }

        MaxHp = GameManager.Instance.IsUseItem(EItemType.HpUp) ? 125 : 100;
        mUserHp = new ReactiveProperty<int>(MaxHp);

        if (!BackEndManager.Instance.IsUserTutorial)
        {
            MessageBroker.Default.Receive<GameMessage<bool>>().
                Where(message => message.Message == EGameMessage.Tutorial)
                .Subscribe(val =>
                {
                    TutorialTrigger = val.Value;
                }).AddTo(this);
            mUserLevel.Value = 3;
            mUserExp.Value = 20;
            mUserGold.Value = 400;
            GameManager.Instance.UseItem(EItemType.Lecturer1st);
            SceneLoadManager.Instance.SceneClear += Clear;

            void Clear()
            {
                SceneLoadManager.Instance.SceneClear -= Clear;
                GameManager.Instance.RemoveItem(EItemType.Lecturer1st);
            }
        }
    }

    private void Start()
    {
        MessageBroker.Default.Receive<GameMessage<int>>().Where(message => message.Message == EGameMessage.StageChange)
            .Subscribe(message =>
            {
                // 첫 시작은 리턴
                if (message.Value <= 1)
                {
                    return;
                }
                int interest = 0;
                foreach (var tableValue in mInterestTable)
                {
                    if (tableValue.Key <= mUserGold.Value)
                    {
                        interest = Mathf.Max(interest, tableValue.Value);
                    }
                }

                UpdateMoney(EMoneyType.Gold, ROUND_BONUS_GOLD_STAGE + interest);
                AddExp(20);
            }).AddTo(this);
    }

    public int GetMaxExp()
    {
        return mExpTable[mUserLevel.Value];
    }

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
        {50,10},
        {100,20}
    };

    private const int ROUND_BONUS_GOLD_STAGE = 10;

    public void Subscribe<T>(T component, EDataType type, Action<int> action, bool isTake1 = false) where T : notnull, Component
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
        {
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Castle_Damage);
            DamageHp?.Invoke(hp);
        }
        else
            HealHp?.Invoke(hp);

        mUserHp.Value = mUserHp.Value - hp <= MaxHp ? mUserHp.Value - hp : MaxHp;
        if (mUserHp.Value < 0)
            mUserHp.Value = 0;
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

    public CharacterData GetCharacterData(CharacterInfo characterInfo, int grade)
    {
        return characterInfo.CharacterEvolutions[grade].GetData;
    }

}
