using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pong
{
    public static class Constants
    {
        public static class Tags
        {
            public const string PLAYER = "Player";
            public const string BALL = "Ball";
        }

        public static class PlayerPrefsKeys
        {
            private const string GAME_QUALITY = "GameQuality";
            private const string VOLUME_MAIN = "Volume_Main";
            private const string VOLUME_SFX = "Volume_Sfx";
            private const string VOLUME_MUSIC = "Volume_Music";
            private const string VOLUME_UI = "Volume_Ui";

            public static int GetPlayerPrefGameQuality() => PlayerPrefs.GetInt(GAME_QUALITY, 1);
            public static void SetPlayerPrefGameQuality(int newValue) => PlayerPrefs.SetInt(GAME_QUALITY, newValue);

            public static float GetPlayerPrefVolumeMain() => PlayerPrefs.GetFloat(VOLUME_MAIN, 0.5f);
            public static void SetPlayerPrefVolumeMain(float newValue) => PlayerPrefs.SetFloat(VOLUME_MAIN, newValue);
            public static float GetPlayerPrefVolumeSfx() => PlayerPrefs.GetFloat(VOLUME_SFX, 0.5f);
            public static void SetPlayerPrefVolumeSfx(float newValue) => PlayerPrefs.SetFloat(VOLUME_SFX, newValue);
            public static float GetPlayerPrefVolumeMusic() => PlayerPrefs.GetFloat(VOLUME_MUSIC, 0.5f);
            public static void SetPlayerPrefVolumeMusic(float newValue) => PlayerPrefs.SetFloat(VOLUME_MUSIC, newValue);
            public static float GetPlayerPrefVolumeUi() => PlayerPrefs.GetFloat(VOLUME_UI, 0.5f);
            public static void SetPlayerPrefVolumeUi(float newValue) => PlayerPrefs.SetFloat(VOLUME_UI, newValue);
        }
    }
}
