using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.BackEndData.Mission;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cargold.FrameWork.BackEnd
{
    
    using _2_Scripts.Game.BackEndData.Stage;
    public enum ECurrency
    {
        [Description("FT")]
        Father, // Feather
        [Description("DI")]
        Diamond, // Diamond
    }
    public class BackEndManager : Singleton<BackEndManager>
    {

        private PlayFabAuthService mAuthService;

        public Dictionary<ECurrency, ReactiveProperty<int>> UserCurrency { get; private set; } =  new Dictionary<ECurrency, ReactiveProperty<int>>
        {
            {ECurrency.Father,new ReactiveProperty<int>(0)},
            {ECurrency.Diamond,new ReactiveProperty<int>(0)}
        };
        public Dictionary<string, SpawnMission> UserMission { get; private set; } = new Dictionary<string, SpawnMission>();
        public List<ChapterData> ChapterDataList { get; private set; } = new();

        public Dictionary<string, MainCharacterData> UserMainCharacterData { get; private set; } = new Dictionary<string, MainCharacterData>();

        public List<SpawnMission> SpawnMissions()
        {
            List<CharacterData> characterDatas = new List<CharacterData>();
            GameManager.Instance.UserCharacterList.ForEach(characterData =>
            {
                characterDatas.Add(characterData.CharacterEvolutions[3].GetData);
            });
            characterDatas.ForEach(data =>
            {
                if(UserMission.TryGetValue(data.Key,out var mission))
                {
                    mission.CharacterKey = data.Key;
                }
                else
                {
                    UserMission.Add(data.Key,new SpawnMission(data.Key));
                }
            });
            return UserMission.Values.ToList();
        }

        protected override void ChangeSceneInit(Scene prev, Scene next)
        {
            
        }

        public void ChapterDataSync(ChapterData chapterData)
        {
            ChapterDataList.Add(chapterData);
        }
        
        public void OnLogin(Action callback)
        {
            LoginAsync(callback).Forget();
        }
        public void AddCurrencyData(ECurrency currency, int amount)
        {
            UserCurrency[currency].Value += amount;
            PublishCurrencyData(currency,amount);
        }
        
        private async UniTaskVoid LoginAsync(Action successCallback)
        {
            mAuthService.Authenticate(Authtypes.Silent);
            await UniTask.WaitUntil(() => mAuthService.SessionTicket != null);
            //맨 마지막에 
            SyncCurrencyDataFromServer(successCallback.Invoke).Forget();
        }
        
        /// <summary>
        /// 서버에서 데이터를 동기화 해야할 때.
        /// </summary>
        private async UniTaskVoid SyncCurrencyDataFromServer(Action successCallback)
        {
            await ReceiveCurrencyData();
            await LoadChapterData();
            await ReceiveMissionData();
            await ReceiveMainCharacterData();
            
            successCallback?.Invoke();
        }

        private async UniTask ReceiveCurrencyData()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (result) =>
            {
                List<ECurrency> keys = UserCurrency.Keys.ToList();
                foreach (var item in keys)
                {
                    string serverKey = Utils.GetEnumDescription(item);
                    UserCurrency[item].Value = result.VirtualCurrency[serverKey];
                }
                tcs.TrySetResult();
            }, (error)=>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });

            await tcs.Task;
        }

        public void SaveCharacterData()
        {
            string jsonData = JsonConvert.SerializeObject(ChapterDataList);
            PublishCharacterData(new Dictionary<string, string> { { "ChapterData", jsonData }, { "MissionData", JsonConvert.SerializeObject(UserMission)}, 
                { "MainCharacterData", JsonConvert.SerializeObject(UserMainCharacterData) }});
        }

        private async UniTask LoadChapterData()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), (result) =>
            {
                if (result.Data.TryGetValue("ChapterData", out var data))
                {
                    ChapterDataList = JsonConvert.DeserializeObject<List<ChapterData>>(data.Value);
                }
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });

            await tcs.Task;
        }
        
        private async UniTask ReceiveMissionData()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), (result) =>
            {
                if (result.Data.TryGetValue("MissionData", out var data))
                {
                    UserMission = JsonConvert.DeserializeObject<Dictionary<string, SpawnMission>>(data.Value);
                }
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });

            await tcs.Task;
        }

        private async UniTask ReceiveMainCharacterData()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), (result) =>
            {
                if(result.Data.TryGetValue("MainCharacterData",out var data))
                {
                    UserMainCharacterData = JsonConvert.DeserializeObject<Dictionary<string, MainCharacterData>>(data.Value);
                }
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });
            await tcs.Task;
        }
        
        private void PublishCharacterData(Dictionary<string,string> data)
        {
           var request = new ExecuteCloudScriptRequest
           {
              FunctionName = "UpdateChapterData",
              FunctionParameter = new {data},
              GeneratePlayStreamEvent = true
           };

           PlayFabClientAPI.ExecuteCloudScript(request, (result) =>
           {
               Debug.Log("PublishChapterData");
           }, (error) =>
           {
               ErrorLog(error);
           });

        }
        
        private void PublishCurrencyData(ECurrency currency,int value)
        {
            string serverKey = Utils.GetEnumDescription(currency);
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "UpdateCurrency",
                FunctionParameter = new Dictionary<string,object>{ {"Currency",serverKey},{"Value",value} },
                GeneratePlayStreamEvent = true
            }, (result) =>
            {
                Debug.Log("UpdateCurrencyFromServer");
            }, (error) =>
            {
                ErrorLog(error);
            });
        }

        private void ErrorLog(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }
        
        /// <summary>
        /// 유니티 종료 전 서버에 데이터 전송
        /// </summary>
        private void OnApplicationQuit()
        {
            SaveCharacterData();
        }

        public void AddSpawnMission(CharacterData characterData)
        {
            if (UserMission.TryGetValue(characterData.Key, out var mission))
            {
                mission.AddSpawnCount(1);
            }
            else
            {
                UserMission.Add(characterData.Key,new SpawnMission(characterData.Key,spawnCount: 1));
            }
        }

        protected override void AwakeInit()
        {
            mAuthService = new PlayFabAuthService();
            MessageBroker.Default.Receive<TaskMessage>().Where(message => message.Task == ETaskList.MapDataResourceLoad)
                .Take(1)
                .Subscribe(_ =>
                {
                    foreach (var mainCharacter in GameManager.Instance.MainCharacterList)
                    {
                        UserMainCharacterData.Add(mainCharacter.name, new MainCharacterData(mainCharacter.name, 1, false, EGetType.Lock));
                    }
                }).AddTo(this);
        }
    }
}