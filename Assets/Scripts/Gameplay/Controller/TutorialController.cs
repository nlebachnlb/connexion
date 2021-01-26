using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Connexion.Gameplay
{
    public class TutorialController : Controller<GameplayApp>
    {
        public BoardController boardController;
        public Text instruction;
        private int _currentStep;
        private Coroutine _instructionProgress;
        private string[] instructions = new string[6]
        {
            "Connect the same dots",
            "Good! Now draw a longer path",
            "You can create a CYCLE to trigger bomb",
            "Nice! Try to draw as long as possible path (from 5 dots)",
            "Sweet! Now you see a Drill dot. It clears all dots on a ROW or a COLUMN",
            "Congratz! You've done tutorial, let's PLAY!"
        };

        private void Start()
        {
            _currentStep = 0;
            BoardModel bModel = app.model.boardModel;
            BoardView bView = app.view.boardView;
            bModel.size = Utility.BoardSize.Small;
            bModel.Initialize();
            bView.OnStartGame();
            OnNotified();
            Debug.Log("Tutorial");
        }

        public void OnNotified()
        {
            Debug.Log("" + _currentStep);
            BoardModel bModel = app.model.boardModel;
            BoardView bView = app.view.boardView;

            if (_instructionProgress != null) StopCoroutine(_instructionProgress);

            StartCoroutine(StepProgress(bModel, bView));
        }

        private IEnumerator InstructPath(Vector2Int[] dots, float delayEachDot)
        {
            yield return new WaitForSeconds(1f);
            DotView[] dotViews = new DotView[dots.Length];
            for (int i = 0; i < dotViews.Length; ++i)
                dotViews[i] = app.view.boardView.dots[dots[i].x, dots[i].y].GetComponent<DotView>();

            while (true)
            {
                Persistent.Instance.transitionHandler.pathAnimator.StartAnimating(dotViews, delayEachDot);
                yield return new WaitForSeconds(2.5f);
            }
        }

        private void OnDestroy()
        {
            Utility.Constants.ValidPath = 3;
        }

        private IEnumerator StepProgress(BoardModel bModel, BoardView bView)
        {
            instruction.text = instructions[_currentStep];
            app.controller.boardController.ClearPathData();
            app.view.boardView.line.positionCount = 0;
            bView.SetInteractability(false);

            yield return new WaitForSeconds(1f);
            bView.SetInteractability(true);

            // First step: connect same dots
            // Board size: small
            if (_currentStep == 0)
            {
                bModel.size = Utility.BoardSize.Small;
                bModel.colorMap = new int[4, 4]
                {
                    { 0, 0, 1, 3},
                    { 0, 1, 2, 3},
                    { 4, 2, 1, 0},
                    { 1, 4, 0, 0}
                };
                bView.ViewUpdate();
                Vector2Int[] dots = new Vector2Int[3]
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0)
                };
                _instructionProgress = StartCoroutine(InstructPath(dots, 0.125f));
                _currentStep++;
            }

            // Second step: draw longer path
            else if (_currentStep == 1)
            {
                bView.ViewClear();
                bModel.colorMap = new int[4, 4]
                {
                    { 0, 3, 0, 3},
                    { 0, 1, 2, 0},
                    { 0, 0, 1, 0},
                    { 2, 4, 2, 0}
                };

                yield return new WaitForSeconds(2f);

                bView.ViewUpdate();
                Vector2Int[] dots = new Vector2Int[4]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(2, 1)
                };
                _instructionProgress = StartCoroutine(InstructPath(dots, 0.125f));
                _currentStep++;
            }

            // Third step: Create a cycle
            else if (_currentStep == 2)
            {
                bView.ViewClear();
                bModel.colorMap = new int[4, 4]
                {
                    { 1, 3, 1, 3},
                    { 2, 0, 0, 2},
                    { 3, 0, 0, 1},
                    { 2, 4, 2, 4}
                };

                yield return new WaitForSeconds(2f);
                Utility.Constants.ValidPath = 5;
                bView.ViewUpdate();
                Vector2Int[] dots = new Vector2Int[5]
                {
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 2),
                    new Vector2Int(2, 2),
                    new Vector2Int(2, 1),
                    new Vector2Int(1, 1)
                };
                _instructionProgress = StartCoroutine(InstructPath(dots, 0.125f));
                _currentStep++;
            }

            // Fourth step: Try to draw a longer path (from 5 dots)
            else if (_currentStep == 3)
            {
                bView.ViewClear();
                bModel.colorMap = new int[4, 4]
                {
                    { 0, 3, 1, 3},
                    { 0, 2, 0, 1},
                    { 0, 1, 4, 0},
                    { 2, 0, 0, 4}
                };

                yield return new WaitForSeconds(2f);
                // Constant violence: Temporary change constant to force player to draw long path
                Utility.Constants.ValidPath = 5;
                bView.ViewUpdate();
                Vector2Int[] dots = new Vector2Int[7]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(3, 1),
                    new Vector2Int(3, 2),
                    new Vector2Int(2, 3),
                    new Vector2Int(1, 2)
                };
                _instructionProgress = StartCoroutine(InstructPath(dots, 0.1f));
                _currentStep++;
            }

            // Drill dot
            else if (_currentStep == 4)
            {
                // Reset constant
                Utility.Constants.ValidPath = 3;
                yield return new WaitForSeconds(2f);
                app.controller.SwitchState(Utility.GameState.GameOver);
                Debug.Log("clear drill");

                yield return new WaitForSeconds(5f);
                instruction.text = instructions[5];
                bView.ViewClear();
                yield return new WaitForSeconds(2f);
                Persistent.Instance.gameManager.FirstTimeCompleted();
                SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
            }
        }
    }
}