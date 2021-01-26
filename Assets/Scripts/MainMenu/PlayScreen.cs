using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Connexion.Miscellaneous
{
    public class PlayScreen : MenuScreen
    {
        public Button playButton;
        public ToggleGroup modeOptions;
        public ToggleGroup tabGroup;
        private Toggle[] _options;
        public GameObject messageBox;

        protected override void Awake()
        {
            base.Awake();
            _options = GetComponentsInChildren<Toggle>();
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            playButton.onClick.AddListener(StartGame);
            _options[(int)Persistent.Instance.gameManager.playMode].isOn = true;
            FadeAlphaTo(1f, 0.25f);
        }

        public void PopupMessage(string message)
        {
            if (!messageBox.activeInHierarchy)
                messageBox.SetActive(true);
            messageBox.GetComponentInChildren<Text>().text = message;
            messageBox.GetComponent<Animator>().SetTrigger("Activate");
        }

        private void StartGame()
        {
            if (_options[1].isOn)
            {
                PopupMessage("Coming soon!");
                return;
            }

            StartCoroutine(StartGameProgress());
        }

        private IEnumerator StartGameProgress()
        {
            for (int i = 0; i < _options.Length; ++i)
                if (_options[i].isOn)
                {
                    Persistent.Instance.gameManager.playMode = (Utility.GameMode)i;
                    Debug.Log(i + ":" + Persistent.Instance.gameManager.playMode);
                    break;
                }

            FadeAlphaTo(0f, 1f);
            tabGroup.gameObject.GetComponent<Animator>().SetTrigger("Exit");
            Persistent.Instance.gameManager.SaveGameDataState();

            yield return new WaitForSecondsRealtime(1f);

            SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Single);
        }
    }
}
