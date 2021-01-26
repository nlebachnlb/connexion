using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class SettingScreen : MenuScreen
    {
        [SerializeField] private Toggle _bgm;
        [SerializeField] private Toggle _sfx;
        public Toggle[] themes;

        private void Start()
        {
            _bgm.onValueChanged.AddListener(ToggleBGM);
            _sfx.onValueChanged.AddListener(ToggleSFX);
            _bgm.isOn = Persistent.Instance.gameManager.bgmVolume > 0f;
            _sfx.isOn = Persistent.Instance.gameManager.sfxVolume > 0f;
        }

        private void ToggleBGM(bool isOn)
        {
            Persistent.Instance.gameManager.bgmVolume = isOn ? GameManager.DEFAULT_BGM_VOL : 0f;
            Persistent.Instance.audioController.music.SyncVolume();
        }

        private void ToggleSFX(bool isOn)
        {
            Persistent.Instance.gameManager.sfxVolume = isOn ? GameManager.DEFAULT_SFX_VOL : 0f;
            Persistent.Instance.audioController.sfx.SyncVolume();
            Persistent.Instance.audioController.sfx.PlaySfx(Persistent.Instance.audioManager.buttonTapped[0]);
        }

        public void ChangeTheme(int index) 
        {
            Persistent.Instance.themeController.ChangeTheme(index);
        }
    }
}