
    //  HP , 재화 , 씬 이동 등 여러가지 관리 목적

    using System;
    using _2_Scripts.Utils;
    using UniRx;

    public class GameManager : Singleton<GameManager>
    {
        
        public float UserHp { get; private set; } = 100;
        // 학년
        public int UserLevel { get; private set; } = 1;
        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<float>>().Where(message => message.Message == EGameMessage.PlayerDamage)
                .Subscribe(message =>
                {
                    UserHp -= message.Value;
                });
        }
    }
