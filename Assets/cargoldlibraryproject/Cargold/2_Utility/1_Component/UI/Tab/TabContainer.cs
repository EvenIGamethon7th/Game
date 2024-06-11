using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.WhichOne;

namespace Cargold.UI.Tab
{
    [System.Serializable]
    public class TabContainer<TabType> : IWhichOne
    {
        [SerializeField] private UI_TabContents_Script<TabType> tabClass;
        [SerializeField] private UI_TabBtn_Script<TabType> tabBtnClass;

        [ShowInInspector] public TabType GetTabType => this.tabClass != null ? this.tabClass.GetTabType : default;
        public UI_TabContents_Script<TabType> GetTabClass => this.tabClass;
        public UI_TabBtn_Script<TabType> GetTabBtnClass => this.tabBtnClass;

        public void Init_Func(TabManager<TabType> _tabManager)
        {
            for (int i = 0; i < 3; i++)
                this.tabClass.Init_Func(i);

            this.tabBtnClass.Init_Func(_tabManager, this.tabClass.GetTabType);
        }

        public void Activate_Func(bool _isImmediatly = true)
        {
            this.tabClass.Activate_Func();
            this.tabBtnClass.Activate_Func(_isImmediatly);
        }

        public virtual void Selected_Func(bool _repeat = false)
        {
            if (_repeat == false)
                this.Activate_Func(false);
        }

        public virtual void SelectCancel_Func()
        {
            this.Deactivate_Func(false, false);
        }

        public void Deactivate_Func(bool _isInit = false, bool _isImmediately = true)
        {
            if (_isInit == false)
            {
                this.tabClass?.Deactivate_Func();
                this.tabBtnClass?.Deactivate_Func(false, _isImmediately);
            }
        }
    }
}