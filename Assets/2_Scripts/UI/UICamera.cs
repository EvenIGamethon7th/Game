using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : Singleton<UICamera>
{
    public Camera Camera { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        Camera = GetComponent<Camera>();
    }
}
