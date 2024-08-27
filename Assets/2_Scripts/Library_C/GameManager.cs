
//  HP , 재화 , 씬 이동 등 여러가지 관리 목적

using System;
using System.Collections.Generic;
using System.IO;
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
    public bool IsTest { get; private set; } = true;
    public bool IsFirstConnect { get; private set; } = true;

    public _2_Scripts.Game.BackEndData.Stage.StageData CurrentStageData { get; private set; }
    public int CurrentDialog { get; set; } = -1;

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

    public List<CharacterInfo> UserCharacterList { get; private set; } = new List<CharacterInfo>();
    public List<MainCharacterInfo> MainCharacterList { get; private set; } = new List<MainCharacterInfo>();
    public MainCharacterInfo CurrentMainCharacter { get; set; }

    private Dictionary<int, CharacterInfo> mCharacterDict = new();

    public CharacterInfo GetCharacterInfo(int group)
    {
       mCharacterDict.TryGetValue(group, out var value);
        return value;
    }

    public void SetCurrentMainCharacter(string key)
    {
        CurrentMainCharacter = MainCharacterList.FirstOrDefault(x => x.name == key);
    }

    private void Start()
    {
        MessageBroker.Default.Receive<TaskMessage>()
            .Where(message => message.Task == ETaskList.CharacterDataResourceLoad).Subscribe(
                _ =>
                {
                    CharacterInfo info;
                    foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is CharacterInfo))
                    {
                        info = resource.Value as CharacterInfo;
                        UserCharacterList.Add(info);
                        mCharacterDict.TryAdd(info.CharacterEvolutions[1].GetData.Group, info);
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

    protected override void AwakeInit()
    {
        string path = Path.Combine(Application.persistentDataPath, "IsFirstConnect");
        Debug.Log(path);
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            IsFirstConnect = JsonUtility.FromJson<bool>(data);
        }

        else
        {
            IsFirstConnect = true;
            CurrentDialog = -1;
        }
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

    private string mPrevName = "";
    protected override void ChangeSceneInit(Scene prev, Scene next)
    {
        Time.timeScale = 1;
        if (mPrevName.CompareTo("Main") == 0 || mPrevName.CompareTo("Challenge") == 0)
        {
            mIngameItem.Clear();
        }

        mPrevName = next.name;
    }
}
