using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.SDK.Purchase
{
#if Purchase_Unity_C
    using Unity.Services.Core;
    using UnityEngine.Analytics;
    using UnityEngine.Purchasing;
    using UnityEngine.Purchasing.Security;

    public class InAppSystem_UnityPurchase : InAppSystem_Manager, IStoreListener
    {
        private IStoreController controller;
        private IExtensionProvider extensions;

        [SerializeField, LabelText("테스트 스토어 UI 모드")] private FakeStoreUIMode fakeStoreUIMode = FakeStoreUIMode.StandardUser;

        public override void Init_Func(int _layer)
        {
            base.Init_Func(_layer);

            if(_layer == 0)
            {
                
            }
            else if(_layer == 1)
            {
                StandardPurchasingModule _standardPurchasingModule = StandardPurchasingModule.Instance();

#if UNITY_EDITOR
                _CallEditor_Func();
#else
                _CallOfficial_Func();
#endif

                void _CallEditor_Func()
                {
                    _standardPurchasingModule.useFakeStoreAlways = true;
                    _standardPurchasingModule.useFakeStoreUIMode = this.fakeStoreUIMode;
                }
                void _CallOfficial_Func()
                {
                    _standardPurchasingModule.useFakeStoreAlways = false;
                }

                ConfigurationBuilder _builder = ConfigurationBuilder.Instance(_standardPurchasingModule);

                IInappData[] _iInappDataArr = Cargold.FrameWork.DataBase_Manager.Instance.GetInapp_C.GetDataArr_Func();
                foreach (IInappData _iInappData in _iInappDataArr)
                {
                    _builder.AddProduct(_iInappData.GetKey, _iInappData.GetProductType, new IDs
                    {
                        {_iInappData.GetGoogleID, GooglePlay.Name},
                        {_iInappData.GetAppleID, MacAppStore.Name}
                    });
                }

                UnityPurchasing.Initialize(this, _builder);
            }
        }

        protected virtual void OnRestore_Func(string _purchasedID)
        {

        }
        public override void OnRestore_Func(System.Action<bool> _callbackDel = null)
        {
#if UNITY_ANDROID
            _OnRestoreAOS_Func();
#elif UNITY_IOS
            _OnRestoreIOS_Func();
#endif
            void _OnRestoreAOS_Func()
            {
                this.extensions.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions(_is =>
                {
                    _Result_Func(_is);
                });
            }
            void _OnRestoreIOS_Func()
            {
                this.extensions.GetExtension<IAppleExtensions>().RestoreTransactions(_is =>
                {
                    _Result_Func(_is);
                });
            }

            void _Result_Func(bool _isResult)
            {
                if (_isResult)
                {
                    // This does not mean anything was restored,
                    // merely that the restoration process succeeded.
                }
                else
                {
                    // Restoration failed.
                }

                _callbackDel?.Invoke(_isResult);

                base.OnRestoreDone_Func(_isResult);
            }
        }
        protected override void OnPurchaseProcess_Official_Func(IInappData _iInappData)
        {
            this.controller.InitiatePurchase(_iInappData.GetKey);
        }
        
        protected override string GetLczPriceStr_Func(string _dataKey)
        {
            Product _product = this.controller.products.WithID(_dataKey);
            return _product.metadata.localizedPriceString;
        }

        void IStoreListener.OnInitialized(IStoreController _controller, IExtensionProvider _extensions)
        {
            this.controller = _controller;
            this.extensions = _extensions;

            this.OnRestore_Func();

            base.OnInitializeDone_Func(true);
        }
        public void OnInitializeFailed(InitializationFailureReason error) // Obsolete
        {
            base.OnInitializeDone_Func(false, error.ToString());
        }
        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            string _str = StringBuilder_C.Append_Func(error.ToString(), " / ", message);
            base.OnInitializeDone_Func(false, _str);
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs _purchaseEvent)
        {
            string _msg = _purchaseEvent.purchasedProduct.GetInfo_Func();
            base.OnPurchaseDone_Func(base.inappKey, PurchaseResult.Succcess, _msg);

            bool _isRestore = false;
#if !UNITY_EDITOR
#if UNITY_ANDROID
            _isRestore = _RestoreAOS_Func();
#endif
#endif

            //if(_isRestore == true)
            //    this.OnRestore_Func(_purchaseEvent.purchasedProduct.definition.id);
            
            return PurchaseProcessingResult.Complete;

            bool _RestoreAOS_Func()
            {
                // 구매 복구라 생각하고 쓴 코드인데, 정확히 뭔지 한 번 더 확인 ㄱㄱ

                //CrossPlatformValidator _validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
                //try
                //{
                //    IPurchaseReceipt[] _resultArr = _validator.Validate(_purchaseEvent.purchasedProduct.receipt);
                //    foreach (IPurchaseReceipt _result in _resultArr)
                //    {
                //        Analytics.Transaction(
                //            _result.productID
                //            , _purchaseEvent.purchasedProduct.metadata.localizedPrice
                //            , _purchaseEvent.purchasedProduct.metadata.isoCurrencyCode
                //            , _result.transactionID, null);
                //    }

                //    return true;
                //}
                //catch (IAPSecurityException)
                //{
                return false;
                //}
            }
        }

        void IStoreListener.OnPurchaseFailed(Product _product, PurchaseFailureReason _failureReason)
        {
            PurchaseResult _purchaseResult = default;
            switch (_failureReason)
            {
                case PurchaseFailureReason.PurchasingUnavailable:
                    _purchaseResult = PurchaseResult.Fail_PurchasingUnavailable;
                    break;

                case PurchaseFailureReason.ExistingPurchasePending:
                    _purchaseResult = PurchaseResult.Fail_ExistingPurchasePending;
                    break;

                case PurchaseFailureReason.ProductUnavailable:
                    _purchaseResult = PurchaseResult.Fail_ProductUnavailable;
                    break;

                case PurchaseFailureReason.SignatureInvalid:
                    _purchaseResult = PurchaseResult.Fail_SignatureInvalid;
                    break;

                case PurchaseFailureReason.UserCancelled:
                    _purchaseResult = PurchaseResult.Fail_UserCancelled;
                    break;

                case PurchaseFailureReason.PaymentDeclined:
                    _purchaseResult = PurchaseResult.Fail_PaymentDeclined;
                    break;

                case PurchaseFailureReason.DuplicateTransaction:
                    _purchaseResult = PurchaseResult.Fail_DuplicateTransaction;
                    break;

                case PurchaseFailureReason.Unknown:
                default:
                    _purchaseResult = PurchaseResult.Fail_Unknown;
                    break;
            }

            string _msg = StringBuilder_C.Append_Func("Reason  : ", _failureReason.ToString_Func(), " / ", _product.GetInfo_Func());

            base.OnPurchaseDone_Func(base.inappKey, _purchaseResult, _msg);
        }
    }

    public static class Extension_C
    {
        public static string GetInfo_Func(this Product _product)
        {
            ProductMetadata _metadata = _product.metadata;
            return StringBuilder_C.Append_Func("(Product) ID : ", _product.definition.id
                , " / Title : ", _metadata.localizedTitle
                , " / Price : ", _metadata.localizedPriceString);
        }
    }
#endif
}