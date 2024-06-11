using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
 
namespace Cargold.FrameWork
{
    [System.Serializable]
    public class LczPreviewData
    {
        [SerializeField, OnValueChanged("CallEdit_KeyChanged_Func")] protected string lczKey = null;

        public string GetLczKey => this.lczKey;
        [ShowInInspector, MultiLineProperty(3), LabelText("미리보기")]
        public string GetLczStr
        {
            get
            {
#if UNITY_EDITOR
                return _CallEdit_Func();
#else
                return _GetLcz_Func();
#endif

#if UNITY_EDITOR
                string _CallEdit_Func()
                {
                    if (Application.isPlaying == false)
                    {
                        if (this.lczKey.IsNullOrWhiteSpace_Func() == true)
                            return "키 비어있음 ~";

                        if (DataBase_Manager.Instance is null == true)
                            return "Database Manager 없음 ~";

                        if (DataBase_Manager.Instance.GetLocalize_C is null == true)
                            return "Localize 데이터 그룹이 없음 ~";

                        if (DataBase_Manager.Instance.GetLocalize_C.TryGetData_Func(this.lczKey, out ILczData _lczData) == false)
                            return "키 없음 ~";

                        SystemLanguage _langType = this.langType == SystemLanguage.Unknown ? SystemLanguage.Korean : this.langType;
                        return _lczData.GetLczStr_Func(_langType);
                    }
                    else
                    {
                        return _GetLcz_Func();
                    }
                }
#endif

                string _GetLcz_Func()
                {
                    if(this.lczKey.IsNullOrWhiteSpace_Func() == false)
                    {
                        if (Cargold.FrameWork.LocalizeSystem_Manager.Instance is null == false)
                            return Cargold.FrameWork.LocalizeSystem_Manager.Instance.GetLcz_Func(this.lczKey);
                        else
                            return "Lcz Manager Not Found";
                    }
                    else
                    {
                        return "Lcz Key Empty";
                    }
                }
            }
        }
#if UNITY_EDITOR
        [SerializeField, LabelText("언어"), OnValueChanged("CallEdit_KeyChanged_Func")] private SystemLanguage langType = SystemLanguage.Korean;
        protected virtual void CallEdit_KeyChanged_Func() { }
#endif
    }

    [System.Serializable]
    public class LczPreviewDataWithTmp : LczPreviewData
    {
        [SerializeField] private TMPro.TextMeshProUGUI tmp = null;

        public void SetTmp_Func(TMPro.TextMeshProUGUI _tmp)
        {
            this.tmp = _tmp;
        }

        public void OnLcz_Func()
        {
            if (this.tmp is null == false)
                this.tmp.text = base.GetLczStr;
        }

#if UNITY_EDITOR
        protected override void CallEdit_KeyChanged_Func()
        {
            this.OnLcz_Func();
        }
#endif
    }
}