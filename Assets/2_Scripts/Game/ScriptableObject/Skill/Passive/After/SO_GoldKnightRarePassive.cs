using _2_Scripts.Game.Monster;
using _2_Scripts.Game.ScriptableObject.Skill.Passive;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/Gold")]
public class SO_GoldKnightRarePassive : AfterPassive
{
    [Title("��� ȹ�� Ȯ��(0 ~ 100)")]
    [SerializeField]
    private float mPercentage;

    [Title("��� ȹ�淮")]
    [SerializeField]
    private int mGold;

    public override void AfterDamage(Monster monsters)
    {
        if (Random.Range(0, 100f) < mPercentage)
        {
            GameManager.Instance.UpdateMoney(EMoneyType.Gold,mGold);
            Debug.Log($"GetGold {mGold}");
        }
    }

}
