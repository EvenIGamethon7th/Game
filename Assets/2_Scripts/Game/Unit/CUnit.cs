using System;
using Rito.Attributes;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    using _2_Scripts.Buff;
    using _2_Scripts.Game.Monster;
    using _2_Scripts.Game.ScriptableObject.Skill.Passive;
    using _2_Scripts.Game.ScriptableObject.Skill.Passive.Buff;
    using System.Net;
    using Unity.VisualScripting;
    using static UnityEngine.GraphicsBuffer;

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
        [field: SerializeField]
        public UniqueQueue<SkillInfo> ReadySkillQueue { get; private set; } = new UniqueQueue<SkillInfo>();

        private Action<Monster[]> mBeforePassive;
        private Action<Monster> mAfterPassive;

        private List<BuffTrigger> mBuffs = new ();

        public bool IsNotCoolTimeSKill = false;
        private GameObject mAlumniEffect;

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
            int flip = target.position.x > transform.position.x ? 1 : -1;
            mAnimation.skeleton.ScaleX = flip;
        }

        public void SetFlipUnit(Vector3 targetPos)
        {
            int flip = targetPos.x > transform.position.x ? 1 : -1;
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
              if (isRange)
              {
                  if (IsNotCoolTimeSKill)
                  {
                      IsNotCoolTimeSKill = false;
                      CoolTimeSkill(1,skill).Forget();
                  }
                  else
                  {
                      CoolTimeSkill(skill).Forget();
                  }
              }
              else
              {
                  AddReadySkill(skill);   
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
            CharacterDatas = MemoryPoolManager<CharacterData>.CreatePoolingObject();
            if (!TryGetComponent<UnitBuffHandler>(out var handler)) Debug.LogError("Has not Handler!!");
            CharacterDatas.Init(characterData, handler.BuffData);

            CharacterDataInfo = ResourceManager.Instance.Load<CharacterInfo>(characterData.characterData);
            
            CharacterDataInfo.ActiveSkillList?
                .Where(skill=> skill.Level <= CharacterDatas.rank)
                .ForEach(skill =>
                CoolTimeSkill(skill).Forget());
            CharacterDataInfo.PassiveSkillList?.
                Where(skill => skill.Level <= CharacterDatas.rank)
                .ForEach(skill =>
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

                    else if (skill.Skill is PassiveBuff)
                    {
                        if (!ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_BuffTrigger, transform.position).TryGetComponent<BuffTrigger>(out var trigger)) return;
                        mBuffs.Add(trigger);
                        trigger.Init(skill.Skill as PassiveBuff);
                    }
                });
            
            if (CharacterDatas.isAlumni)
            {
                mAlumniEffect = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_Magic_Effect_11, transform.position);
                mAlumniEffect.transform.parent = transform;
            }
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

        private CancellationTokenSource mCancleToken = new CancellationTokenSource();

        private void OnEnable()
        {
            mCancleToken = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            CancelAndDisposeToken();
        }

        private async UniTaskVoid CoolTimeSkill(SkillInfo skill)
        {

            await UniTask.WaitForSeconds(skill.CoolTime,cancellationToken:mCancleToken.Token);
            AddReadySkill(skill);
        }

        private async UniTaskVoid CoolTimeSkill(float time,SkillInfo skill)
        {
         
            await UniTask.WaitForSeconds(time,cancellationToken:mCancleToken.Token);
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
            for (int i = 0; i < mBuffs.Count; ++i)
            {
                mBuffs[i].Clear();
            }
            gameObject.SetActive(false);
            transform.parent = mOriginParent;
            mAlumniEffect?.SetActive(false);
        }
        
        private void CancelAndDisposeToken()
        {
            if (mCancleToken != null)
            {
                if (!mCancleToken.IsCancellationRequested)
                {
                    mCancleToken.Cancel();
                }
                mCancleToken.Dispose();
                mCancleToken = null;
            }
            else
            {
                mCancleToken = new CancellationTokenSource();
            }
        }
    }
}
