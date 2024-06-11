using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Cargold.Effect
{
    public abstract class Effect_Element_Script : MonoBehaviour
    {
        [SerializeField] protected int sortingLayerOffset = 1;
        [ShowInInspector] protected System.Action<Effect_Element_Script> playDoneDel;
        protected bool isDone;
        public bool IsDone { get { return this.isDone; } }

        public virtual void Init_Func(System.Action<Effect_Element_Script> _playDoneDel)
        {
            this.playDoneDel = _playDoneDel;
        }
        public virtual void Activate_Func()
        {
            this.gameObject.SetActive(true);
        }
        public void SetSortingOrder_Func(int _layer)
        {
            this.SetSortingOrderElem_Func(_layer + sortingLayerOffset);
        }
        protected abstract void SetSortingOrderElem_Func(int _layer);

        public virtual void Deactivate_Func()
        {
            this.gameObject.SetActive(false);

            this.isDone = false;
        }

        public void Reset()
        {
            this.CallEditor_Catching_Func();
        }
        public virtual void CallEditor_Catching_Func() { }
    } 
}