using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.WhichOne;

namespace Cargold.UI.Tab
{
    [System.Serializable]
    public class TabManager<TabType>
    {
        [ReadOnly, ShowInInspector] private TabWhichOne tabWhichOneClass;
        [ReadOnly, ShowInInspector] private Dictionary<TabType, TabContainer<TabType>> tabClassDic;
        [SerializeField] private TabContainer<TabType>[] tabContainerClassArr;
        [SerializeField] private TabType defaultType;

        public virtual bool IsAniWhenActivate => false;
        public virtual bool IsAniWhenDeactivate => false;

        public void Init_Func()
        {
            this.tabWhichOneClass = new TabWhichOne();

            this.tabClassDic = new Dictionary<TabType, TabContainer<TabType>>();

            foreach (TabContainer<TabType> _tabContainerClass in this.tabContainerClassArr)
            {
                TabType _tabType = _tabContainerClass.GetTabType;
                this.tabClassDic.Add_Func(_tabType, _tabContainerClass);

                _tabContainerClass.Init_Func(this);
            }

            this.Deactivate_Func();
        }

        public virtual void Activate_Func()
        {
            TabContainer<TabType> _tabContainerClass = this.tabClassDic.GetValue_Func(this.defaultType);

            foreach (var item in this.tabClassDic)
            {
                TabContainer<TabType> _value = item.Value;
                if(_tabContainerClass != _value)
                    _value.Deactivate_Func(false, this.IsAniWhenDeactivate == false);
            }

            this.tabWhichOneClass.Activate_Func(_tabContainerClass, this.IsAniWhenActivate == false);
        }

        public virtual void OnSelect_Func(UI_TabBtn_Script<TabType> _tabBtnClass, bool _isCancelWhenTwiceSelected = false)
        {
            TabType _tabType = _tabBtnClass.GetTabType;
            TabContainer<TabType> _tabContainerClass = this.tabClassDic.GetValue_Func(_tabType);
            this.tabWhichOneClass.Selected_Func(_tabContainerClass, _isCancelWhenTwiceSelected);
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            this.tabWhichOneClass.SelectCancel_Func();
        }

        public class TabWhichOne : WhichOne<TabContainer<TabType>>
        {
            public void Activate_Func(TabContainer<TabType> _tabContainer, bool _isImmediatly = true)
            {
                base.whichOne = _tabContainer;

                _tabContainer.Activate_Func(_isImmediatly);
            }
        }
    }
}