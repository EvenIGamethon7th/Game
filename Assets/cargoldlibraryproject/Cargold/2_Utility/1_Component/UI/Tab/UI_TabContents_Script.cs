using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.WhichOne;

namespace Cargold.UI.Tab
{
    public abstract class UI_TabContents_Script<TabType> : UI_Script
    {
        [ReadOnly, ShowInInspector] private TabManager<TabType> tabContainer;

        public abstract TabType GetTabType { get; }
    }
}