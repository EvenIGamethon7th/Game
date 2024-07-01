using System;
using Rito.Attributes;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Game.ScriptableObject.Skill;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

namespace _2_Scripts.Game.Unit
{
    public enum EUnitClass
    {
        None,
        Indian_Archer,
        Fox_Archer,
        Oni_Archer,
        Angel_Archer,
        Devil_Archer,
        Steam_Punk_Archer,
        Royal_Archer,
        Poison_Archer,
        Green_Archer,
        Holy_Archer
    }

    public enum EUnitRank
    {
        None,
        Normal,
        Rare,
        Unique
    }

    public enum EUnitStates
    {
        Idle,
        Move,
        Attack
    }

    public class CUnit : MonoBehaviour
    {
        public EUnitClass CurrentUnitClass { get; private set; } = EUnitClass.None;
        public EUnitRank CurrentUnitRank { get; private set; } = EUnitRank.None;
        public CharacterInfo CharacterDataInfo { get; private set; }
        public CharacterData CharacterDatas { get; private set; }
        private MeshRenderer mMeshRenderer;
        private SkeletonAnimation mAnimation;

        
        public Skill DefaultAttack { get; private set; }
        public Queue<Skill> ReadySkillQueue { get; private set; } = new Queue<Skill>();
        private void Awake()
        {
            mAnimation = GetComponent<SkeletonAnimation>();
            mMeshRenderer = mAnimation.GetComponent<MeshRenderer>();
            mMeshRenderer.sortingOrder = 11;
        }


        private void CharacterDataLoad(string characterDataKey)
        {
            var originData = DataBase_Manager.Instance.GetCharacter.GetData_Func(characterDataKey);
            CharacterDatas = global::Utils.DeepCopy(originData);
            CharacterDataInfo = ResourceManager.Instance.Load<CharacterInfo>(originData.characterPack);
            DefaultAttack = CharacterDataInfo.DefaultAttack;
            foreach (var skill in CharacterDataInfo.SkillList)
            {
                CoolTimeSkill(skill).Forget();
            }
        }
        
        
        public void Init(EUnitClass unitClass, EUnitRank unitRank,string characterDataKey)
        {
            CharacterDataLoad(characterDataKey);
            CurrentUnitClass = unitClass;
            CurrentUnitRank = unitRank;
            var mat = mMeshRenderer.materials;

            mAnimation.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{CurrentUnitClass}_{CurrentUnitRank}_{ELabelNames.SkeletonData}");
            mat[0] = ResourceManager.Instance.Load<Material>($"{CurrentUnitClass}_{CurrentUnitRank}_{ELabelNames.Material}");
            mMeshRenderer.materials = mat;
            string skinName = mAnimation.skeletonDataAsset.name;
            mAnimation.initialSkinName = skinName.Substring(0, skinName.LastIndexOf('_'));

            var skins = mAnimation.Skeleton.Data.Skins.ToList();
            mAnimation.Skeleton.SetSkin(skins[1]);
            
            mAnimation.Initialize(true);

            mAnimation.skeleton.SetSlotsToSetupPose();
            
            mAnimation.state.SetAnimation(0, "Idle_1", true);
            gameObject.name = mAnimation.initialSkinName;
            RunAttack();
        }
        private async UniTaskVoid CoolTimeSkill(SkillInfo skill)
        {
            await UniTask.WaitForSeconds(skill.CoolTime);
            ReadySkillQueue.Enqueue(skill.Skill);
        }


        // TODO 사거리에 안들어오면 실행 X 공격 Speed와 Skill Cool Time 따로 할 지 합칠 지 고민 중
        private void RunAttack()
        {
            Observable.Interval(System.TimeSpan.FromSeconds(CharacterDatas.atkSpeed))
                .Subscribe(_ =>
                {
                    DefaultAttack.CastAttack(this.transform,CharacterDatas);
                    UpdateState(EUnitStates.Attack);
                })
                .AddTo(this);
        }
        

        public void UpdateState(EUnitStates state)
        {
            switch (state)
            {
                case EUnitStates.Idle:
                    mAnimation.state.SetAnimation(0, "Idle_1", true);
                    break;

                case EUnitStates.Move:
                    mAnimation.state.SetAnimation(0, "Run_Weapon", true);
                    break;

                case EUnitStates.Attack:
                    mAnimation.state.SetAnimation(0, "Attack_1", false);
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, CharacterDatas.range);
        }
    }
}
