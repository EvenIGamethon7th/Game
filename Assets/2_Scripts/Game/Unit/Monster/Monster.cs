using _2_Scripts.Game.Unit;
using Rito.Attributes;
using UnityEngine;

namespace _2_Scripts.Game.Monster
{
    public class Monster : MonoBehaviour
    {
        [GetComponent] private SpriteRenderer mSprite;
        
        private MonsterData mMonsterData;
        public void SpawnMonster(string key)
        {
            var originData = DataBase_Manager.Instance.GetMonster.GetData_Func(key);
            mMonsterData = global::Utils.DeepCopy(originData);
            //TODO Sprite Change And Animation
        }
        
    }
}