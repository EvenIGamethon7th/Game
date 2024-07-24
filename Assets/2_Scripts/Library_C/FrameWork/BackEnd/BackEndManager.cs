﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using EntityKey = PlayFab.DataModels.EntityKey;

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

        public List<ChapterData> ChapterDataList { get; private set; } = new()
        {
            {new ChapterData()
            {
                ChapterNumber = 1,
                Star = 0,
                StageList = new List<StageData>()
                {
                    new StageData()
                    {
                        StageNumber = 1,
                        Star = 0,
                        IsClear = false
                    },
                    new StageData()
                    {
                        StageNumber = 2,
                        Star = 0,
                        IsClear = false
                    },
                    new StageData()
                    {
                        StageNumber = 3,
                        Star = 0,
                        IsClear = false
                    }
                }
            }}
        };

        protected override void Awake()
        {
            base.Awake();
            mAuthService = new PlayFabAuthService();
        }

        protected override void ChangeSceneInit(Scene prev, Scene next)
        {
            
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
            SaveChapterData();
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

        public void SaveChapterData()
        {
            string jsonData = JsonConvert.SerializeObject(ChapterDataList);
            PublishChapterData(new Dictionary<string, string> { { "ChapterData", jsonData } });
        }
        
        private void PublishChapterData(Dictionary<string,string> data)
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
            
        }

    }
}