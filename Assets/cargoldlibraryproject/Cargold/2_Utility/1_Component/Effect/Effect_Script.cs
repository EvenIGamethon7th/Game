using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold.PoolingSystem;
using Cargold;
using Cargold.FrameWork;

namespace Cargold.Effect
{
    public abstract class Effect_Script : MonoBehaviour, IPooler
    {
        public const string EffectGroup = "/이펙트";
        public const string SfxGroup = "/효과음";

        private static List<Effect_Script> EffectClassList = new List<Effect_Script>();

        [BoxGroup(Cargold.CargoldLibrary_C.Mandatory)] [SerializeField] protected Cargold.Effect.EffectKey effectType = Cargold.Effect.EffectKey.None;
        [BoxGroup(Cargold.CargoldLibrary_C.Mandatory), SerializeField] private Effect_SetPos_Script setPosClass = null;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional)] [SerializeField] protected Effect_Element_Script[] elementClassArr = null;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional)] [SerializeField] private BoneType boneType = BoneType.None;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional)] [SerializeField] private Vector2 offsetPos = Vector2.zero;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("위치를 따라다닐 건가요?")] [SerializeField] private bool isFollowPos = false;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("회전시킬 건가요?")] [SerializeField] private bool isFollowRot = false;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("대상의 좌우 방향에 따를 건가요?"), SuffixLabel("IFS만 가능")] [SerializeField] private bool isFollowLookDir = false;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("풀링 반환 시 히어라키 계층 이동 여부")] [SerializeField] private bool isPoolDespawn_NotSetParent = true;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional), LabelText("자체 비활성화 여부")] [SerializeField] private bool isSelfDeactivate = true;
        [FoldoutGroup(Cargold.CargoldLibrary_C.Optional + SfxGroup), LabelText("활성화 효과음")] [SerializeField] private SfxType sfxType = SfxType.None;
        private ITargetByEffect iTarget;
        private Transform targetTrf;
        private CoroutineData selfDeactivateCorData;

        public Cargold.Effect.EffectKey EffectType => this.effectType;
        public virtual string GetPoolingKey => this.effectType.ToString_Func();
        public BoneType GetSpawnBoneType => this.boneType;
        public abstract EffectActivateType GetEffectActivateType { get; }

        public virtual void Init_Func()
        {
#if UNITY_EDITOR
            if (this.setPosClass == null)
                Debug_C.Error_Func("다음의 이펙트에 SetPos가 Null입니다. : " + this.transform.GetPath_Func()); 
#endif

            foreach (Effect_Element_Script _elementClass in this.elementClassArr)
                _elementClass.Init_Func(this.CallDel_ElementPlayDone_Func);

            this.Deactivate_Func(true);
        }

        public void Activate_Func(ITargetByEffect _iTarget)
        {
            this.ActivateDone_Func();

            this.iTarget = _iTarget;

            this.SetFollow_Func(TargetType.Bone);
        }
        public void Activate_Func(Transform _targetTrf)
        {
            this.ActivateDone_Func();

            this.targetTrf = _targetTrf;

            this.SetFollow_Func(TargetType.Transform);
        }
        public void Activate_Func(Vector2 _pos)
        {
            this.ActivateDone_Func();

            this.SetPos_Func(_pos, _pos);

            this.transform.rotation = Quaternion.identity;

            this.SetDir_Func(TargetType.None);
        }
        protected virtual void ActivateDone_Func()
        {
            this.gameObject.SetActive(true);

            foreach (Effect_Element_Script _elementClass in elementClassArr)
                _elementClass.Activate_Func();

            if (sfxType != SfxType.None)
                Cargold.FrameWork.SoundSystem_Manager.Instance.PlaySfx_Func(this.sfxType);

            Effect_Script.EffectClassList.AddNewItem_Func(this);

            if(this.isSelfDeactivate == true)
                this.selfDeactivateCorData.StartCoroutine_Func(this.SelfDeactivate_Cor());
        }
        private IEnumerator SelfDeactivate_Cor()
        {
            while (this.gameObject.activeInHierarchy == true)
                yield return null;

            this.Deactivate_Func();
        }

        private IEnumerator FollowIFS_Cor(FollowType _followType, TargetType _targetType)
        {
            #region Transform
            if (_targetType == TargetType.Transform)
            {
                if (_followType == FollowType.Pos)
                {
                    while ((this.targetTrf is null) == false)
                    {
                        this.SetPos_Func(_targetType);

                        yield return null;
                    }
                }
                else if (_followType == FollowType.Rot)
                {
                    while ((this.targetTrf is null) == false)
                    {
                        this.SetRot_Func(_targetType);

                        yield return null;
                    }
                }
                else if (_followType == FollowType.Dir)
                {
                    while ((this.targetTrf is null) == false)
                    {
                        this.SetDir_Func(_targetType);

                        yield return null;
                    }
                }
                else
                    Debug_C.Error_Func("_followType : " + _followType);
            }
            #endregion
            #region Bone
            else if (_targetType == TargetType.Bone)
            {
                switch (_followType)
                {
                    case FollowType.Pos:
                        {
                            while ((this.iTarget is null) == false)
                            {
                                this.SetPos_Func(_targetType);

                                yield return null;
                            }
                        }
                        break;

                    case FollowType.Rot:
                        {
                            while ((this.iTarget is null) == false)
                            {
                                this.SetRot_Func(_targetType);

                                yield return null;
                            }
                        }
                        break;

                    case FollowType.Dir:
                        {
                            while ((this.iTarget is null) == false)
                            {
                                this.SetDir_Func(_targetType);

                                yield return null;
                            }
                        }
                        break;

                    case FollowType.None:
                    default:
                        Debug_C.Error_Func("_followType : " + _followType);
                        break;
                }
            } 
            #endregion
            else
            {
                Debug_C.Error_Func("_targetType : " + _targetType);
            }
        }

        protected virtual void ElementPlayDone_Func(Effect_Element_Script _elementClass) { }
        public virtual void Deactivate_Func(bool _isInit = false, bool _isOutList = true)
        {
            foreach (Effect_Element_Script _elementClass in elementClassArr)
                _elementClass.Deactivate_Func();

            this.iTarget = null;
            this.targetTrf = null;

            if (_isInit == false)
            {
                PoolingSystem_Manager.Instance.Despawn_Func(this.GetPoolingKey, this, this.isPoolDespawn_NotSetParent);

                if (_isOutList == true)
                    Effect_Script.EffectClassList.Remove_Func(this);
            }

            if (this.isSelfDeactivate == true)
                this.selfDeactivateCorData.StopCorountine_Func();

            this.gameObject.SetActive(false);
        }

        private void SetFollow_Func(TargetType _targetType)
        {
            if (this.isFollowPos == false)
                this.SetPos_Func(_targetType);
            else
                StartCoroutine(this.FollowIFS_Cor(FollowType.Pos, _targetType));

            if (this.isFollowRot == false)
                this.SetRot_Func(TargetType.None);
            else
                StartCoroutine(this.FollowIFS_Cor(FollowType.Rot, _targetType));

            if (this.isFollowLookDir == false)
                this.SetDir_Func(TargetType.None);
            else
                StartCoroutine(this.FollowIFS_Cor(FollowType.Dir, _targetType));
        }
        private void SetPos_Func(TargetType _targetType)
        {
            Vector2 _pos = Vector2.zero;
            Vector2 _layerPos = Vector2.zero;

            if (_targetType == TargetType.Bone)
            {
                _pos = this.iTarget.GetBonePos_Func(this.boneType);

                _layerPos = this.iTarget.GetPos_Func();
            }
            else if (_targetType == TargetType.Transform)
            {
                _pos = (Vector2)this.targetTrf.position;

                _layerPos = (Vector2)this.targetTrf.position;
            }
            else
                Debug_C.Error_Func("_targetType : " + _targetType);

            _pos += this.offsetPos;
            this.SetPos_Func(_pos, _layerPos);
        }
        private void SetPos_Func(Vector2 _pos, Vector2 _layerPos)
        {
            this.setPosClass.SetPos_Func(_pos, _layerPos);
        }
        private void SetRot_Func(TargetType _targetType = TargetType.None)
        {
            Quaternion _quat = Quaternion.identity;

            if (_targetType == TargetType.Bone)
                _quat = this.iTarget.GetBoneRot_Func(this.boneType);
            else if (_targetType == TargetType.Transform)
                _quat = this.targetTrf.rotation;

            this.transform.rotation = _quat;
        }
        private void SetDir_Func(TargetType _targetType = TargetType.None)
        {
            bool _isLookDir = true;

            if (_targetType == TargetType.Bone)
                _isLookDir = iTarget.GetLookDir_Func();
            else if (_targetType == TargetType.Transform)
                _isLookDir = 0f < this.targetTrf.localScale.x;
            else
                _isLookDir = true;

            float _dir = _isLookDir == true ? this.transform.localScale.x : -this.transform.localScale.x;
            this.transform.SetScaleRight_Func(_dir);
        }

        public float GetPosY_Func()
        {
            return this.transform.position.y;
        }
        public void SetSortingOrder_Func(int _layer)
        {
            if (this.gameObject.activeSelf == true)
            {
                foreach (Effect_Element_Script _elementClass in elementClassArr)
                    _elementClass.SetSortingOrder_Func(_layer);
            }
        }

        private void CallDel_ElementPlayDone_Func(Effect_Element_Script _elementClass)
        {
            this.ElementPlayDone_Func(_elementClass);
        }

#if UNITY_EDITOR
        public void Reset()
        {
            this.CallEdit_Catching_Func();
        }
        [Button("캐싱 ㄱㄱ~")]
        protected virtual void CallEdit_Catching_Func()
        {
            if(this.setPosClass == null)
            {
                if(this.transform is RectTransform == true)
                    this.setPosClass = this.gameObject.AddComponent<Effect_SetPos_Rtrf_Script>();
                else
                    this.setPosClass = this.gameObject.AddComponent<Effect_SetPos_Trf_Script>();
            }

            ParticleSystem[] _psArr = this.transform.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem _ps in _psArr)
            {
                if (_ps.TryGetComponent(out Effect_Element_ParticleSystem_Script _elemClass) == false)
                    _ps.gameObject.AddComponent<Effect_Element_ParticleSystem_Script>();
            }

            Animator[] _animatorArr = this.transform.GetComponentsInChildren<Animator>();
            foreach (Animator _animator in _animatorArr)
            {
                if (_animator.TryGetComponent(out Effect_Element_Animator_Script _elemClass) == false)
                    _animator.gameObject.AddComponent<Effect_Element_Animator_Script>();
            }

            Effect_Element_Script[] _effElemClassArr = this.gameObject.GetComponentsInChildren<Effect_Element_Script>();
            this.CallEdit_Catching_Func(_effElemClassArr);
        }
        protected virtual void CallEdit_Catching_Func(Effect_Element_Script[] _effElemClassArr)
        {
            this.elementClassArr = _effElemClassArr;

            if (this.elementClassArr != null)
            {
                foreach (Effect_Element_Script _elementClass in this.elementClassArr)
                    _elementClass.CallEditor_Catching_Func();
            }
            else
                Debug_C.Error_Func("Element 없는디?");

            if (this.TryGetComponent(out Effect_SetPos_Script _setPosClass) == true)
                this.setPosClass = _setPosClass;
        }
        public void CallEdit_SetPos_Func(Effect_SetPos_Script _setposClass)
        {
            this.setPosClass = _setposClass;
        }
        public void CallUnitTest_Func()
        {
            bool _isError = false;

            if (this.effectType == EffectKey.None)
            {
                _isError = true;
                Debug.LogError("이펙트 타입이 세팅되지 않고 None으로 되어있음");
            }

            foreach (var _elementClass in this.elementClassArr)
            {
                if (_elementClass is null)
                {
                    _isError = true;

                    Debug.LogError("이펙트 엘렘이 Null임");
                }
            }

            if (_isError == true)
            {
                Debug.LogError(this.gameObject.name);
            }

            this.CallEdit_Catching_Func();
        } 

        [Button("PS) ScalingMode : Hierarchy")]
        protected void CallEdit_Catching_PS_Func()
        {
            foreach (Effect_Element_Script _elementClass in this.elementClassArr)
            {
                if (_elementClass is Effect_Element_ParticleSystem_Script)
                {
                    Effect_Element_ParticleSystem_Script _psElemClass = _elementClass as Effect_Element_ParticleSystem_Script;

                    ParticleSystem _ps = _psElemClass.GetPS;
                    ParticleSystem.MainModule _mainModule = _ps.main;
                    _mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
                }
            }

            this.transform.GetChild(0).localScale = Vector3.one * 100f;

            Prefab_C.Apply_Func(this.gameObject);
        }
#endif

        void IPooler.InitializedByPoolingSystem()
        {
            this.Init_Func();
        }

        private static Effect_Script Spawn_Func(EffectKey _effectKey)
        {
            string _effectKeyStr = _effectKey.ToString_Func();
            return PoolingSystem_Manager.Instance.Spawn_Func<Effect_Script>(_effectKeyStr);
        }
        public static Effect_Script Activate_Func(EffectKey _effectKey, ITargetByEffect _targetCharClass, bool _isActive = true)
        {
            Effect_Script _effectClass = Effect_Script.Spawn_Func(_effectKey);

            if (_isActive == true)
                _effectClass.Activate_Func(_targetCharClass);

            return _effectClass;
        }
        public static Effect_Script Activate_Func(EffectKey _effectKey, Transform _targetTrf, bool _isActive = true)
        {
            Effect_Script _effectClass = Effect_Script.Spawn_Func(_effectKey);

            if (_isActive == true)
                _effectClass.Activate_Func(_targetTrf);

            return _effectClass;
        }
        public static Effect_Script Activate_Func(EffectKey _effectKey, Vector2 _targetPos, bool _isActive = true)
        {
            Effect_Script _effectClass = Effect_Script.Spawn_Func(_effectKey);

            if (_isActive == true)
                _effectClass.Activate_Func(_targetPos);

            return _effectClass;
        }
        public static Effect_Script Activate_Func(string _effectKeyStr, Transform _targetTrf, bool _isActive = true)
        {
            Effect_Script _effectClass = PoolingSystem_Manager.Instance.Spawn_Func<Effect_Script>(_effectKeyStr);

            if (_isActive == true)
                _effectClass.Activate_Func(_targetTrf);

            return _effectClass;
        }
        public static Effect_Script Activate_Func(string _effKeyStr, RectTransform _targetRtrf, bool _isActive = true)
        {
            Effect_Script _effectClass = PoolingSystem_Manager.Instance.Spawn_Func<Effect_Script>(_effKeyStr, _targetRtrf);

            if (_isActive == true)
                _effectClass.Activate_Func(Vector2.zero);

            return _effectClass;
        }

        public static void DeactivateAll_Func()
        {
            foreach (Effect_Script _effectClass in Effect_Script.EffectClassList)
                _effectClass.Deactivate_Func(_isOutList: false);

            Effect_Script.EffectClassList.Clear();
        }

        public enum EffectActivateType
        {
            None = 0,

            Timer,
            OneShot,
            WaitCall,
        }

        private enum FollowType
        {
            None = 0,

            Pos,
            Rot,
            Dir,
        }

        private enum TargetType
        {
            None = 0,

            Bone,
            Transform,
        }

        public interface ITargetByEffect
        {
            Vector2 GetBonePos_Func(BoneType _boneType);
            Vector2 GetPos_Func();
            Quaternion GetBoneRot_Func(BoneType _boneType);
            bool GetLookDir_Func();
        }

        public enum BoneType
        {
            None = 0,
        }
    } 
}