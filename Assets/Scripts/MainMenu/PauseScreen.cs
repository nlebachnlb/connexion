using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class PauseScreen : Entity<Gameplay.GameplayApp>
    {
        [SerializeField] private Button _resume;
        [SerializeField] private Toggle _bgm;
        [SerializeField] private Toggle _sfx;
        [SerializeField] private Button _menu;
        [SerializeField] private Button _restart;

        private void Start()
        {
            _resume.onClick.AddListener(ResumeGame);
            _bgm.onValueChanged.AddListener(ToggleBGM);
            _sfx.onValueChanged.AddListener(ToggleSFX);
            _menu.onClick.AddListener(BackToMenu);
            _restart.onClick.AddListener(Restart);

            _bgm.isOn = Persistent.Instance.gameManager.bgmVolume > 0f;
            _sfx.isOn = Persistent.Instance.gameManager.sfxVolume > 0f;
        }

        public void ResumeGame()
        {
            StartCoroutine(ResumeProgress());
        }

        private IEnumerator ResumeProgress()
        {
            GetComponent<Animator>().SetTrigger("Exit");
            app.controller.pauseState = 1;
            yield return new WaitForSecondsRealtime(1f);
            app.controller.Unpause();
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

        private void BackToMenu()
        {
            StartCoroutine(BackProgress());
        }

        private IEnumerator BackProgress()
        {
            Persistent.Instance.gameManager.OnEndGameplay();
            app.view.gameUIView.UnregisterInput();
            GetComponent<Animator>().SetTrigger("Exit");
            app.controller.pauseState = 1;
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        }

        private void Restart()
        {
            StartCoroutine(RestartProgress());
        }

        private IEnumerator RestartProgress()
        {
            Persistent.Instance.gameManager.OnEndGameplay();
            app.view.gameUIView.UnregisterInput();
            GetComponent<Animator>().SetTrigger("Exit");
            app.controller.pauseState = 1;
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }
}