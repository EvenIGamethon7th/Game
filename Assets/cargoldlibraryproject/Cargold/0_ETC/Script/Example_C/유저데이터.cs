using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Example {
[System.Serializable]
public class 유저데이터 : Cargold.FrameWork.UserData_C
{
    public List<유저재화데이터> userWealthDataList;

    public 유저데이터()
    {
        this.userWealthDataList = new List<유저재화데이터>();
    }
}

#region Wealth
[System.Serializable]
public class 유저재화데이터 : Cargold.FrameWork.UserSystem_Manager.Wealth_C<재화타입, 재화밸류>.IWealthData
{
    public 재화타입 wealthType;
    public 재화밸류 quantity;

    public 유저재화데이터(재화타입 _wealthType, 재화밸류 _quantity)
    {
        this.wealthType = _wealthType;
        this.quantity = _quantity;
    }

    재화타입 Cargold.FrameWork.UserSystem_Manager.Wealth_C<재화타입, 재화밸류>.IWealthData.GetWealthType => this.wealthType;
    재화밸류 Cargold.FrameWork.UserSystem_Manager.Wealth_C<재화타입, 재화밸류>.IWealthData.GetQuantity => this.quantity;

    void Cargold.FrameWork.UserSystem_Manager.Wealth_C<재화타입, 재화밸류>.IWealthData.AddQuantity_Func(재화밸류 _quantity)
    {
        this.quantity += _quantity;
    }

    void Cargold.FrameWork.UserSystem_Manager.Wealth_C<재화타입, 재화밸류>.IWealthData.SetQuantity_Func(재화밸류 _quantity)
    {
        this.quantity = _quantity;
    }

    bool Cargold.FrameWork.UserSystem_Manager.Wealth_C<재화타입, 재화밸류>.IWealthData.TryGetSubtract_Func(재화밸류 _quantity, bool _isJustCheck)
    {
        if (_quantity <= this.quantity)
        {
            if (_isJustCheck == false)
                this.quantity -= _quantity;

            return true;
        }
        else
        {
            return false;
        }
    }
}
#endregion
} // End