
//  HP , 재화 , 씬 이동 등 여러가지 관리 목적

using System;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Utils;
using Spine.Unity;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;
using Random = System.Random;

public enum EItemType
{
    Lecturer1st,
    Lecturer2nd,
    HpUp,
}

public class GameManager : Singleton<GameManager>
{

    public ReactiveProperty<float> UserHp { get; private set; } = new ReactiveProperty<float>(100);

    private int mMaxHp = 100;

    public event Action<float> DamageHp;
    public event Action<float> HealHp;
    // 학년
    public ReactiveProperty<int> UserLevel { get; private set; } = new ReactiveProperty<int>(1);

    public ReactiveProperty<int> UserExp { get; private set; } = new ReactiveProperty<int>(0);

    public ReactiveProperty<int> UserGold { get; private set; } = new ReactiveProperty<int>(30);

    public ReactiveProperty<int> UserLuckyCoin { get; private set; } = new ReactiveProperty<int>(0);

    public bool IsTest { get; private set; } = true;

    public _2_Scripts.Game.BackEndData.Stage.StageData CurrentStageData { get; private set; }

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

    public void SetCurrentStageData(_2_Scripts.Game.BackEndData.Stage.StageData stageData)
    {
        CurrentStageData = stageData;
    }
    
    public void NotTestMode()
    {
        IsTest = false;
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

        UserHp.Value = UserHp.Value - hp <= mMaxHp ? UserHp.Value - hp : mMaxHp;
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
    public List<MainCharacterInfo> MainCharacterList { get; private set; } = new List<MainCharacterInfo>();
    public MainCharacterInfo CurrentMainCharacter { get; set; }

    public void SetCurrentMainCharacter(string key)
    {
        CurrentMainCharacter = MainCharacterList.FirstOrDefault(x => x.name == key);
    }

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
        return mExpTable[UserLevel.Value];
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

    private readonly Dictionary<int, int> mInterestTable = new()
    {
        {100,20},
        {300,30}
    };

    private const int ROUND_BONUS_GOLD_STAGE = 10;
    private void Start()
    {
        MessageBroker.Default.Receive<GameMessage<int>>().Where(message => message.Message == EGameMessage.StageChange)
            .Subscribe(message =>
            {
                // 첫 시작은 리턴
                if (message.Value == 0)
                {
                    return;
                }
                int interest = 0;
                foreach (var tableValue in mInterestTable)
                {
                    if (tableValue.Key < UserGold.Value)
                    {
                        interest = Mathf.Max(interest, tableValue.Value);
                    }
                }

                UpdateMoney(EMoneyType.Gold, ROUND_BONUS_GOLD_STAGE + interest);
                AddExp(20);
            }).AddTo(this);

        MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.CharacterDataResourceLoad).Subscribe(
                _ =>
                {
                    foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is CharacterInfo))
                    {
                        UserCharacterList.Add(resource.Value as CharacterInfo);
                    }
                }).AddTo(this);

        MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.MainCharacterDataResourceLoad).Subscribe(
                _ =>
                {
                    foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is MainCharacterInfo))
                    {
                        MainCharacterList.Add(resource.Value as MainCharacterInfo);
                    }
                }).AddTo(this);
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

    protected override void ChangeSceneInit(Scene prev, Scene next)
    {
        //TOdo UserHp Max HP는 BackEnd에서 가져오는 형태로 수정해야할듯 차후
        UserHp.Value = mMaxHp;
        UserLevel.Value = 1;
        UserGold.Value = 30;
        UserLuckyCoin.Value = 0;
        UserExp.Value = 0;
    }
}
