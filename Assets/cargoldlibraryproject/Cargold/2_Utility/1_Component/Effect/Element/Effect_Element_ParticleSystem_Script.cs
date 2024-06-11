using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Cargold;

namespace Cargold.Effect
{
    public class Effect_Element_ParticleSystem_Script : Effect_Element_Script
    {
        [BoxGroup("캐싱 데이터")] [SerializeField] private ParticleSystem ps = null;
        [UnityEngine.Serialization.FormerlySerializedAs("renderer")]
        [BoxGroup("캐싱 데이터")] [SerializeField] private ParticleSystemRenderer psRenderer = null;

        public ParticleSystem GetPS => this.ps;

        public override void Init_Func(Action<Effect_Element_Script> _playDoneDel)
        {
            base.Init_Func(_playDoneDel);

            if (this.ps == null || this.psRenderer == null)
            {
                CallEditor_Catching_Func();
            }
        }

        public override void Activate_Func()
        {
            base.Activate_Func();

            this.ps.Play();
        }
        protected override void SetSortingOrderElem_Func(int _layer)
        {
            this.psRenderer.sortingOrder = _layer;
        }
        public override void Deactivate_Func()
        {
            base.Deactivate_Func();

            this.ps.Stop();
        }

        public void OnParticleSystemStopped()
        {
            base.isDone = true;

#if UNITY_EDITOR
            if (base.playDoneDel == null)
            {
                Debug_C.Error_Func("다음 이펙트 엘렘의 파티클 완료 콜백이 없습니다. : " + this.transform.GetPath_Func());
                return;
            }
#endif

            base.playDoneDel(this);
        }

        [Button("캐싱 ㄱㄱ~")]
        public override void CallEditor_Catching_Func()
        {
            this.ps = this.gameObject.GetComponent<ParticleSystem>();

            if (this.ps != null)
            {
                ParticleSystem.MainModule _mainModule = this.ps.main;
                _mainModule.stopAction = ParticleSystemStopAction.Callback;

                this.psRenderer = this.ps.GetComponent<ParticleSystemRenderer>();
                // 카라리의 이펙트에서 레이어 통제가 필요할 시...
                //this.psRenderer.sortingLayerID = DataBase_Manager.Field.SortingLayerID_Field;

                base.CallEditor_Catching_Func();
            }
            else
            {
                Debug_C.Error_Func("PS 없는데?");
            }
        }
    } 
}