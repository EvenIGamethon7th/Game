using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;

namespace Cargold
{
    public class SfxController_Script : SerializedMonoBehaviour
    {
        [SerializeField, LabelText("효과음"), DictionaryDrawerSettings(KeyLabel = "애니 이벤트", ValueLabel = "SfxData")]
        private Dictionary<string, SfxData> sfxDataDic = new Dictionary<string, SfxData>();

        [SerializeField, FoldoutGroup("폐기 예정"), LabelText("효과음"), DictionaryDrawerSettings(KeyLabel = "애니 이벤트", ValueLabel = "SfxType")]
        private Dictionary<string, SfxType> sfxDic = new Dictionary<string, SfxType>();

        public void CallAni_Sfx_Func(string _sfxKey)
        {
            if (this.sfxDataDic.TryGetValue(_sfxKey, out SfxData _sfxData) == true)
            {
                _sfxData.PlaySfx_Func();
            }
            else
            {
                if (this.sfxDic.TryGetValue(_sfxKey, out SfxType _sfxType) == true)
                {
                    Debug_C.Warning_Func("다음 효과음 컨트롤러에 폐기 예정인 데이터가 있습니다. : " + this.transform.GetPath_Func());

                    Cargold.FrameWork.SoundSystem_Manager.Instance.PlaySfx_Func(_sfxType);
                }
#if UNITY_EDITOR
                else
                {
                    Debug_C.Log_Func(_sfxKey + "라는 효과음은 없슴다. 경로 : " + this.transform.GetPath_Func());
                }
#endif
            }


        }

        [System.Serializable]
        public class SfxData
        {
            [SerializeField] private SfxType sfxType;
            [SerializeField, LabelText("카라리UI")] private Cargold.UI.UI_Script uiClass;
            [SerializeField, LabelText("효과음 출력 시 UI의 활성화 상태"), EnableIf("@this.uiClass != null")] private bool isDeactivate;

            public void PlaySfx_Func()
            {
                if(this.uiClass != null)
                {
                    if (this.uiClass.IsActivate == this.isDeactivate)
                        return;
                }

                Cargold.FrameWork.SoundSystem_Manager.Instance.PlaySfx_Func(this.sfxType);
            }
        }
    }
}