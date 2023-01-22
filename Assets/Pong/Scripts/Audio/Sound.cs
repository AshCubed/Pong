using UnityEngine;

namespace Pong.Audio
{
    [System.Serializable]
    public class Sound
    {
        public string Name;
        public AudioClip Clip;

        public enum SoundTypes { MAIN_TITLE, UI, MUSIC, SFX };

        public SoundTypes SoundType;

        [Range(0f, 1f)]
        public float Volume = .5f;
        [Range(.1f, 3f)]
        public float Pitch = 1f;

        public bool Loop;

        [HideInInspector]
        public AudioSource Source;
    }
}
