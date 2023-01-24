using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pong.Audio
{
    public class ButtonClick : MonoBehaviour
    {
        [SerializeField] private AudioManager _audioManager;

        /// <summary>
        /// Adds <see cref="PlayButtonClick"/> method to every button in the scene>
        /// </summary>
        private void SetUpButtons()
        {
            LeanTween.delayedCall(0.5f, () =>
            {
                foreach (var item in FindObjectsOfType<UnityEngine.UI.Button>(true))
                {
                    item.onClick.AddListener(PlayButtonClick);
                }
                foreach (var item in FindObjectsOfType<UnityEngine.UI.Toggle>(true))
                {
                    item.onValueChanged.AddListener((bool x) => PlayButtonClick());
                }
            });
        }

        /// <summary>
        /// Plays one shot a button click sound from the Audio Manager
        /// </summary>
        private void PlayButtonClick()
        {
            _audioManager.PlayOneShot("ButtonClick");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetUpButtons();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
