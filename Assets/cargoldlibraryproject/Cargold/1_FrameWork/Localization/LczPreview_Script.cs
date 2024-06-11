using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using TMPro;

namespace Cargold.FrameWork
{
    public class LczPreview_Script : MonoBehaviour
    {
        [SerializeField, InlineProperty, HideLabel] private LczPreviewDataWithTmp tmpLczData = new LczPreviewDataWithTmp();

        public string GetLczKey => this.tmpLczData.GetLczKey;

        private void OnEnable()
        {
#if UNITY_EDITOR
            if(this.GetLczKey.IsNullOrWhiteSpace_Func() == false)
            {
                if (DataBase_Manager.Instance.GetLocalize_C.TryGetData_Func(this.GetLczKey, out ILczData _lczData) == false)
                {
                    Debug_C.Error_Func($"다음 LczKey에 해당하는 텍스트를 찾을 수 없습니다. { this.tmpLczData.GetLczKey} / 경로 : {this.transform.GetPath_Func()}");

                    return;
                }
            }
            else
            {
                Debug_C.Warning_Func($"다음 경로의 LczPreview에 Key가 비어있습니다. {this.transform.GetPath_Func()}");
            }
#endif

            UserSystem_Manager.Instance.GetCommon.Subscribe_LangTypeChanged_Func(this.CallDel_LangTypeChanged_Func);
        }

        private void OnDisable()
        {
            UserSystem_Manager.Instance.GetCommon.Unsubscribe_LangTypeChanged_Func(this.CallDel_LangTypeChanged_Func);
        }

        private void Reset()
        {
            if(this.TryGetComponent(out TextMeshProUGUI _tmp) == true)
            {
                this.tmpLczData.SetTmp_Func(_tmp);
            }

            this.tmpLczData.OnLcz_Func();
        }

        private void CallDel_LangTypeChanged_Func(SystemLanguage _langType)
        {
            this.tmpLczData.OnLcz_Func();
        }
    }
}