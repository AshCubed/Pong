using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Pong
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _vCam;
        private CinemachineBasicMultiChannelPerlin _noisePerlin;

        private float _shakeTime;
        private bool _isShaking = false;
        private float _shakeTimeElapsed = 0f;

        private void Awake()
        {
            _noisePerlin = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void Shake(float hitAmplitudeGain, float hitFrequencyGain, float shakeTime)
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
    }
}
