
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

    protected override void AwakeInit()
    {
        string path = Path.Combine(Application.persistentDataPath, "IsFirstConnect");
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            IsFirstConnect = JsonUtility.FromJson<bool>(data);
        }

        else
        {
            IsFirstConnect = true;
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
        if (mPrevName.CompareTo("Main") == 0)
        {
            mIngameItem.Clear();
        }

        mPrevName = next.name;
    }
}
