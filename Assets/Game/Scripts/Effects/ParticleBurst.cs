using UnityEngine;

namespace Game.Effects
{
    public class ParticleBurst : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private Color _color = Color.white;

        public void Burst(Vector3 position, int count)
        {
            if (count <= 0)
                return;

            if (!_particles.isPlaying)
                _particles.Play();

            var emitParams = new ParticleSystem.EmitParams
            {
                position = position,
                applyShapeToPosition = true,
                startColor = _color
            };
            _particles.Emit(emitParams, count);
        }

        public void Clear() =>
            _particles.Clear();
    }
}
