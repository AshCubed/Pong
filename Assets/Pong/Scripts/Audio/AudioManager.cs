using UnityEngine.Audio;
using System;
using UnityEngine;


namespace Pong.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _audioMixerMainGroup;
        [SerializeField] private AudioMixerGroup _audioMixersUi;
        [SerializeField] private AudioMixerGroup _audioMixersMusic;
        [SerializeField] private AudioMixerGroup _audioMixersSfx;
        [Space(10f)]
        [SerializeField] private Sound[] _sounds;

        public static AudioManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            //Creates audio sources for every sound in the list
            foreach (var s in _sounds)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.Clip;

                s.Source.volume = s.Volume;
                s.Source.pitch = s.Pitch;
                s.Source.loop = s.Loop;

                s.Source.outputAudioMixerGroup = s.SoundType switch
                {
                    Sound.SoundTypes.MAIN_TITLE => _audioMixersMusic,
                    Sound.SoundTypes.UI => _audioMixersUi,
                    Sound.SoundTypes.MUSIC => _audioMixersMusic,
                    Sound.SoundTypes.SFX => _audioMixersSfx,
                    _ => s.Source.outputAudioMixerGroup
                };
            }
        }

        private void Start()
        {
            PlaySounds("BackgroundMusic");
        }

        /// <summary>
        /// Play an audio source with the requested sound name
        /// </summary>
        /// <param name="audioName">Name of the audio which the user detailed in the Sounds array</param>
        public void PlaySounds(string audioName)
        {
            var s = Array.Find(_sounds, sound => sound.Name == audioName);

            if (s == null) return;

            if (!s.Source.isPlaying)
                s.Source.Play();
        }

        /// <summary>
        /// Play an audio source, one shot, with the requested sound name
        /// </summary>
        /// <param name="audioName">Name of the audio which the user detailed in the Sounds array</param>
        public void PlayOneShot(string audioName)
        {
            var foundSound = Array.Find(_sounds, sound => sound.Name == audioName);
            foundSound?.Source.PlayOneShot(foundSound.Clip);
        }

        /// <summary>
        /// Adjust the sound of the given named audio source name
        /// </summary>
        /// <param name="audioName">Name of the audio which the user detailed in the Sounds array</param>
        /// <param name="volume">Required level of volume</param>
        public void ChangeVolume(string audioName, float volume)
        {
            var s = Array.Find(_sounds, sound => sound.Name == audioName);

            if (s == null)
            {
                return;
            }

            if (s.Source.volume != volume)
            {
                s.Source.volume = volume;
            }
        }

        /// <summary>
        /// Stops an audio source with the requested sound name
        /// </summary>
        /// <param name="audioName">Name of the audio which the user detailed in the Sounds array</param>
        public void StopSounds(string audioName)
        {
            var s = Array.Find(_sounds, sound => sound.Name == audioName);
            if (s == null) return;
            if (s.Source.isPlaying)
                s.Source.Stop();
        }

        /// <summary>
        /// Stops all audio sources
        /// </summary>
        public void StopAll()
        {
            foreach (var s in _sounds)
            {
                s.Source.Stop();
            }
        }

        /// <summary>
        /// Fades out an audio source with the requested sound name
        /// </summary>
        /// <param name="audioName">Name of the audio which the user detailed in the Sounds array</param>
        /// <param name="onComplete">Method group which will be called when fade out is complete</param>
        public void FadeOutAudio(string audioName, Action onComplete)
        {
            var sound = GetSound(audioName);
            var v = sound.Source.volume;
            LeanTween.value(v, 0, 1f).setOnUpdate((float x) => {
                sound.Source.volume = x;
            }).setOnComplete(() => {
                onComplete?.Invoke();
                sound.Source.Stop();
                sound.Source.volume = sound.Volume;
            });
        }

        /// <summary>
        /// Attempts to find a sound object with the requested sound name
        /// </summary>
        /// <param name="audioName">Name of the audio which the user detailed in the Sounds array</param>
        /// <returns>Sound object to be used</returns>
        private Sound GetSound(string audioName)
        {
            var s = Array.Find(_sounds, sound => sound.Name == audioName);
            return s;
        }
    }
}
