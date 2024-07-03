﻿namespace _2_Scripts.Utils
{
    public enum EGameMessage
    {
        PlayerDamage,
        BossDeath,
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
    }
}