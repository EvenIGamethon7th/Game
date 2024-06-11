using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using UnityEngine.UI;

namespace Cargold.UI.Focus
{
    public class PropertyAdapter_UI_Focus_Script : Example.PropertyAdapter
    {
        public GameObject groupObj = null;
        public RectTransform focusRtrf = null;
        public Animation focusAnim = null;
        public Button btn = null;

        public override string GetLibraryClassType => LibraryRemocon.UtilityClassData.FocusSystemData.Instance.GetClassName;
    }
}