using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cargold.Dialogue
{
    public class PropertyAdapter_UI_Dialogue_Button : Example.PropertyAdapter
    {
        public int id;
        public PropertyAdapter_UI_Dialogue_Script uiDialougeClass = null;
        public Image img = null;
        public TextMeshProUGUI tmp = null;

        public override string GetLibraryClassType => LibraryRemocon.UtilityClassData.DialogueData.Instance.GetUiBtnClassNameStr;
    }
}