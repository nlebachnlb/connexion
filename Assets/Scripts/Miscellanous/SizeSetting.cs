using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Gameplay;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class SizeSetting : Entity<GameplayApp>
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _playWithBoosterButton;
        [SerializeField] private GameObject _messageBox;
        private bool _withBooster = false;
        public Text boostersDisplay;

        private int ParseOption(BoardSize size)
        {
            if (size == BoardSize.Small) return 0;
            if (size == BoardSize.Medium) return 1;
            if (size == BoardSize.Large) return 2;
            return -1;
        }

        private void Start() 
        {
            app.view.gameUIView.SetInteractability(false);    
            _playButton.onClick.AddListener(StartGame);
            _playWithBoosterButton.onClick.AddListener(StartGameWithBooster);
            boostersDisplay.text = Persistent.Instance.gameManager.items[4].ToString();

            Dropdown dropdown = GetComponentInChildren<Dropdown>();
            dropdown.value = ParseOption(Persistent.Instance.gameManager.currentSize);

            if (Persistent.Instance.gameManager.items[4] <= 0)
                _playWithBoosterButton.interactable = false;
        }

        private void StartGame()
        {
            var audioManager = Persistent.Instance.audioManager;
            var sfxController = Persistent.Instance.audioController.sfx;
            sfxController.PlaySfx(audioManager.buttonTapped[0], Persistent.Instance.gameManager.sfxVolume);

            Dropdown dropdown = GetComponentInChildren<Dropdown>();
            var option = dropdown.value;
            BoardSize size = BoardSize.Small;

            _playButton.interactable = false;
            dropdown.interactable = false;

            switch(option)
            {
                case 0: size = BoardSize.Small; break;
                case 1: size = BoardSize.Medium; break;
                case 2: size = BoardSize.Large; break;
            }

            app.model.boardModel.size = size;
            Persistent.Instance.gameManager.currentSize = app.model.boardModel.size;
            GetComponent<Animator>().SetTrigger("Exit");

            StartCoroutine(EnterGame());
        }

        private void StartGameWithBooster()
        {
            _withBooster = true;
            Persistent.Instance.gameManager.items[4] -= 1;
            StartGame();
        }

        private IEnumerator EnterGame()
        {
            yield return new WaitForSecondsRealtime(1f);
            app.controller.PopupMessage("Ready!!!");

            if (_withBooster)
            {
                yield return new WaitForSecondsRealtime(2f);
                string msg = "Boost " + (app.model.mode == GameMode.Moves ? "Moves" : "Time") + " x 2";
                app.controller.PopupMessage(msg);
            }

            app.view.gameUIView.BuildInHUD();

            yield return new WaitForSecondsRealtime(2f);
            app.view.gameUIView.SetInteractability(true);
            app.controller.StartGame(_withBooster);
            Destroy(gameObject);
        }
    }
}