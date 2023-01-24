using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.InputSystem;

namespace Pong.Managers
{
    public interface IPause
    {
        /// <summary>
        /// Subscribes to an event in Settings Manager, this is called whenever the game is paused
        /// </summary>
        public void Pause();
        /// <summary>
        /// Subscribes to an event in Settings Manager, this is called whenever the game is resumed
        /// </summary>
        public void Resume();
    }

    public class SettingsManager : MonoBehaviour, IPause
    {
        [SerializeField] private InputActionReference _inputActionReferencePause;
        [SerializeField] private CanvasGroup _settingsCanvas;
        [Tooltip("Add any UI that must be inactive for users to be able to pause/unpause the game")]
        [SerializeField] private GameObject[] _objectsCheck;
        [Header("Buttons")]
        [SerializeField] private Button _btnExit;
        [SerializeField] private Button _btnContinue;
        [SerializeField] private Button _btnPause;
        [Header("Audio")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private float _volumeMin, _volumeMax;
        [SerializeField] private Slider _sliderMainV;
        [SerializeField] private Slider _sliderSfxV;
        [SerializeField] private Slider _sliderUiV;
        [SerializeField] private Slider _sliderMusicV;
        
        private bool _isPaused;
        private Action _onGamePause;
        private Action _onGameResume;

        private void Awake()
        {
            LoadPlayerPrefsSettingsValues();
            UiSetUp();
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;

            //Find all MonoBehaviours which use the IPause interface and add them to the relevant actions.
            foreach (var item in FindObjectsOfType<MonoBehaviour>().OfType<IPause>())
            {
                _onGamePause += item.Pause;
                _onGameResume += item.Resume;
            }

            _settingsCanvas.alpha = 0;
            _settingsCanvas.gameObject.SetActive(false);
        }

        private void Start()
        {
            CloseActiveObjects();
            _isPaused = false;
            _btnExit.onClick.AddListener(ApplicationExit);
            _btnContinue.onClick.AddListener(() => _onGameResume?.Invoke());
            if (_btnPause != null)
                _btnPause.onClick.AddListener(() => _onGamePause?.Invoke());
        }

        /// <summary>
        /// Initializes all UI
        /// </summary>
        private void UiSetUp()
        {
            _sliderMainV.minValue = _volumeMin;
            _sliderMainV.maxValue = _volumeMax;
            _sliderSfxV.minValue = _volumeMin;
            _sliderSfxV.maxValue = _volumeMax;
            _sliderMusicV.minValue = _volumeMin;
            _sliderMusicV.maxValue = _volumeMax;
            _sliderUiV.minValue = _volumeMin;
            _sliderUiV.maxValue = _volumeMax;
            
            //All volume controls
            _sliderMainV.onValueChanged.AddListener(delegate { SetMainVolume(_sliderMainV.value); });
            _sliderSfxV.onValueChanged.AddListener(delegate { SetSfxMainVolume(_sliderSfxV.value); });
            _sliderMusicV.onValueChanged.AddListener(delegate { SetMusicVolume(_sliderMusicV.value); });
            _sliderUiV.onValueChanged.AddListener(delegate { SetUiMainVolume(_sliderUiV.value); });
        }

        /// <summary>
        /// Closes all active objects in <see cref="_objectsCheck"/>
        /// </summary>
        private void CloseActiveObjects()
        {
            foreach (var item in _objectsCheck)
            {
                if (item.activeSelf)
                {
                    item.SetActive(false);
                }
            }
        }
        
        private LTDescr OpenSettingsMenu()
        {
            _isPaused = true;
            _settingsCanvas.gameObject.SetActive(true);
            var fadeTween = LeanTween.alphaCanvas(_settingsCanvas, 1, 0.5f);
            return fadeTween;
        }

        private LTDescr CloseSettingsMenu()
        {
            _isPaused = false;
            UpdatePlayerPrefsSettingsValues();
            var fadeTween = LeanTween.alphaCanvas(_settingsCanvas, 0, 0.5f).setOnComplete(
                () => { _settingsCanvas.gameObject.SetActive(false); });
            return fadeTween;
        }

        public void ApplicationExit()
        {
            UpdatePlayerPrefsSettingsValues();
            Audio.AudioManager.Instance.StopAll();
            Application.Quit();
        }
        
        /// <summary>
        /// If the user has pressed a physical pause button
        /// </summary>
        /// <param name="obj"></param>
        private void PauseActionPerformed(InputAction.CallbackContext obj)
        {
            CloseActiveObjects();
            if (!_isPaused)
                _onGamePause?.Invoke();
            else
                _onGameResume?.Invoke();
        }

        private void OnEnable()
        {
            _inputActionReferencePause.action.performed += PauseActionPerformed;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }
        private void OnDestroy()
        {
            _inputActionReferencePause.action.performed -= PauseActionPerformed;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
        }

        [ContextMenu("Clear Player Prefs")]
        private void ClearPlayerPrefs() => PlayerPrefs.DeleteAll();
        
        #region IPause
        public void Pause()
        {
            OpenSettingsMenu();
        }

        public void Resume()
        {
            CloseSettingsMenu();
        }
        #endregion

        #region Value Setters
        private void SetMainVolume(float volume)
        {
            _audioMixer.SetFloat("MasterVolume", volume);
        }

        private void SetSfxMainVolume(float volume)
        {

            _audioMixer.SetFloat("SFXVolume", volume);
        }

        private void SetUiMainVolume(float volume)
        {
            _audioMixer.SetFloat("UiVolume", volume);
        }

        private void SetMusicVolume(float volume)
        {
            _audioMixer.SetFloat("MusicVolume", volume);
        }
        #endregion

        #region Scene Load/Unload
        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            //Debug.Log("New Load test");
            LoadPlayerPrefsSettingsValues();
        }

        private void SceneManager_sceneUnloaded(Scene arg0)
        {
            //Debug.Log("New Unload test");
            UpdatePlayerPrefsSettingsValues();
        }
        #endregion

        #region Player Prefs Control
        private void UpdatePlayerPrefsSettingsValues()
        {
            //All volume values
            Constants.PlayerPrefsKeys.SetPlayerPrefVolumeMain(_sliderMainV.value);
            Constants.PlayerPrefsKeys.SetPlayerPrefVolumeSfx(_sliderSfxV.value);
            Constants.PlayerPrefsKeys.SetPlayerPrefVolumeMusic(_sliderMusicV.value);
            Constants.PlayerPrefsKeys.SetPlayerPrefVolumeUi(_sliderUiV.value);
        }

        private void LoadPlayerPrefsSettingsValues()
        {
            //All volume values
            _sliderMainV.value = Constants.PlayerPrefsKeys.GetPlayerPrefVolumeMain();
            SetMainVolume(_sliderMainV.value);
            _sliderSfxV.value = Constants.PlayerPrefsKeys.GetPlayerPrefVolumeSfx();
            SetSfxMainVolume(_sliderSfxV.value);
            _sliderMusicV.value = Constants.PlayerPrefsKeys.GetPlayerPrefVolumeMusic();
            SetMusicVolume(_sliderMusicV.value);
            _sliderUiV.value = Constants.PlayerPrefsKeys.GetPlayerPrefVolumeUi();
            SetUiMainVolume(_sliderUiV.value);
        }
        #endregion
    }
}
