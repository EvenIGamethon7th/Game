using _2_Scripts.Game.ScriptableObject.Skill;
using _2_Scripts.UI.Ingame;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace _2_Scripts.Game.Unit.MainCharacter
{
    public class MainCharacterController : MonoBehaviour
    {
        public enum EMainCharacterSkillType
        {
            Buff,
            Attack
        }

        [SerializeField]
        private float mCoolTime = 30;
        [SerializeField]
        private float mCurrentCoolTime = 0;
        [SerializeField]
        private UI_MainCharacter mMainCharacterUI;
        [SerializeField]
        private GameObject mSkillTrigger;
        [SerializeField]
        private EMainCharacterSkillType mSkillType;

        private CancellationTokenSource mCts = new CancellationTokenSource();

        private void Start()
        {
            if (mSkillType == EMainCharacterSkillType.Buff)
            {
                mMainCharacterUI.PointerUp += UseSkill;
            }

            else
            {
                mMainCharacterUI.EndDrag += UseSkill;
                mMainCharacterUI.Drag += SetSkillPos;
                mMainCharacterUI.OnImage += SetSkillActive;
            }
        }

        private async UniTask CheckSkillCoolTime()
        {
            while (mCurrentCoolTime > 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCts.Token);
                mCurrentCoolTime -= Time.deltaTime;
                mMainCharacterUI.SetCoolTime(mCurrentCoolTime);
            }
        }

        private void UseSkill()
        {
            if (!CheckCondition())
            {
                //TODO: 스킬 사용 불가
                return;
            }
            mCurrentCoolTime = mCoolTime;
            mMainCharacterUI.SetCoolTime(mCoolTime);
            CheckSkillCoolTime().Forget();

            if (mSkillTrigger.activeSelf)
            {
                mSkillTrigger.TryGetComponent<TestMainCharacterSkill>(out var test);
                test.UseSkill();
                mSkillTrigger.SetActive(false);
            }
        }

        private void SetSkillActive(bool active)
        {
            mSkillTrigger.SetActive(!active && mCurrentCoolTime <= 0);
        }

        private void SetSkillPos(Vector2 mousePos)
        {
            mSkillTrigger.transform.position = mousePos;
        }

        private void UseSkill(Vector2 mousePos)
        {
            mSkillTrigger.transform.position = mousePos;
            UseSkill();
        }

        private bool CheckCondition()
        {
            if (mCurrentCoolTime > 0) return false;

            if (EMainCharacterSkillType.Buff == mSkillType) return true;

            if (!mSkillTrigger.activeSelf) return false;

            return true;
        }

        private void OnDestroy()
        {
            mMainCharacterUI.PointerUp -= UseSkill;
            mMainCharacterUI.EndDrag -= UseSkill;
            mMainCharacterUI.Drag -= SetSkillPos;
            mMainCharacterUI.OnImage -= SetSkillActive;

            mCts.Cancel();
            mCts.Dispose();
            mCts = null;
        }
    }
}