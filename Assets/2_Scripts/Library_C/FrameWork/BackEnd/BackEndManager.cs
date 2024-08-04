﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using _2_Scripts.Game.BackEndData.Enchant;
using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.BackEndData.Mission;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.Utilities;
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
            {ECurrency.Diamond,new ReactiveProperty<int>(0)}
        };
        public Dictionary<string, SpawnMission> UserMission { get; private set; } = new Dictionary<string, SpawnMission>();
        public List<ChapterData> ChapterDataList { get; private set; } = new();

        public Dictionary<string, MainCharacterData> UserMainCharacterData { get; private set; } = new Dictionary<string, MainCharacterData>();

        public Dictionary<EEnchantClassType, CharacterEnchantData> UserEnchantData { get; private set; } = new();
        public List<CatalogItem> CatalogItems { get; private set; } = new List<CatalogItem>();
        public List<StoreItem> PublicStoreItems { get; private set; } = new List<StoreItem>();
        public List<ItemInstance> UserInventory { get; private set; }= new List<ItemInstance>();

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
            mbIsLoadData = true;
            successCallback?.Invoke();
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
            PublishCharacterData(new Dictionary<string, string> { { "ChapterData", jsonData }, { "MissionData", JsonConvert.SerializeObject(UserMission)}, 
                { "MainCharacterData", JsonConvert.SerializeObject(UserMainCharacterData) },{"EnchantData",JsonConvert.SerializeObject(UserEnchantData)}});
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
                itemKey.Add(GetInventoryItem(item.Key).ItemInstanceId,item.Value);
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
        private async UniTask ReceiveInventory()
        {
            var tcs = new UniTaskCompletionSource();
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (result) =>
            {
                UserInventory = result.Inventory;
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