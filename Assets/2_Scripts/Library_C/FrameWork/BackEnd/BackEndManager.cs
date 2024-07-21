using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using UnityEngine;

namespace Cargold.FrameWork.BackEnd
{
    public class BackEndManager : Singleton<BackEndManager>
    {
        private PlayFabAuthService mAuthService;

        protected override void Awake()
        {
            base.Awake();
            mAuthService = new PlayFabAuthService();
        }
        
        public void OnLogin(Action callback)
        {
            LoginAsync(callback).Forget();
        }


        private async UniTaskVoid LoginAsync(Action successCallback = null)
        {
            mAuthService.Authenticate(Authtypes.Silent);
            await UniTask.WaitUntil(() => mAuthService.SessionTicket != null);
            //맨 마지막에 
            SyncCurrencyDataFromServer(() =>
            {
                successCallback.Invoke();
            });
     
        }
        
        private void InitData()
        {
           // 처음 유저 정보 불러오기
        }
        
        
        /// <summary>
        /// 서버에서 데이터를 동기화 해야할 때.
        /// </summary>
        public void SyncCurrencyDataFromServer(Action callback = null)
        {
            GetCurrencyDataBackEnd(callback).Forget();
        }
        private async UniTaskVoid GetCurrencyDataBackEnd(Action callback = null)
        {
            bool isResult = false;
            PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(),
                (result) =>
                {
                    //TODO 
                    // List<string> Keys = new List<string>(_userCurrecy.Keys);
                    // foreach (var item in Keys)
                    // {
                    //     _userCurrecy[item] = result.VirtualCurrency[item];
                    // }
                
                    isResult = true;
                },
                (error) => { ErrorLog(error); });
            await UniTask.WaitUntil(() => { return isResult == true; });
            callback?.Invoke();
        }
        
        private void ErrorLog(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }
    }
}