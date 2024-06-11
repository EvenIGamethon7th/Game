using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.UI.Tab
{
    public abstract class UI_TabBtn_Script<TabType> : MonoBehaviour
    {
#if DoTween_C
        [SerializeField, LabelText("카라리 버튼")] private UI_BaseButton_Script baseBtnClass = null; 
#endif
        [ReadOnly, ShowInInspector] protected TabManager<TabType> tabContainer;
        [ReadOnly, ShowInInspector] protected TabType tabType;

        public TabType GetTabType => this.tabType;

        public virtual void Init_Func(TabManager<TabType> _tabContainer, TabType _tabType)
        {
            this.tabContainer = _tabContainer;
            this.tabType = _tabType;

            this.Deactivate_Func(true);
        }

        public virtual void Activate_Func(bool _isImmediatly = true)
        {
#if DoTween_C
            if (this.baseBtnClass != null)
                this.baseBtnClass.OnBtnState_Func(true, _isImmediatly); 
#endif
        }

        protected virtual void OnBtn_Func()
        {
            this.tabContainer.OnSelect_Func(this);
        }

        public virtual void Deactivate_Func(bool _isInit = false, bool _isImmediately = true)
        {
            if(_isInit == false)
            {
#if DoTween_C
                if (this.baseBtnClass != null)
                    this.baseBtnClass.OnBtnState_Func(false, _isImmediately); 
#endif
            }
            else
            {
#if DoTween_C
                if (this.baseBtnClass != null)
                    this.baseBtnClass.OnBtnState_Func(false, true);
#endif
            }
        }

        public void CallBtn_Func()
        {
            this.OnBtn_Func();
        }
    } 
}