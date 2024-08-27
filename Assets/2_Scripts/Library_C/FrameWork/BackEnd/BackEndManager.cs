using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using _2_Scripts.Game.BackEndData.Enchant;
using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.BackEndData.Mission;
using _2_Scripts.Game.BackEndData.Shop;
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
        [Description("TI")]
        Ticket,
        [Description("DR")]
        DailyReward
    }
    
    public enum EEnchantClassType
    {
        [Description("Lo_ClassType_1")]
        Warrior,
        [Description("Lo_ClassType_3")]
        Archer,
        [Description("Lo_ClassType_2")]
        Mage
    }
    public class BackEndManager : Singleton<BackEndManager>
    {

        private PlayFabAuthService mAuthService;

        public Dictionary<ECurrency, ReactiveProperty<int>> UserCurrency { get; private set; } =  new Dictionary<ECurrency, ReactiveProperty<int>>
        {
            {ECurrency.Father,new ReactiveProperty<int>(0)},
            {ECurrency.Diamond,new ReactiveProperty<int>(0)},
            {ECurrency.Ticket,new ReactiveProperty<int>(0)},
            {ECurrency.DailyReward,new ReactiveProperty<int>(0)}
        };
        public Dictionary<string, SpawnMission> UserMission { get; private set; } = new Dictionary<string, SpawnMission>();
        public List<ChapterData> ChapterDataList { get; private set; } = new();

        public Dictionary<string, MainCharacterData> UserMainCharacterData { get; private set; } = new Dictionary<string, MainCharacterData>();
        public Dictionary<string, PlayMission> UserPlayMission { get; private set; } = new Dictionary<string, PlayMission>();
        public Dictionary<EEnchantClassType, CharacterEnchantData> UserEnchantData { get; private set; } = new();
        public List<CatalogItem> CatalogItems { get; private set; } = new List<CatalogItem>();
        public List<StoreItem> PublicStoreItems { get; private set; } = new List<StoreItem>();
        public List<ItemInstance> UserInventory { get; private set; }= new List<ItemInstance>();
        public Dictionary<string,FreeRewardData> UserFreeRewardData { get; private set; } = new Dictionary<string, FreeRewardData>();
        
        public SurviveMission UserServiceMission { get; private set; } = new SurviveMission();
        
        public int UserDailyReward { get; private set; } = 0;
        
        public bool IsUserTutorial { get; set; } = false;

        public bool IsSelectMainCharacter { get; set; } = false;
        public string GetUserNickName()
        {
            return PlayFabAuthService.NickName;
        }

        private bool mbIsLoadData = false;
        public CharacterEnchantData GetEnchantData(EEnchantClassType classType)
        {
            if(UserEnchantData.TryGetValue(classType,out var data))
            {
                return data;
            }
            UserEnchantData[classType] = new CharacterEnchantData(classType,0,false);
            return UserEnchantData[classType];
        }
        

        public CatalogItem GetStoreItem(string itemId)
        {
            return CatalogItems.Find(item => item.ItemId == itemId);
        }
        public ItemInstance GetInventoryItem(string itemId)
        {
            return UserInventory.Find(item=>item.ItemId == itemId);
        }
        public void PurchasePopUpItem(StoreItem item,Action successCallback,Action failedCallBack)
        {
            var itemPrice = item.VirtualCurrencyPrices.Values.First();
            if(itemPrice > UserCurrency[ECurrency.Diamond].Value)
            {
                failedCallBack?.Invoke();
                return;
            }
            successCallback?.Invoke();
            UserCurrency[ECurrency.Diamond].Value -= (int)itemPrice;
            PurchaseItem(item.ItemId,itemPrice,item.VirtualCurrencyPrices.Keys.First(),failedCallBack,"PublicShop");
        }
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
            if (UserMission.TryGetValue("Character_51", out var mission))
            {
                mission.CharacterKey = "Character_51";
            }
            else
            {
                UserMission.Add("Character_51",new SpawnMission("Character_51",100,true,true));   
            }
            return UserMission.Values.OrderByDescending(m => m.IsEquip).ThenByDescending(m=>m.IsGet).ToList();
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

        public void CheckVersion(Action successCallback,Action failCallback)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "VersionCheck",
                FunctionParameter = new {version = Application.version},
                GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(request, (result) =>
            {
                var functionResult = result.FunctionResult as IDictionary<string, object>;
                if ((bool)functionResult["success"])
                {
                    successCallback?.Invoke();
                }
                else
                {
                    failCallback?.Invoke();
                }
            },ErrorLog);
        }
        public void ChangeDisplayName(string nickName,Action successCallback,Action<string> failCallback)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "ChangeDisplayName",
                FunctionParameter = new {DisplayName = nickName},
                GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(request, (result) =>
            {
                var functionResult = result.FunctionResult as IDictionary<string, object>;
                if ((bool)functionResult["success"])
                {
                    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                    {
                        DisplayName = (string)functionResult["nickName"]
                    }, (result2) =>
                    {
                        PlayFabAuthService.NickName = result2.DisplayName;
                        successCallback?.Invoke();
                    }, (error) =>
                    {
                        var message = error.ErrorMessage;
                        if (error.Error == PlayFabErrorCode.NameNotAvailable)
                        {
                            message = "중복된 닉네임 입니다.";
                        }
                        failCallback?.Invoke(message);
                    });
                }
                else
                {
                    failCallback?.Invoke((string)functionResult["errorMessage"]);
                }
                
            },ErrorLog);
        }
        
        private async UniTaskVoid LoginAsync(Action successCallback)
        {
            if (mAuthService == null)
            {
                mAuthService = new PlayFabAuthService();
            }
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
            await ReceiveEnchantData();
            await ReceiveStoreItems("PublicShop");
            await ReceiveInventory();
            await FetchCatalogItems();
            await ReceivePlayerData();
            await ReceiveFreeRewardData();
            mbIsLoadData = true;
            successCallback?.Invoke();
            AutoSave().Forget();
        }

        
        private async UniTask ReceiveFreeRewardData()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), (result) =>
            {
                if (result.Data.TryGetValue("FreeRewardData", out var data))
                {
                   UserFreeRewardData = JsonConvert.DeserializeObject<Dictionary<string,FreeRewardData>>(data.Value);
                }
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });
            await tcs.Task;
        }
        private async UniTask ReceivePlayerData()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), (result) =>
            {
                // 양이 많아지면 객체로 빼야할듯 TODO 
                if(result.Data.TryGetValue("DailyReward",out var data))
                {
                    UserDailyReward = int.Parse(data.Value);
                }  
                if(result.Data.TryGetValue("IsUserTutorial",out var tutoData))
                {
                    IsUserTutorial = bool.Parse(tutoData.Value);
                }
                if(result.Data.TryGetValue("IsSelectMainCharacter",out var selectData))
                {
                    IsSelectMainCharacter = bool.Parse(selectData.Value);
                }
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });
           await tcs.Task;
        }
        private async UniTask ReceiveEnchantData()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), (result) =>
            {
                if (result.Data.TryGetValue("EnchantData", out var data))
                {
                    UserEnchantData = JsonConvert.DeserializeObject<Dictionary<EEnchantClassType, CharacterEnchantData>>(data.Value);
                }
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });
            await tcs.Task;
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
            PublishCharacterData(new Dictionary<string, string> { { "ChapterData", jsonData }, { "MissionData", JsonConvert.SerializeObject(UserMission)}, { "PlayMissionData", JsonConvert.SerializeObject(UserPlayMission)},
                { "MainCharacterData", JsonConvert.SerializeObject(UserMainCharacterData) },{"EnchantData",JsonConvert.SerializeObject(UserEnchantData)},
                {"DailyReward",UserDailyReward.ToString()},
                {"FreeRewardData",JsonConvert.SerializeObject(UserFreeRewardData)},
                {"IsSelectMainCharacter",IsSelectMainCharacter.ToString()},
                {"SurviveMission",JsonConvert.SerializeObject(UserServiceMission)},
                {"IsUserTutorial",IsUserTutorial.ToString()}});
        }

        public async UniTask GetFeatherTimer(Action<DateTime, bool> callback)
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (result) =>
            {
                DateTime nextFreeTicket = DateTime.MinValue;
                bool staminaCapped = true;

                if (result.VirtualCurrencyRechargeTimes.TryGetValue("FT", out VirtualCurrencyRechargeTime rechargeDetails))
                {
                    if (result.VirtualCurrency.TryGetValue("FT", out int staminaBalance))
                    {
                        UserCurrency[ECurrency.Father].Value = staminaBalance;
                        if (staminaBalance < rechargeDetails.RechargeMax)
                        {
                            nextFreeTicket = DateTime.Now.AddSeconds(rechargeDetails.SecondsToRecharge);
                            staminaCapped = false;
                        }
                        else
                        {
                            staminaCapped = true;
                        }
                    }
                }
                callback?.Invoke(nextFreeTicket, staminaCapped);
            }, (error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
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

                if (result.Data.TryGetValue("PlayMissionData", out var missionData))
                {
                    UserPlayMission = JsonConvert.DeserializeObject<Dictionary<string, PlayMission>>(missionData.Value);
                }
                
                if(result.Data.TryGetValue("SurviveMission",out var serviceMission))
                {
                    UserServiceMission = JsonConvert.DeserializeObject<SurviveMission>(serviceMission.Value);
                }
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });

            await tcs.Task;
        }

        public void UseInventoryItem(Dictionary<string,int> ItemsToConsume)
        {
            Dictionary<string, int> itemKey = new();
            foreach (var item in ItemsToConsume)
            {
                var findItem = GetInventoryItem(item.Key);
                if(findItem == null)
                    continue;
                itemKey.Add(findItem.ItemInstanceId,item.Value);
            }
            
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
               FunctionName = "UseInventoryItem",
               FunctionParameter = new {itemsToConsume=itemKey},
               GeneratePlayStreamEvent = true
            }, (result) =>
            {
                ReceiveInventory().Forget();
                Debug.Log("UseInventoryItem");
            }, (error) =>
            {
                ErrorLog(error);
            });
        }

        public void OpenContainerItem(string itemCode)
        {
            var itemKey = UserInventory.Find(x => x.ItemId == itemCode);
            if (itemKey != null)
            {
                PlayFabClientAPI.UnlockContainerInstance(
                    new UnlockContainerInstanceRequest
                    {
                        CatalogVersion = "shop", 
                        ContainerItemInstanceId = itemKey.ItemInstanceId
                    }, (result) =>
                    {
                        ReceiveCurrencyData().Forget();
                        ReceiveInventory().Forget();
                    }, (error) =>
                    {
                        ErrorLog(error);
                    });
            }
        }
        
        
        public void GrantItem(string itemId,Action callback = null)
        {
            var grantFreeItemRequest = new ExecuteCloudScriptRequest
            {
                FunctionName = "GrantFreeItem",
                FunctionParameter = new {itemId},
                GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(grantFreeItemRequest, (result) =>
            {
                ReceiveInventory(()=>callback?.Invoke()).Forget();
                Debug.Log("GrantItem");
            }, (error) =>
            {
                ErrorLog(error);
            });
        }
        private void PurchaseItem(string itemId,uint price,string vc,Action failedCallBack,string storeId)
        {
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
            {
                ItemId = itemId,
                Price = (int)price,
                VirtualCurrency = vc,
                StoreId = storeId
            }, (result) =>
            {
                ReceiveInventory().Forget();
             
            }, (error) =>
            {
                failedCallBack?.Invoke();
                ErrorLog(error);
            });
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
        private async UniTask ReceiveInventory(Action callBack = null)
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (result) =>
            {
                UserInventory = result.Inventory;
                callBack?.Invoke();
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });
            await tcs.Task;
        }

        private async UniTask FetchCatalogItems()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
            {
                CatalogVersion = "shop"
            }, (result) =>
            {
                CatalogItems = result.Catalog;
                tcs.TrySetResult();
            }, (error) =>
            {
                tcs.TrySetException(new Exception(error.GenerateErrorReport()));
                ErrorLog(error);
            });
            await tcs.Task;
        }
        private async UniTask ReceiveStoreItems(string storeId)
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest()
            {
                StoreId = storeId
            }, (result) =>
            {
                PublicStoreItems = result.Store;
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

        private async UniTask AutoSave()
        {
            await UniTask.WaitUntil(()=>mbIsLoadData);
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromMinutes(10));
                SaveCharacterData();
            }
      
        } 
        /// <summary>
        /// 유니티 종료 전 서버에 데이터 전송
        /// </summary>
        private void OnApplicationQuit()
        {
            if (mbIsLoadData)
            {
                SaveCharacterData();
            }
        }
#if !UNITY_EDITOR
        private void OnApplicationFocus(bool hasFocus)
        {
            if (mbIsLoadData && hasFocus == false)
            {
                SaveCharacterData();
            }
        }
#endif
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

        public void UpdateDailyReward()
        {
            AddCurrencyData(ECurrency.DailyReward,-1);
            UserDailyReward++;
        }
    }
}