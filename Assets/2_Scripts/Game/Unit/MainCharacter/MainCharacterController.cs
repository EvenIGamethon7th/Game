using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Game.ScriptableObject.Skill;
using _2_Scripts.Game.Unit.Data;
using _2_Scripts.UI.Ingame;
using _2_Scripts.Utils;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
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
        private MainCharacterInfo mCharacterInfo;
        [SerializeField]
        private CharacterData mCharacterData;

        [SerializeField]
        private float mCoolTime = 30;
        [SerializeField]
        private float mCurrentCoolTime = 0;
        [SerializeField]
        private GameObject mSkillTrigger;
        [SerializeField]
        private BuffData mBuffData;
        [SerializeField]
        private EMainCharacterSkillType mSkillType;

        private CancellationTokenSource mCts = new CancellationTokenSource();
        private GameMessage<float> mCoolTimeMessage;

        [SerializeField]
        private Vector2 mPos;

        private void Start()
        {
            //TODO 나중에 게임매니저에서 받아오기
            if (!GameManager.Instance.IsTest)
                mCharacterInfo = GameManager.Instance.CurrentMainCharacter;
            transform.position = mPos;
            mCharacterData = MemoryPoolManager<CharacterData>.CreatePoolingObject();
            mBuffData = MemoryPoolManager<BuffData>.CreatePoolingObject();
            mCharacterData.Init(mCharacterInfo.CharacterEvolutions[1].GetData, mBuffData);
            mCoolTime = mCharacterInfo.SkillList[0].CoolTime;
            mCoolTimeMessage = new GameMessage<float>(EGameMessage.MainCharacterCoolTime, 0);
            MessageBroker.Default.Publish(mCoolTimeMessage);
            mSkillTrigger.SetActive(false);

            if (mSkillType == EMainCharacterSkillType.Buff)
            {
                MessageBroker.Default.Receive<GameMessage<bool>>().
                    Where(message => message.Message == EGameMessage.MainCharacterSkillUse).
                    Subscribe(message =>
                    {
                        UseSkill();
                    }).AddTo(this);
            }

            else
            {
                MessageBroker.Default.Receive<GameMessage<Vector2>>().
                   Where(message => message.Message == EGameMessage.MainCharacterSkillUse).
                   Subscribe(message =>
                   {
                        UseSkill(message.Value);
                   }).AddTo(this);

                MessageBroker.Default.Receive<GameMessage<Vector2>>().
                   Where(message => message.Message == EGameMessage.MainCharacterSkillDuring).
                   Subscribe(message =>
                   {
                       SetSkillPos(message.Value);
                   }).AddTo(this);

                MessageBroker.Default.Receive<GameMessage<bool>>().
                   Where(message => message.Message == EGameMessage.MainCharacterSkillDuring).
                   Subscribe(message =>
                   {
                       SetSkillActive(message.Value);
                   }).AddTo(this);
            }
        }

        private async UniTask CheckSkillCoolTime()
        {
            while (mCurrentCoolTime > 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mCts.Token);
                mCurrentCoolTime -= Time.deltaTime;
                mCoolTimeMessage.SetValue(mCurrentCoolTime);
                MessageBroker.Default.Publish(mCoolTimeMessage);
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
            mCoolTimeMessage.SetValue(mCurrentCoolTime);
            MessageBroker.Default.Publish(mCoolTimeMessage);
            CheckSkillCoolTime().Forget();

            if (mSkillType == EMainCharacterSkillType.Attack && mSkillTrigger.activeSelf)
            {
                mCharacterInfo.SkillList[mCharacterData.rank - 1].Skill.CastAttack(mSkillTrigger.transform, mCharacterData);
                mSkillTrigger.SetActive(false);
            }

            else if (mSkillType == EMainCharacterSkillType.Buff)
            {
                mCharacterInfo.SkillList[mCharacterData.rank - 1].Skill.CastAttack(transform, mCharacterData);
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
            mCts.Cancel();
            mCts.Dispose();
            mCts = null;
            mBuffData.Clear();
            mCharacterData.Clear();
        }
    }
}