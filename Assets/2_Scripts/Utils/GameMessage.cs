using System;

namespace _2_Scripts.Utils
{
    public enum EGameMessage
    {
        BossDeath,
        StageChange,
        ExpChange,
        SelectCharacter,
        GoAcademy,
        MainCharacterSkillDuring,
        MainCharacterSkillUse,
        MainCharacterCoolTime,
        GameStartPopUpOpen,
        ChapterChange,
        CharacterCardChange,
        MonsterHp,
        MainCharacterChange
    }
    
    public class GameMessage<T>
    {
        public EGameMessage Message { get; private set; }
        public T Value { get; private set; }

        public GameMessage(EGameMessage message,T value)
        {
            Value = value;
            Message = message;
        }

        public void SetValue(T value)
        {
            Value = value;
        }
    }
}