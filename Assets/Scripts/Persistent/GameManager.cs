using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Connexion.Utility
{
    public class GameManager : MonoBehaviour
    {
        public enum PlayerTheme
        {
            NightSky = 0
        }

        public static float DEFAULT_BGM_VOL = 0.5f;
        public static float DEFAULT_SFX_VOL = 1f;
        public GameMode DEFAULT_MODE = GameMode.Timed;
        public BoardSize DEFAULT_SIZE = BoardSize.Small;

        [Header("Game Setting")]
        [Range(0, 1)] public float bgmVolume = DEFAULT_BGM_VOL;
        [Range(0, 1)] public float sfxVolume = DEFAULT_SFX_VOL;
        public GameObject[] themes;
        public int currentTheme;

        [Header("In-game Data")]
        public GameMode playMode;
        public BoardSize currentSize;
        public int[] items = new int[5];
        public int dots;

        #region Game Setting
        public void LoadGameSetting()
        {
            bgmVolume = PlayerPrefs.GetFloat("Bgm", DEFAULT_BGM_VOL);
            sfxVolume = PlayerPrefs.GetFloat("Sfx", DEFAULT_SFX_VOL);
            currentTheme = PlayerPrefs.GetInt("theme", 0);
        }

        public void SaveGameSetting()
        {
            PlayerPrefs.SetFloat("Bgm", bgmVolume);
            PlayerPrefs.SetFloat("Sfx", sfxVolume);
            PlayerPrefs.SetInt("theme", currentTheme);
        }
        #endregion 

        #region Persistent Data
        public void LoadGameDataState()
        {
            playMode = (GameMode)PlayerPrefs.GetInt("current-mode", (int)DEFAULT_MODE);
            currentSize = (BoardSize)PlayerPrefs.GetInt("current-size", (int)DEFAULT_SIZE);
            dots = PlayerPrefs.GetInt("dots", Constants.DefaultDots);

            for (int i = 0; i < Constants.numberOfItems; ++i)
                items[i] = PlayerPrefs.GetInt("item-" + i, 1);

            Debug.Log(playMode + ":" + currentSize);
        }

        public void SaveGameDataState()
        {
            PlayerPrefs.SetInt("current-mode", (int)playMode);
            PlayerPrefs.SetInt("current-size", (int)currentSize);
            PlayerPrefs.SetInt("dots", dots);

            for (int i = 0; i < Constants.numberOfItems; ++i)
                PlayerPrefs.SetInt("item-" + i, items[i]);
        }
        
        public void ResetGameData()
        {
            PlayerPrefs.SetInt("current-mode", 0);
            PlayerPrefs.SetInt("current-size", 1);
            PlayerPrefs.SetInt("dots", 1000);


            for (int i = 0; i < Constants.numberOfItems; ++i)
                PlayerPrefs.SetInt("item-" + i, 1);

            PlayerPrefs.DeleteKey("first-time");
        }
        #endregion

        private void Awake()
        {
            LoadGameSetting();
            LoadGameDataState();
        }

        #region DEBUG
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ResetGameData();
            }
        }
        #endregion

        private void Start()
        {
            // SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
            if (!PlayerPrefs.HasKey("first-time"))
                SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Single);
            else
                SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
            Persistent.Instance.inputHandler.onPressBack += HandlePressBack;
        }

        private void HandlePressBack()
        {
            var scene = SceneManager.GetActiveScene().name;

            switch (scene)
            {
                case "Menu": Application.Quit(); break;
            }
        }

        private void OnApplicationQuit()
        {
            SaveGameDataState();
            SaveGameSetting();
            PlayerPrefs.Save();
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus) return;
            SaveGameDataState();
            SaveGameSetting();
            PlayerPrefs.Save();
        }

        public void OnEndGameplay()
        {
            SaveGameDataState();
            SaveGameSetting();
        }

        public void FirstTimeCompleted()
        {
            PlayerPrefs.SetInt("first-time", 1);
        }
    }
}