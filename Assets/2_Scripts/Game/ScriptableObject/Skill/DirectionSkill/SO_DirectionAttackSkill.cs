using System;
using System.Collections.Generic;
using _2_Scripts.Game.Unit;
using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.DirectionSkill
{   
    using Monster;
    [CreateAssetMenu(menuName = "ScriptableObject/Skill/DirectionAttack",fileName = "Direction_")]
    public class SO_DirectionAttackSkill : DirectionSkill
    {
        [SerializeField] 
        [Title("지속 시간 ( 1초면 1)")]
        private float mLifeTime;
        
        [SerializeField]
        [Title("데미지 증감 퍼센트")]
        private float mPercentDamage;
        [SerializeField]
        [Title("기본 사거리를 따른다.")]
        private bool mbFollowDefaultRange = true;

       
        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData, Action<Monster[]> beforeDamage = null, Action <Monster> passive = null)
        {
            float range = mbFollowDefaultRange ? ownerData.range : this.Range;
            float totalDamage = ownerData.GetTotalDamageToType(AttackType) * (mPercentDamage * 0.01f);
            var detectingTargets = Physics2D.OverlapCircle(ownerTransform.position, range, TargetLayer);
            if (detectingTargets == null)
            {
                return false;
            }
            CastEffectPlay(ownerTransform.position);
            if (ownerTransform.TryGetComponent<CUnit>(out var attacker))
                attacker.SetFlipUnit(detectingTargets.transform);
            SpawnCollisionObject(detectingTargets.transform, totalDamage, attacker);
            
            return true;
        }

        // target을 기준으로 x , y를 늘리면서 타겟 감지.

        private void SpawnCollisionObject(Transform target,float damage,CUnit attacker = null)
        {
            HashSet<Vector3> spawnPos = new HashSet<Vector3>(); 
            HashSet<Monster> takeDamageMonsters = new HashSet<Monster>();
            var targetCell= MapManager.Instance.GetCellFromWorldPos(target.position);
            
            Vector2 movementVector = ((target.transform.position) - target.GetComponent<Monster>().NextWayPointVector).normalized;

            /// 상위 객체 하나로 묶어서 방향 회전으로 box colider x ,y 
            if (Math.Abs(movementVector.x) < Math.Abs(movementVector.y))
            {
                for (int i = -mDirectionPos.y; i <= mDirectionPos.y; i++)
                {
                    var cellPos = new Vector3Int(targetCell.x, targetCell.y + i);
                    var currentCellWorldPos = MapManager.Instance.GetWorldPosFromCell(cellPos);
                    MapManager.Instance.CheckTileSlotOnUnit(cellPos, colliders =>
                    {
                        foreach (var collider in colliders)
                        {
                            if (collider.CompareTag("Monster"))
                            {
                                takeDamageMonsters.Add(collider.GetComponent<Monster>());
                            }
                        }
                    });
                    spawnPos.Add(currentCellWorldPos);
                }
            }
            else
            {
                for (int i = -mDirectionPos.x; i <= mDirectionPos.x; i++)
                {
                    var cellPos = new Vector3Int(targetCell.x + i, targetCell.y);
                    var currentCellWorldPos = MapManager.Instance.GetWorldPosFromCell(cellPos);
                    MapManager.Instance.CheckTileSlotOnUnit(cellPos, colliders =>
                    {
                        foreach (var collider in colliders)
                        {
                            if (collider.CompareTag("Monster"))
                            {
                                takeDamageMonsters.Add(collider.GetComponent<Monster>());
                            }
                        }
                    });
                    spawnPos.Add(currentCellWorldPos);
                }
            }
            HitEffectPlay(target.position);

            if (mSpawnCollisionGo != null)
            {
                foreach (var pos in spawnPos)
                {
                    var collisionSkill = ObjectPoolManager.Instance.CreatePoolingObject(mSpawnCollisionGo,pos).GetComponent<SkillCollision>();
                    collisionSkill.Init(mLifeTime,this.StatueEffects,attacker);
                }
            }
            foreach (var monster in takeDamageMonsters)
            {
                monster.TakeDamage(damage,AttackType);
                if (mSpawnCollisionGo == null)
                {
                    var statusEffectHandler = monster.gameObject.GetComponent<StatusEffectHandler>();
                    StatueEffects?.ForEach(effect =>statusEffectHandler.AddStatusEffect(effect,attacker));
                }
            }
        }
    }
}