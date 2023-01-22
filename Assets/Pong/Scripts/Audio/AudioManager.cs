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
            foreach (Sound s in _sounds)
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

        public void PlaySounds(string audioName)
        {
            var s = Array.Find(_sounds, sound => sound.Name == audioName);

            if (s == null)
            {
                //Debug.Log("Sound: " + name + " not found!");
                return;
            }

            if (!s.Source.isPlaying)
            {
                //Debug.Log("Sound: " + name + " Playing!");
                s.Source.Play();
            }
        }

        public void PlayOneShot(string audioName)
        {
            Sound s = Array.Find(_sounds, sound => sound.Name == audioName);

            if (s == null)
            {
                //Debug.Log("Sound: " + name + " not found!");
                return;
            }

            s.Source.PlayOneShot(s.Clip);
        }

        public void ChangeVolume(string audioName, float volume)
        {
            Sound s = Array.Find(_sounds, sound => sound.Name == audioName);

            if (s == null)
            {
                //Debug.Log("Sound: " + name + " not found!");
                return;
            }

            if (s.Source.volume != volume)
            {
                //Debug.Log("Sound: " + name + " Volume Adujusted to " + amnt);
                s.Source.volume = volume;
            }
        }

        public void StopSounds(string audioName)
        {
            var s = Array.Find(_sounds, sound => sound.Name == audioName);
            if (s == null)
            {
                // Debug.Log("Sound: " + name + " not found!");
                return;
            }
            if (s.Source.isPlaying)
            {
                s.Source.Stop();
            }
        }

        public void StopAll()
        {
            foreach (var s in _sounds)
            {
                s.Source.Stop();
            }
        }

        public void FadeOutAudio(string audioName, Action onComplete)
        {
            Sound sound = GetSound(audioName);
            float v = sound.Source.volume;
            LeanTween.value(v, 0, 1f).setOnUpdate((float x) => {
                sound.Source.volume = x;
            }).setOnComplete(() => {
                onComplete?.Invoke();
                sound.Source.Stop();
                sound.Source.volume = sound.Volume;
            });
        }

        public Sound GetSound(string audioName)
        {
            Sound s = Array.Find(_sounds, sound => sound.Name == audioName);
            return s;
        }
    }
}
