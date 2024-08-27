
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold.PoolingSystem;
using TMPro;
using System;


namespace Cargold.UI
{
    public class UI_BaseToast_Script : MonoBehaviour, IPooler
    {
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr)] [SerializeField] private GameObject groupObj = null;
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr)] [SerializeField] private Animation anim = null;
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr)] [SerializeField] private TextMeshProUGUI contentTmp = null;

        private Action endCallbackDel;

        public void Init_Func()
        {
            this.Deactivate_Func(true, false);
        }

        public void Activate_WithLcz_Func(string _lczKey, Action _endCallbackDel = null, bool isIgnoreTimeScale = false)
        {
            string _str = FrameWork.LocalizeSystem_Manager.Instance.GetLcz_Func(_lczKey);
            this.Activate_Func(_str, _endCallbackDel, isIgnoreTimeScale);
        }
        public void Activate_WithContent_Func(string _contentStr, Action _endCallbackDel = null, bool isIgnoreTimeScale = false, float time = 1f)
        {
            this.Activate_Func(_contentStr, _endCallbackDel, isIgnoreTimeScale, time);
        }
        protected virtual void Activate_Func(string _contentStr, Action _endCallbackDel = null, bool isIgnoreTimeScale = false, float time = 1f)
        {
            this.groupObj.SetActive(true);

            this.contentTmp.text = _contentStr;

            this.transform.SetAsLastSibling();

            this.anim.Play_Func(isIgnoreTimeScale: isIgnoreTimeScale, _speed: time);

            this.endCallbackDel = _endCallbackDel;
        }

        public void Deactivate_Func(bool _isInit = false, bool _isCallManager = false)
        {
            if (_isInit == false)
            {
                if (_isCallManager == false)
                    UI_Toast_Manager.Instance.RemoveElem_Func(this);

                Cargold.FrameWork.PoolingSystem_Manager.Instance.Despawn_Func(PoolingKey.ToastElem, this);
            }

            this.endCallbackDel = null;

            this.groupObj.SetActive(false);
        }

        public void CallAni_Close_Func()
        {
            if (this.endCallbackDel != null)
                this.endCallbackDel();

            this.Deactivate_Func();
        }

#if UNITY_EDITOR
        public void CallEdit_SetAni_Func(AnimationClip _clip)
        {
            this.anim.clip = _clip;
        }
#endif

        void IPooler.InitializedByPoolingSystem()
        {
            this.Init_Func();
        }
    }
}