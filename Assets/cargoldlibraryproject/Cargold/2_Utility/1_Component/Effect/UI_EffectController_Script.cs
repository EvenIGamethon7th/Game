using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;

namespace Cargold.Effect
{
    public class UI_EffectController_Script : SerializedMonoBehaviour
    {
        [LabelText("이펙트 컨트롤 데이터"), SerializeField] private Dictionary<string, EffectControlData> effectControlDataDic = new Dictionary<string, EffectControlData>();

        public void CallAni_EffectController_Func(string _key)
        {
            if (_key.IsNullOrWhiteSpace_Func() == true)
                return;

            if (PoolingSystem_Manager.Instance.IsGeneratedPool(_key) == true)
            {
                if (this.effectControlDataDic.TryGetValue(_key, out EffectControlData _effectControlData) == true)
                {
                    _effectControlData.Activate_Func(_key, this.transform);
                }
                else
                {
                    Debug_C.Error_Func(_key + " 이펙트 키가 " + this.transform.GetPath_Func() + "의 컨트롤 데이터에 없습니다.");
                }
            }
            else
            {
                Debug_C.Error_Func(_key + " 이펙트 키가 풀링에 없습니다.");
            }
        }

        [System.Serializable]
        public class EffectControlData
        {
            [SerializeField, LabelText("어디에 띄움?")] private Transform pivotTrf = null;
            [SerializeField, LabelText("UI 데이터")] private UiData uiData = null;

            public Transform GetPivotTrf => this.pivotTrf;

            public void Activate_Func(string _key, Transform _thisTrf)
            {
                if(this.uiData != null)
                {
                    if (this.uiData.IsActivate == false)
                        return;
                }

                Transform _parentTrf = null;

                if (this.pivotTrf == null)
                {
                    Debug_C.Warning_Func(_thisTrf.GetPath_Func() + " - " + _key + " - pivotTrf가 비어있습니다.");

                    _parentTrf = _thisTrf;
                }
                else
                {
                    _parentTrf = this.pivotTrf;
                }

                Effect_Script _effectClass = PoolingSystem_Manager.Instance.Spawn_Func<Cargold.Effect.Effect_Script>(_key, _scale: 0.01f);
                _effectClass.transform.SetParent(_parentTrf);
                _effectClass.transform.localScale = Vector3.one * _effectClass.transform.localScale.x;
                _effectClass.Activate_Func(Vector2.zero);
            }

            [System.Serializable]
            public class UiData
            {
                [SerializeField, LabelText("UI 클래스")] private Cargold.UI.UI_Script uiClass = null;
                [SerializeField, LabelText("이펙트 생성 시점")] private BeginType beginType = BeginType.Activate;

                public bool IsActivate
                {
                    get
                    {
                        if (this.uiClass == null)
                            return true;

                        switch (this.beginType)
                        {
                            case BeginType.Activate:
                                return this.uiClass.IsActivate == false;
                                
                            case BeginType.Deactivate:
                                return this.uiClass.IsActivate == true;

                            default:
                                return true;
                        }
                    }
                }

                public enum BeginType
                {
                    None = 0,

                    Activate = 10,
                    Deactivate = 20,
                    Always = 30,
                }
            }
        }
    } 
}