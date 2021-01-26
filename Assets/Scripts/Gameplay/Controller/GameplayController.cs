using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Features;
using Connexion.Utility;
using Connexion.Miscellaneous;
using Connexion.Gameplay.State;

namespace Connexion.Gameplay
{
    public class GameplayController : Controller<GameplayApp>
    {
        #region Controllers
        public BoardController boardController { get; private set; }
        public FeaturesController featuresController { get; private set; }
        public ItemsController itemsController { get; private set; }
        public TutorialController tutorialController { get; private set; }
        #endregion

        public StateMachine.StateMachine stateMachine;

        public MoveMode moveMode { get { return app.model.moveMode; } }
        public float counterValue { get { return app.model.counterValue; } }
        public bool isGameOver { get; set; }
        public int pauseState { get; set; } // 0. actuallyUnpaused, 1. inPauseProgress, 2. actuallyPaused

        public GameState currentState { get; private set; }

        private bool _timePaused = false;
        public bool timePaused { set { _timePaused = value; } get { return _timePaused; } }
        [SerializeField] private GameObject _messageBox;

        private void Awake()
        {
            boardController = GetComponentInChildren<BoardController>();
            featuresController = GetComponentInChildren<FeaturesController>();
            itemsController = GetComponentInChildren<ItemsController>();
            tutorialController = FindObjectOfType<TutorialController>();

            stateMachine = new StateMachine.StateMachine();
        }

        private void Start()
        {
            Persistent.Instance.inputHandler.onPressBack += OnPausePressed;
            currentState = GameState.None;
            SwitchState(GameState.Play);
            pauseState = 1;
        }

        /// <summary>
        /// Direct the state machine to switch state based on state name
        /// User just need to provide the name
        /// </summary>
        public void SwitchState(GameState stateName)
        {
            if (currentState != GameState.None && currentState == stateName) return;

            switch (stateName)
            {
                case GameState.Play:
                    stateMachine.SwitchState(new Playing(this, app.view.boardView, app.view.gameUIView));
                    break;
                case GameState.UsingItems:
                    stateMachine.SwitchState(new UsingItems(this, app.view.gameUIView));
                    break;
                case GameState.GameOver:
                    stateMachine.SwitchState(new GameOver(this, app.view));
                    break;
                case GameState.Summary:
                    stateMachine.SwitchState(new Summary(Persistent.Instance.gameManager, app.model));
                    break;
            }

            currentState = stateName;
        }

        public void StartGame(bool withBooster)
        {
            app.model.boardModel.Initialize();
            app.view.boardView.OnStartGame();
            boardController.InitializeGameplay();
            app.model.InitData(withBooster);

            if (app.model.mode == GameMode.Timed)
            {
                Debug.Log("Time mode started");
                StartCoroutine(StartCounterControlling());
            }

            isGameOver = false;
            pauseState = 0;
            app.view.gameUIView.BuildInCounter();
            app.view.gameUIView.ItemsUpdate();
        }

        private void Update()
        {
            stateMachine.Update();
        }

        public IEnumerator StartCounterControlling()
        {
            if (app.model.mode == GameMode.Timed)
            {
                float time = 0f;
                while (app.model.counterValue >= 0)
                {
                    time += _timePaused ? 0f : Time.deltaTime;

                    if (time > 1f)
                    {
                        app.model.counterValue -= 1f;
                        app.view.gameUIView.CounterViewUpdate(0.5f);
                        time = 0f;
                        // Debug.Log(app.model.counterValue);
                    }
                    else
                    {
                        yield return null;
                    }
                }

                // If it goes out there, times up
                app.model.counterValue = 0;
                OnGameOver();
            }
        }

        public void ScoresUpdate(int[] achievedScores)
        {
            // Update model
            for (int i = 0; i < 5; ++i)
                app.model.ScoreUpdate(i, achievedScores[i]);

            // Tell view to catch the updates
            app.view.gameUIView.CatchUpdates(app.model.moves, app.model.scores);
        }

        /// <summary>
        /// Event: On end of a move
        /// </summary>
        public void OnEndOfMove(int[] achievedScores)
        {
            // If this is a tutorial
            if (tutorialController != null)
            {
                tutorialController.OnNotified();
                return;
            }

            // Update model
            for (int i = 0; i < 5; ++i)
                app.model.ScoreUpdate(i, achievedScores[i]);

            app.model.MovesUpdate(1);
            if (app.model.mode == GameMode.Moves)
            {
                app.model.counterValue -= 1f;
                app.view.gameUIView.CounterViewUpdate(1f);
            }

            // Tell view to catch the updates
            app.view.gameUIView.CatchUpdates(app.model.moves, app.model.scores);

            // If Moves mode, check whether player uses all moves
            if (app.model.mode == GameMode.Moves)
            {
                if (app.model.counterValue < 0)
                {
                    app.model.counterValue = 0;
                    OnGameOver();
                    return;
                }
            }

            // If Out of Moves, re-generate the board (grid)
            if (boardController.NoMoreMoves())
            {
                StartCoroutine(HandleOutOfMoves());
            }
        }

        private IEnumerator HandleOutOfMoves()
        {
            // 1. Make sure the board (grid) can't be interactable
            app.view.boardView.SetInteractability(false);
            //    If in timed mode, make sure counter is paused
            _timePaused = true;
            // 2. Animate the text
            app.view.gameUIView.AnimateOutOfMoves();
            yield return new WaitForSecondsRealtime(2f);

            // 3. Clear board view
            Debug.Log("Out of moves");
            app.view.boardView.ViewClear();
            yield return new WaitForSecondsRealtime(1f);

            // 4. Re-generate the board model
            boardController.GenerateBoard();

            // 5. Done. The board is now interactable
            app.view.boardView.SetInteractability(true);
            //    Counter continues couting
            _timePaused = false;
        }

        public void OnGameOver()
        {
            SwitchState(GameState.GameOver);
        }

        public void PopupMessage(string message)
        {
            if (!_messageBox.activeInHierarchy)
                _messageBox.SetActive(true);
            _messageBox.GetComponentInChildren<Text>().text = message;
            _messageBox.GetComponent<Animator>().SetTrigger("Activate");
        }

        public void OnPausePressed()
        {
            // Animation is playing, commands denied
            if (pauseState == 1) return;

            // Game is unpaused
            if (pauseState == 0)
                StartCoroutine(PauseGameProgess());
            else if (pauseState == 2)
                // Map resume event with button event on UI view
                app.view.gameUIView.pauseScreen.ResumeGame();
        }

        public void Unpause() { StartCoroutine(UnpauseGameProgess()); }

        private IEnumerator PauseGameProgess()
        {
            pauseState = 1;

            // 1. Make sure the board (grid) can't be interactable
            app.view.boardView.SetInteractability(false);
            //    If in timed mode, make sure counter is paused
            _timePaused = true;

            // 2. Pan camera to right in order to hide the board (grid)
            var camera = Camera.main;
            var dest = camera.transform.position;
            Vector2 Min = camera.ViewportToWorldPoint(new Vector2(0f, 0f));
            Vector2 Max = camera.ViewportToWorldPoint(new Vector2(1f, 1f));

            dest.x -= (Max.x - Min.x);
            Camera.main.GetComponent<Mover>().MoveTo(dest, 1f);
            GameObject theme = GameObject.FindWithTag("Theme");
            theme.GetComponent<Mover>().MoveTo(dest, 1f);

            // 3. Build-out the HUD
            app.view.gameUIView.BuildOutHUD();
            app.view.gameUIView.BuildOutCounter();

            yield return new WaitForSecondsRealtime(1f);

            // 4. Now screen is empty, show the pause menu
            pauseState = 2;
            app.view.gameUIView.pauseScreen.gameObject.GetComponent<Animator>().SetTrigger("Entrance");
            app.view.gameUIView.pauseScreen.gameObject.SetActive(true);
        }

        private IEnumerator UnpauseGameProgess()
        {
            pauseState = 1;

            // 1. Make sure the board (grid) can't be interactable
            app.view.boardView.SetInteractability(false);
            //    If in timed mode, make sure counter is paused
            _timePaused = true;

            // 2. Pan camera to left in order to show the board (grid)
            var camera = Camera.main;
            var dest = camera.transform.position;
            Vector2 Min = camera.ViewportToWorldPoint(new Vector2(0f, 0f));
            Vector2 Max = camera.ViewportToWorldPoint(new Vector2(1f, 1f));

            dest.x += (Max.x - Min.x);
            Camera.main.GetComponent<Mover>().MoveTo(dest, 1f);
            GameObject.FindObjectOfType<Theme>().gameObject.GetComponent<Mover>().MoveTo(dest, 1f);

            // 3. Build-in the HUD
            app.view.gameUIView.BuildInHUD();
            app.view.gameUIView.BuildInCounter();

            yield return new WaitForSecondsRealtime(1f);

            // 4. State recovered
            pauseState = 0;
            //    Board (grid) is now interactable
            app.view.boardView.SetInteractability(true);
            //    If in timed mode, counter will continue counting
            _timePaused = false;
            app.view.gameUIView.pauseScreen.gameObject.SetActive(false);
        }
    }
}
