namespace _2_Scripts.UI.Ingame.LootBox
{
    public interface ILootBoxReward
    {

        public string RewardMessage();
        public void Reward();
        
        public bool CanReward();
    }
}