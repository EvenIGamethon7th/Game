using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;


[System.Serializable]
public class UserData : Cargold.FrameWork.UserData_C
{
    public List<UserWealthData> userWealthDataList;

    public UserData()
    {
        this.userWealthDataList = new List<UserWealthData>();
    }
}

#region Wealth
[System.Serializable]
public class UserWealthData : Cargold.FrameWork.UserSystem_Manager.Wealth_C<WealthType, int>.IWealthData
{
    public WealthType wealthType;
    public int quantity;

    public UserWealthData(WealthType _wealthType, int _quantity)
    {
        this.wealthType = _wealthType;
        this.quantity = _quantity;
    }

    WealthType Cargold.FrameWork.UserSystem_Manager.Wealth_C<WealthType, int>.IWealthData.GetWealthType => this.wealthType;
    int Cargold.FrameWork.UserSystem_Manager.Wealth_C<WealthType, int>.IWealthData.GetQuantity => this.quantity;

    void Cargold.FrameWork.UserSystem_Manager.Wealth_C<WealthType, int>.IWealthData.AddQuantity_Func(int _quantity)
    {
        this.quantity += _quantity;
    }

    void Cargold.FrameWork.UserSystem_Manager.Wealth_C<WealthType, int>.IWealthData.SetQuantity_Func(int _quantity)
    {
        this.quantity = _quantity;
    }

    bool Cargold.FrameWork.UserSystem_Manager.Wealth_C<WealthType, int>.IWealthData.TryGetSubtract_Func(int _quantity, bool _isJustCheck)
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
