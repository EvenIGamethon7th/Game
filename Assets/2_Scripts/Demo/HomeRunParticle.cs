using UnityEngine;

namespace _2_Scripts.Demo
{
    public class HomeRunParticle : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;

        public void SetTextureParticle(Sprite sprite)
        {
            particleSystem.textureSheetAnimation.SetSprite(0, sprite);
        }
    }
}