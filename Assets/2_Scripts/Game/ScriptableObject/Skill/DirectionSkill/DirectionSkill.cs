using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.DirectionSkill
{
    using _2_Scripts.Game.Monster;
    using System.Collections.Generic;

    /// <summary>
    /// 상하좌우로 영향이 가는 스킬 목록
    /// </summary>
    public abstract class DirectionSkill : Skill
    {
        [SerializeField] 
        [Title("상하좌우 몇 칸까지")]
        protected Vector2Int mDirectionPos;

        [SerializeField] 
        [Title("상하좌우에 소환할 충돌 감지 오브젝트")]
        protected GameObject mSpawnCollisionGo;
        public abstract override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster[]> beforeDamage = null, Action < Monster> passive = null);
        
    }
}