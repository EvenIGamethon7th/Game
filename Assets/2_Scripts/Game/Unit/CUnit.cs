using Rito.Attributes;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        private MeshRenderer mMeshRenderer;
        private SkeletonAnimation mAnimation;

        private void Awake()
        {
            mAnimation = GetComponent<SkeletonAnimation>();
            mMeshRenderer = mAnimation.GetComponent<MeshRenderer>();
            mMeshRenderer.sortingOrder = 11;
        }

        public void Init(EUnitClass unitClass, EUnitRank unitRank)
        {
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
    }
}
