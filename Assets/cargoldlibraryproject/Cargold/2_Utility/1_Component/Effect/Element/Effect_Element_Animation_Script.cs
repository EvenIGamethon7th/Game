using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace Cargold.Effect
{
    public class Effect_Element_Animation_Script : Effect_Element_Script
    {
        [SerializeField] private Animation anim = null;
        [SerializeField] private SpriteRenderer[] spriteRendererArr = null;

        public override void Activate_Func()
        {
            base.Activate_Func();

            anim?.Play();
        }
        protected override void SetSortingOrderElem_Func(int _layer)
        {
            foreach (SpriteRenderer _spriteRenderer in spriteRendererArr)
                _spriteRenderer.sortingOrder = _layer;
        }
        public override void Deactivate_Func()
        {
            base.Deactivate_Func();

            anim?.Stop();
        }

        public void CallAni_Func(string _key)
        {

        }
        public void CallAni_Done_Func()
        {
            base.isDone = true;

            base.playDoneDel(this);
        }

        [Button("캐싱 ㄱㄱ ~")]
        public override void CallEditor_Catching_Func()
        {
            this.anim = this.GetComponent<Animation>();

            SpriteRenderer[] _spriteRendererArr = this.GetComponentsInChildren<SpriteRenderer>();
            this.spriteRendererArr = _spriteRendererArr;

            // 카라리의 이펙트에서 레이어 통제가 필요할 시...
            //foreach (SpriteRenderer _spriteRenderer in _spriteRendererArr)
            //    _spriteRenderer.sortingLayerID = DataBase_Manager.Field.SortingLayerID_Field;

            base.CallEditor_Catching_Func();
        }
    } 
}