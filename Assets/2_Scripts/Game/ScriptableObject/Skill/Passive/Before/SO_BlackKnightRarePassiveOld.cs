using System;
using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.Passive
{
    // 일단 해당 코드 실행시 무한 반복문에 걸리는 듯 ㅇㅇ 확인 해봐야 함
    // [CreateAssetMenu(menuName = "ScriptableObject/PassiveSkill/execution")]
    // public class SO_BlackKnightRarePassiveOld : BeforePassive
    // {
    //     [Title("처형 체력 비율")] 
    //     [SerializeField] 
    //     private float mPercent;
    //     public override void BeforeDamage(Monster.Monster[] monsters)
    //     {
    //         foreach (var monster in monsters)
    //         {
    //             if (monster.GetMonsterData.hp <= monster.GetMonsterData.MaxHp * (mPercent* 0.01f))
    //             {
    //                 // TODO 공격력 1 상승 어떻게?? 
    //                 monster.TakeDamage(monster.GetMonsterData.MaxHp * (mPercent * 0.01f),Define.EAttackType.TrueDamage);
    //                 HitEffectPlay(monster.transform.position);
    //             }
    //         }
    //     }
    // }
}