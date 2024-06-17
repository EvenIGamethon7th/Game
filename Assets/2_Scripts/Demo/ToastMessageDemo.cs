using System;
using UnityEngine;

namespace _2_Scripts.Demo
{
    public class ToastMessageDemo : MonoBehaviour
    {
        private void Start()
        {
            UI_Toast_Manager.Instance.Activate_WithLcz_Func(TableDataKey_C.Localize_Lo_Test, () =>
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("끝남");
            });
            
        }
    }
}