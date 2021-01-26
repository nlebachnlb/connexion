using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Connexion.Utility;
using Connexion.Miscellaneous;

namespace Connexion.Gameplay
{
    public class GameplayUIView : View<GameplayApp>
    {
        private bool _backPressed = false;
        private bool _isAnimating = false;

        [SerializeField] private Text _moves;
        [SerializeField] private GameObject[] _dotSchemes = new GameObject[5];
        [SerializeField] private GameObject _counter;

        [Header("Top HUD")]
        [SerializeField] private Animator _topAnimator;
        [SerializeField] private GameObject[] topElements;
        [SerializeField] private Button _pauseButton;

        [Header("Middle HUD")]
        [SerializeField] private GameObject _outOfMoves;

        [Header("Bottom HUD")]
        [SerializeField] private Animator _bottomAnimator;
        [SerializeField] private GameObject[] bottomElements;
        [SerializeField] private Toggle[] _items;
        private Text[] _itemsText;

        [Header("Interactable (Buttons)")]
        [SerializeField] private Button[] interactableElements;

        [Header("Game over mask")]
        [SerializeField] private Button _playAgain;
        [SerializeField] private Button _backToMenu;
        [SerializeField] private GameObject _mask;
        [SerializeField] private Text _totalDotText;
        [SerializeField] private GameObject[] _scoresDisplay = new GameObject[5];

        public PauseScreen pauseScreen;

        private void Awake()
        {
            if (app.controller.tutorialController != null) return;
            _itemsText = new Text[_items.Length];
            for (int i = 0; i < _items.Length; ++i)
                _itemsText[i] = _items[i].gameObject.GetComponentInChildren<Text>();
        }

        private void Start()
        {
            // FULLY CONTROL the buttons
            _playAgain.onClick.AddListener(OnPlayAgain);
            _backToMenu.onClick.AddListener(HandlePressBack);
            _pauseButton.onClick.AddListener(app.controller.OnPausePressed);

            // Register press back event
            Persistent.Instance.inputHandler.onPressBack += HandlePressBack;
            _backPressed = false;
            _isAnimating = false;

            for (int i = 0; i < 5; ++i)
            {
                _dotSchemes[i].GetComponent<Image>().color = app.view.viewTheme.colorScheme[i];
                _scoresDisplay[i].GetComponent<Image>().color = app.view.viewTheme.colorScheme[i];
            }

            var theme = app.view.viewTheme;

            for (int i = 0; i < topElements.Length; ++i)
            {
                var img = topElements[i].GetComponent<Image>();
                var txt = topElements[i].GetComponent<Text>();
                if (img != null) img.color = theme.UIScheme;
                if (txt != null) txt.color = theme.UIScheme;
            }

            for (int i = 0; i < bottomElements.Length; ++i)
            {
                var img = bottomElements[i].GetComponent<Image>();
                var txt = bottomElements[i].GetComponent<Text>();
                if (img != null) img.color = theme.UIScheme;
                if (txt != null) txt.color = theme.UIScheme;
            }
        }

        // Get selected item, if player does not want to use item, return -1
        public int GetSelectedItem()
        {
            for (int i = 0; i < _items.Length; ++i)
                if (_items[i].isOn)
                    return i;

            return -1;
        }

        public void ResetItems()
        {
            foreach (var x in _items) x.isOn = false;
        }

        public void ShowItemInstruction(bool enabled, string instruction = "")
        {
            if (bottomElements[1].activeInHierarchy != enabled)
                bottomElements[1].SetActive(enabled);

            if (instruction != "")
                bottomElements[1].GetComponent<Text>().text = instruction;
        }

        // Catch the update of scores
        public void CatchUpdates(int moves, int[] scores)
        {
            _moves.text = "" + moves;

            for (int i = 0; i < 5; ++i)
                _dotSchemes[i].GetComponentInChildren<Text>().text = "" + scores[i];
        }

        public void ItemsUpdate()
        {
            for (int i = 0; i < Constants.numberOfItems - 1; ++i)
            {
                _itemsText[i].text = app.model.items[i].ToString();
                _items[i].interactable = true;
                if (app.model.items[i] <= 0)
                    _items[i].interactable = false;
            }
        }

        public void AnimateOutOfMoves()
        {
            _outOfMoves.SetActive(true);
            _outOfMoves.GetComponent<Animator>().SetTrigger("Entrance");
            StartCoroutine(DeactivateOutOfMoves(2f));
        }

        private void OnPlayAgain()
        {
            StartCoroutine(RestartScene());
        }

        private void HandlePressBack()
        {
            if (SceneManager.GetActiveScene().name != "Gameplay") return;

            if (app.controller.isGameOver && !_backPressed && !_isAnimating)
            {
                StartCoroutine(BackToMenu());
                _backPressed = true;
            }
        }

        public void UnregisterInput()
        {
            Persistent.Instance.inputHandler.onPressBack -= HandlePressBack;
        }

        private IEnumerator BackToMenu()
        {
            app.controller.SwitchState(GameState.Summary);
            Persistent.Instance.gameManager.SaveGameDataState();

            // Unregister event
            UnregisterInput();

            _playAgain.interactable = false;
            _mask.GetComponent<Animator>().SetTrigger("Exit");

            yield return new WaitForSecondsRealtime(2f);
            SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
        }

        private IEnumerator RestartScene()
        {
            app.controller.SwitchState(GameState.Summary);
            Persistent.Instance.gameManager.SaveGameDataState();

            // Unregister event
            UnregisterInput();

            _playAgain.interactable = false;
            _mask.GetComponent<Animator>().SetTrigger("Exit");

            yield return new WaitForSecondsRealtime(2f);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        private IEnumerator DeactivateOutOfMoves(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            _outOfMoves.SetActive(false);
        }

        private IEnumerator AnimateCounter(float duration)
        {
            float elapsed = 0f;
            Image cntImg = _counter.GetComponentsInChildren<Image>()[1];

            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / duration);
                // var limit = app.model.mode == GameMode.Timed ? Constants.DefaultTime : Constants.DefaultMoves;
                var limit = app.model.startCounterValue;
                cntImg.fillAmount = Mathf.Lerp(cntImg.fillAmount, app.controller.counterValue / limit, percent);
                yield return null;
            }
        }

        private IEnumerator AnimateDuring(float duration)
        {
            _isAnimating = true;
            Debug.Log("Start animating" + _isAnimating);
            yield return new WaitForSecondsRealtime(duration);
            _isAnimating = false;
            Debug.Log("End animating" + _isAnimating);
        }

        public void CounterViewUpdate(float duration)
        {
            StartCoroutine(AnimateCounter(duration));
        }

        public void BuildInCounter()
        {
            _counter.GetComponent<Animator>().SetTrigger("Entrance");
        }

        public void BuildOutCounter()
        {
            _counter.GetComponent<Animator>().SetTrigger("Exit");
        }

        public void BuildInHUD()
        {
            _topAnimator.SetTrigger("Entrance");
            _bottomAnimator.SetTrigger("Entrance");
        }

        public void BuildOutHUD()
        {
            _topAnimator.SetTrigger("Exit");
            _bottomAnimator.SetTrigger("Exit");
        }

        public void BuildInMask()
        {
            _mask.SetActive(true);
            _totalDotText.text = "" + app.model.TotalScore();
            StartCoroutine(AnimateDuring(2f));

            for (int i = 0; i < 5; ++i)
            {
                _scoresDisplay[i].GetComponentInChildren<Text>().text = _dotSchemes[i].GetComponentInChildren<Text>().text;
            }
        }

        public void SetInteractability(bool enabled)
        {
            for (int i = 0; i < interactableElements.Length; ++i)
                interactableElements[i].interactable = enabled;
        }
    }
}
