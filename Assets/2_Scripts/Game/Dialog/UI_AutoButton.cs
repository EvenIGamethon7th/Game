using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_AutoButton : Button
{
    private ReactiveProperty<bool> mIsAuto = new ReactiveProperty<bool>(false);

    protected override void Awake()
    {
        base.Awake();
        onClick.AddListener(IsAuto);
    }

    private void IsAuto()
    {
        mIsAuto.Value = !mIsAuto.Value;
    }
}
