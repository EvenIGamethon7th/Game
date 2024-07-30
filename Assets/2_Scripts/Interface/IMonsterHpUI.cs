using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterHpUI
{
    public void InitHpUI(float maxHp);
    public void SetHpUI(float currentHp);
    public bool Active { get; set; }

    public void UpdatePos(Vector3 pos);
}
