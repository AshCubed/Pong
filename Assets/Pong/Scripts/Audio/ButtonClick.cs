using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pong.Audio
{
    public class ButtonClick : MonoBehaviour
    {
        [SerializeField] private AudioManager _audioManager;

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
