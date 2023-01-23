using System;
using Cinemachine;
using UnityEngine;

namespace Pong
{
    public enum BallHit
    {
        WALL, SCORE_TRIGGER
    }
    
    public class CameraShake : MonoBehaviour, IGameManager
    {
        [SerializeField] private CinemachineVirtualCamera _vCam;
        [SerializeField] private float _hitAmplitudeGainWall, _hitFrequencyGainWall, _shakeTimeWall;
        [SerializeField] private float _hitAmplitudeGainScore, _hitFrequencyGainScore, _shakeTimeScore;
        private CinemachineBasicMultiChannelPerlin _noisePerlin;

        private float _shakeTime;
        private bool _isShaking = false;
        private float _shakeTimeElapsed = 0f;

        private void Awake()
        {
            _noisePerlin = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Shake(float hitAmplitudeGain, float hitFrequencyGain, float shakeTime)
        {
            _shakeTime = shakeTime;
            _noisePerlin.m_AmplitudeGain = hitAmplitudeGain;
            _noisePerlin.m_FrequencyGain = hitFrequencyGain;
            _shakeTimeElapsed = 0;
            _isShaking = true;
        }

        private void StopShake()
        {
            _isShaking = false;
            _noisePerlin.m_AmplitudeGain = 0;
            _noisePerlin.m_FrequencyGain = 0;
            _shakeTimeElapsed = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_isShaking) return;
            _shakeTimeElapsed += Time.deltaTime;
            if (!(_shakeTimeElapsed > _shakeTime)) return;
            StopShake();
        }

        #region IGameManager
        public void OnInit()
        {
            
        }

        public void OnPlayerScoreChange(Paddle paddleScorer, int newPlayer1Score, int newPlayer2Score)
        {
            
        }

        public void OnBallHit(BallHit ballHit)
        {
            switch (ballHit)
            {
                case BallHit.WALL:
                    Shake(_hitAmplitudeGainWall, _hitFrequencyGainWall, _shakeTimeWall);
                    break;
                case BallHit.SCORE_TRIGGER:
                    Shake(_hitAmplitudeGainScore, _hitFrequencyGainScore, _shakeTimeScore);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ballHit), ballHit, null);
            }
        }

        public void OnGameRetry(float timeTillGameStart)
        {
            
        }

        public void OnGameEnd(Color colorP1, Color colorP2, int scoreP1, int scoreP2)
        {
            
        }

        public void OnPlayerJoined(Paddle paddle, Color color)
        {
            
        }

        public void OnPlayerLeft(Paddle paddle)
        {
            
        }
        #endregion
    }
}
