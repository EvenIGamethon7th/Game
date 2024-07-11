using System.Collections.Generic;
using _2_Scripts.Game.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.ScriptableObject.Skill.DirectionSkill
{   [CreateAssetMenu(menuName = "ScriptableObject/Skill/DirectionAttack",fileName = "Direction_")]
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

        [SerializeField] 
        [Title("물리데미지 인지? False이면 마법데미지로")]
        private bool mbIsAttackDamage;
        public override bool CastAttack(Transform ownerTransform, CharacterData ownerData)
        {
            float range = mbFollowDefaultRange ? ownerData.range : this.Range;
            float totalDamage = ownerData.matk * (mPercentDamage * 0.01f);
            var detectingTargets = Physics2D.OverlapCircle(ownerTransform.position, range, TargetLayer);
            if (detectingTargets == null)
            {
                return false;
            }
            CastEffectPlay(ownerTransform.position);
            ownerTransform.GetComponent<CUnit>().SetFlipUnit(detectingTargets.transform);
            SpawnCollisionObject(detectingTargets.transform,totalDamage);
            return true;
        }

        // target을 기준으로 x , y를 늘리면서 타겟 감지.

        private void SpawnCollisionObject(Transform target,float damage)
        {
            HashSet<Vector3> spawnPos = new HashSet<Vector3>(); 
            var targetCell= MapManager.Instance.GetCellFromWorldPos(target.position);
            for (int i = -mDirectionPos.x; i <= mDirectionPos.x; i++)
            {
                var cellPos = new Vector3Int(targetCell.x + i, targetCell.y);
                var currentCellWorldPos = MapManager.Instance.GetWorldPosFromCell(cellPos);
                spawnPos.Add(currentCellWorldPos);
            }
            for (int i = -mDirectionPos.y; i <= mDirectionPos.y; i++)
            {
                var cellPos = new Vector3Int(targetCell.x, targetCell.y + i);
                var currentCellWorldPos = MapManager.Instance.GetWorldPosFromCell(cellPos);
                spawnPos.Add(currentCellWorldPos);
            }


            foreach (var pos in spawnPos)
            {
               var collisionSkill = ObjectPoolManager.Instance.CreatePoolingObject(mSpawnCollisionGo,pos).GetComponent<SkillCollision>();
               collisionSkill.Init(mLifeTime,damage,TargetLayer,HitEffect);
               
            }
            
        }
    }
}