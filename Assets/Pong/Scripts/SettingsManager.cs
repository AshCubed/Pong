using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Linq;

namespace Pong
{
    public interface IPause
    {
        public void Pause();
        public void Resume();
    }

    public class SettingsManager : MonoBehaviour, IPause
    {
        private static SettingsManager _instance;

        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SettingsManager>();
                }
                return _instance;
            }
        }

        [Space(4)]
        [SerializeField] private CanvasGroup _settingsCanvas;
        [SerializeField] private Button _btnMainMenu;
        [SerializeField] private Button _btnExit;
        [SerializeField] private Button _btnContinue;
        [Tooltip("On a scene by scene basis assign this variable")]
        [SerializeField] private Button _btnPause;
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Slider _sliderMainV;
        [SerializeField] private Slider _sliderSfxV;
        [SerializeField] private Slider _sliderUiV;
        [SerializeField] private Slider _sliderMusicV;
        [SerializeField] private TMP_Dropdown _qualityDropdown;
        private bool _isPaused;

        private Action _onGamePause;
        private Action _onGameResume;

        private void Awake()
        {
            UiSetUp();

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;

            //Find all MonoBehaviours which use the IPause interface and add them to the relevant actions.
            foreach (var item in FindObjectsOfType<MonoBehaviour>().OfType<IPause>())
            {
                _onGamePause += item.Pause;
                _onGameResume += item.Resume;
            }
        }

        private void Start()
        {
            _isPaused = false;
            _btnExit.onClick.AddListener(ApplicationExit);
            _btnMainMenu.onClick.AddListener(LoadMainMenu);
            _btnContinue.onClick.AddListener(() => _onGameResume?.Invoke());
            if (_btnPause != null)
                _btnPause.onClick.AddListener(() => _onGamePause?.Invoke());
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!_isPaused)
                    _onGamePause?.Invoke();
                else
                    _onGameResume?.Invoke();
            }
        }

        private void UiSetUp()
        {
            //Game Quality
            string[] qualityNames = QualitySettings.names;
            _qualityDropdown.ClearOptions();
            List<string> qualityOptions = new List<string>();
            int currentQialityIndex = 0;
            for (int i = 0; i < qualityNames.Length; i++)
            {
                string option = qualityNames[i];
                qualityOptions.Add(option);

                if (QualitySettings.GetQualityLevel() == i)
                    currentQialityIndex = i;
            }
            _qualityDropdown.AddOptions(qualityOptions);
            _qualityDropdown.value = currentQialityIndex;
            _qualityDropdown.RefreshShownValue();
            _qualityDropdown.onValueChanged.AddListener(delegate { SetQuality(_qualityDropdown.value); });
            //All volume controls
            _sliderMainV.onValueChanged.AddListener(delegate { SetMainVolume(_sliderMainV.value); });
            _sliderSfxV.onValueChanged.AddListener(delegate { SetSfxMainVolume(_sliderSfxV.value); });
            _sliderMusicV.onValueChanged.AddListener(delegate { SetMusicVolume(_sliderMusicV.value); });
            _sliderUiV.onValueChanged.AddListener(delegate { SetUiMainVolume(_sliderUiV.value); });
        }

        public void Pause()
        {
            OpenSettingsMenu();
        }

        public void Resume()
        {
            CloseSettingsMenu();
        }

        public LTDescr OpenSettingsMenu()
        {
            _isPaused = true;
            _settingsCanvas.gameObject.SetActive(true);
            LTDescr fadeTween = LeanTween.alphaCanvas(_settingsCanvas, 1, 0.5f);
            return fadeTween;
        }

        public LTDescr CloseSettingsMenu()
        {
            _isPaused = false;
            UpdatePlayerPrefsSettingsValues();
            LTDescr fadeTween = LeanTween.alphaCanvas(_settingsCanvas, 0, 0.5f).setOnComplete(() => { _settingsCanvas.gameObject.SetActive(false); });
            return fadeTween;
        }

        void LoadMainMenu()
        {
            _isPaused = false;
            LeanTween.delayedCall(CloseSettingsMenu().delay, () => SceneManager.LoadScene(Constants.SceneTags.MAIN_MENU));
            //CloseSettingsMenu();

            //fadeCamera.FadeOut(2f, () => SceneManager.LoadScene("Main Menu"));
        }

        public void ApplicationExit()
        {
            UpdatePlayerPrefsSettingsValues();
            Audio.AudioManager.Instance.StopAll();
            Application.Quit();
            Debug.Log("APPLICATION QUIT");
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
        }

        [ContextMenu("Clear Player Prefs")]
        private void ClearPlayerPrefs() => PlayerPrefs.DeleteAll();

        #region Setting Setters

        #region VolumeControls
        public void SetMainVolume(float volume)
        {
            _audioMixer.SetFloat("MasterVolume", volume);
        }

        public void SetSfxMainVolume(float volume)
        {

            _audioMixer.SetFloat("SFXVolume", volume);
        }

        public void SetUiMainVolume(float volume)
        {
            _audioMixer.SetFloat("UiVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            _audioMixer.SetFloat("MusicVolume", volume);
        }
        #endregion

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            //Debug.Log(QualitySettings.GetQualityLevel());
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
            //Game Quality
            //Debug.Log(QualitySettings.GetQualityLevel());
            Constants.PlayerPrefsKeys.SetPlayerPrefGameQuality(QualitySettings.GetQualityLevel());

            //All volume vals
            Constants.PlayerPrefsKeys.SetPlayerPrefVolumeMain(_sliderMainV.value);
            Constants.PlayerPrefsKeys.SetPlayerPrefVolumeSfx(_sliderSfxV.value);
            Constants.PlayerPrefsKeys.SetPlayerPrefVolumeMusic(_sliderMusicV.value);
            Constants.PlayerPrefsKeys.SetPlayerPrefVolumeUi(_sliderUiV.value);
        }

        private void LoadPlayerPrefsSettingsValues()
        {
            //Game Quality
            _qualityDropdown.value = Constants.PlayerPrefsKeys.GetPlayerPrefGameQuality();
            SetQuality(_qualityDropdown.value);

            //All volume vals
            _sliderMainV.value = Constants.PlayerPrefsKeys.GetPlayerPrefVolumeMain();
            SetMainVolume(_sliderMainV.value);
            _sliderSfxV.value = Constants.PlayerPrefsKeys.GetPlayerPrefVolumeSfx();
            SetSfxMainVolume(_sliderSfxV.value);
            _sliderMusicV.value = Constants.PlayerPrefsKeys.GetPlayerPrefVolumeMusic();
            SetMainVolume(_sliderMusicV.value);
            _sliderUiV.value = Constants.PlayerPrefsKeys.GetPlayerPrefVolumeUi();
            SetMainVolume(_sliderUiV.value);
        }
        #endregion
    }
}
