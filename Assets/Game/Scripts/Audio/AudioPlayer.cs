using Game.Configs;
using UnityEngine;

namespace Game.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _loopSource;
        [SerializeField, Min(1)] private int _voiceCount = 8;

        private AudioSource[] _voices;
        private int _nextVoice;

        private void Awake()
        {
            _voices = new AudioSource[_voiceCount];
            for (int i = 0; i < _voiceCount; i++)
            {
                _voices[i] = gameObject.AddComponent<AudioSource>();
                _voices[i].playOnAwake = false;
            }
        }

        public void Play(SoundEffect effect)
        {
            if (effect?.Clip == null)
                return;

            AudioSource voice = _voices[_nextVoice];
            _nextVoice = (_nextVoice + 1) % _voices.Length;

            voice.clip = effect.Clip;
            voice.volume = effect.Volume;
            voice.pitch = 1f + Random.Range(-effect.PitchJitter, effect.PitchJitter);
            voice.Play();
        }

        public void StartLoop(AudioClip clip, float volume)
        {
            if (clip == null || _loopSource.isPlaying)
                return;

            _loopSource.clip = clip;
            _loopSource.volume = volume;
            _loopSource.loop = true;
            _loopSource.Play();
        }

        public void SetLoopPitch(float pitch) =>
            _loopSource.pitch = pitch;

        public void StopLoop() =>
            _loopSource.Stop();
    }
}
