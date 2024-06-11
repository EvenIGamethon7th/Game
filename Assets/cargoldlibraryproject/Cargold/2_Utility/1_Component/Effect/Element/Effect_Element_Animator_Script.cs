using Cargold;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.Effect
{
    public class Effect_Element_Animator_Script : Effect_Element_Script
    {
        [SerializeField] private Animator anim = null;
        [SerializeField] private SpriteRenderer[] spriteRendererArr = null;

        public override void Init_Func(Action<Effect_Element_Script> _playDoneDel)
        {
            base.Init_Func(_playDoneDel);

            anim.enabled = true;
        }

        public override void Activate_Func()
        {
            base.Activate_Func();

            anim.enabled = true;
            anim.Play(0);
        }
        protected override void SetSortingOrderElem_Func(int _layer)
        {
            foreach (SpriteRenderer _spriteRenderer in spriteRendererArr)
                _spriteRenderer.sortingOrder = _layer;
        }
        public override void Deactivate_Func()
        {
            base.Deactivate_Func();

            anim.enabled = false;
        }

        public void CallAni_Func(string _key)
        {

        }
        public void CallAni_Done_Func()
        {
            base.isDone = true;

            base.playDoneDel(this);
        }
        [Button("캐싱 ㄱㄱ~")]
        public override void CallEditor_Catching_Func()
        {
            this.anim = this.gameObject.GetComponent<Animator>();
            this.spriteRendererArr = this.gameObject.GetComponentsInChildren<SpriteRenderer>();

            if (this.anim == null)
                Debug_C.Error_Func("Animator 없는디?");

            if (this.spriteRendererArr != null)
            {
                // 카라리의 이펙트에서 레이어 통제가 필요할 시...
                //foreach (SpriteRenderer _spriteRenderer in this.spriteRendererArr)
                //    _spriteRenderer.sortingLayerID = DataBase_Manager.Field.SortingLayerID_Field;

                base.CallEditor_Catching_Func();
            }
            else
                Debug_C.Error_Func("SpriteRenderer 없는디?");
        }
    } 
}
