using System;
using Rito.Attributes;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Game.ScriptableObject.Skill;
using _2_Scripts.Utils.Structure;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

namespace _2_Scripts.Game.Unit
{
    using _2_Scripts.Game.Monster;
    using _2_Scripts.Game.ScriptableObject.Skill.Passive;

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
        None,
        Idle,
        Move,
        Attack
    }

    public class CUnit : MonoBehaviour
    {
        [field: SerializeField]
        public CharacterInfo CharacterDataInfo { get; private set; }
        [field: SerializeField]
        public CharacterData CharacterDatas { get; private set; }
        private MeshRenderer mMeshRenderer;
        private SkeletonAnimation mAnimation;
        private Transform mOriginParent;
        
        public delegate void FSMAction();

        private Dictionary<EUnitStates, FSMAction> mActions = new ();
        public EUnitStates CurrentState { get; private set; } = EUnitStates.None;
        public Dictionary<Skill,SkillInfo> SkillInfoDictionary { get; private set; } = new Dictionary<Skill, SkillInfo>();
        public UniqueQueue<SkillInfo> ReadySkillQueue { get; private set; } = new UniqueQueue<SkillInfo>();

        private Action<Monster[]> mBeforePassive;
        private Action<Monster> mAfterPassive;

        public bool IsNotCoolTimeSKill = false;

        private void Awake()
        {
            mAnimation = GetComponent<SkeletonAnimation>();
            mMeshRenderer = mAnimation.GetComponent<MeshRenderer>();
            mMeshRenderer.sortingOrder = 11;
            foreach (var state in Enum.GetValues(typeof(EUnitStates)))
            {
                mActions.Add((EUnitStates)state, () => { });
            }

            InitActionAnimation();
            mOriginParent = transform.parent;
        }

        public void SetFlipUnit(Transform target)
        {
            int flip = target.transform.position.x > this.transform.position.x ? 1 : -1;
            mAnimation.skeleton.ScaleX = flip;
        }

        public void AddReadySkill(SkillInfo skill)
        {
            ReadySkillQueue.Enqueue(skill);
        }

        //임시 스킬 업데이트 함수
        private void Update()
        {
            if (ReadySkillQueue.Count != 0)
            {
              var skill = ReadySkillQueue.Peek();
              var isRange = skill.Skill.CastAttack(this.transform,CharacterDatas);
              ReadySkillQueue.Dequeue();
              if (isRange && IsNotCoolTimeSKill == false)
              {
                CoolTimeSkill(skill).Forget();
              }
              else
              {
                  AddReadySkill(skill);   
                  IsNotCoolTimeSKill = false;
              }
            }
        }

        /// <summary>
        ///  임시 mActions 초기화
        /// </summary>
        private void InitActionAnimation()
        {
            mActions[EUnitStates.Idle] = () =>
            {
                CurrentState = EUnitStates.Idle;
                mAnimation.state.SetAnimation(0, "Idle_1", true);
            };
            mActions[EUnitStates.Move] = () =>
            {
                CurrentState = EUnitStates.Move;
                mAnimation.state.SetAnimation(0, "Run_Weapon", true);
            };
            mActions[EUnitStates.Attack] = () =>
            {
                CurrentState = EUnitStates.Attack;
                mAnimation.state.SetAnimation(0, "Attack_1", false);
            };
        }
        
        private void CharacterDataLoad(CharacterData characterData)
        {
            CharacterDatas = global::Utils.DeepCopy(characterData);
            CharacterDataInfo = ResourceManager.Instance.Load<CharacterInfo>(characterData.characterData);
            
            CharacterDataInfo.ActiveSkillList?.Where(skill=> skill.Level <= CharacterDatas.rank).ForEach(skill =>
            {
                SkillInfoDictionary.Add(skill.Skill,skill);
                CoolTimeSkill(skill).Forget();
            });
            CharacterDataInfo.PassiveSkillList?.Where(skill => skill.Level <= CharacterDatas.rank).ForEach(skill =>
            {
                if (skill.Skill is BeforePassive)
                {
                    var s = skill.Skill as BeforePassive;
                    mBeforePassive += s.BeforeDamage;
                }
                else if (skill.Skill is AfterPassive)
                {
                    var s = skill.Skill as AfterPassive;
                    mAfterPassive += s.AfterDamage;
                }
            });
        }
        
        public void Init(CharacterData characterData)
        {
            CharacterDataLoad(characterData);
            var mat = mMeshRenderer.materials;

            mAnimation.skeletonDataAsset = ResourceManager.Instance.Load<SkeletonDataAsset>($"{characterData.characterPack}_{ELabelNames.SkeletonData}");
            mat[0] = ResourceManager.Instance.Load<Material>($"{characterData.characterPack}_{ELabelNames.Material}");
            mMeshRenderer.materials = mat;
            string skinName = mAnimation.skeletonDataAsset.name;
            mAnimation.initialSkinName = skinName.Substring(0, skinName.LastIndexOf('_'));

            var skins = mAnimation.Skeleton.Data.Skins.ToList();
            mAnimation.Skeleton.SetSkin(skins[1]);
            
            mAnimation.Initialize(true);

            mAnimation.skeleton.SetSlotsToSetupPose();

            UpdateState(EUnitStates.Idle);
            gameObject.name = mAnimation.initialSkinName;
        }

        public bool DefaultAttack()
        {
            return CharacterDataInfo.DefaultAttack.CastAttack(transform, CharacterDatas, mBeforePassive, mAfterPassive);
        }

        private async UniTaskVoid CoolTimeSkill(SkillInfo skill)
        {
            await UniTask.WaitForSeconds(skill.CoolTime);
            AddReadySkill(skill);
        }

        public void UpdateState(EUnitStates state)
        {
            mActions[state].Invoke();
        }

        public void AddActionState(EUnitStates state, FSMAction action)
        {
            mActions[state] += action;
        }
        public void RemoveActionState(EUnitStates state, FSMAction action)
        {
            mActions[state] -= action;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, CharacterDatas.range);
        }

        public void Clear()
        {
            mBeforePassive = null;
            mAfterPassive = null;
            ReadySkillQueue.Clear();
            gameObject.SetActive(false);
            transform.parent = mOriginParent;
        }
    }
}
