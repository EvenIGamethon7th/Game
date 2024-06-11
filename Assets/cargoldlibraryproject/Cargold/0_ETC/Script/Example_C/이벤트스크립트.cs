using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
namespace Cargold.Example {
public class 이벤트스크립트 : Cargold.EventSystem.Event_Script
{
    public override string GetEventKey => this.gameObject.name;
}
} // End