using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class MissionReward : SerializedMonoBehaviour
    {
        [SerializeField] 
        public IItemAcquisition ItemAcquisition { get; private set; }
    }
}