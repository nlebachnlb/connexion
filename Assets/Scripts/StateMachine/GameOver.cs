using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Features;
using Connexion.Gameplay;
using Connexion.StateMachine;
using Connexion.Utility;

namespace Connexion.Gameplay.State
{
    public class GameOver : IState
    {
        private GameplayController _controller;
        private GameplayView _view;

        public GameOver(GameplayController controller, GameplayView view)
        {
            _controller = controller;
            _view = view;
        }

        void IState.Enter()
        {
            Debug.Log("GAME OVER!");
            OnGameOver();
        }

        void IState.Execute()
        {

        }

        void IState.Exit()
        {

        }

        // Clear all drill dots and gain scores automatically (bonus)
        private IEnumerator LastHura()
        {
            // Lazy access to board model
            int size = (int)_controller.app.model.boardModel.size;

            yield return new WaitForSecondsRealtime(1f);

            // Lock direction of all drill dots
            for (int u = 0; u < size; ++u)
                for (int v = 0; v < size; ++v)
                {
                    var dot = _view.boardView.dots[u, v].GetComponent<DotView>();
                    if (dot == null) continue;
                    var drill = dot.drill;

                    if (drill != null)
                        drill.GetComponent<Drill>().catched = true;
                }

            yield return new WaitForSecondsRealtime(1f);

            while (true)
            {
                Vector2Int drillCoord = new Vector2Int(-1, -1);

                for (int u = 0; u < size; ++u)
                    for (int v = 0; v < size; ++v)
                        if (_view.boardView.dots[u, v].GetComponent<DotView>().drill != null)
                        {
                            drillCoord = new Vector2Int(u, v);
                            break;
                        }

                if (drillCoord.x == -1 || drillCoord.y == -1) break;

                Vector2Int[] cleared = new Vector2Int[1] { drillCoord };
                float completionTime = _controller.boardController.CompletePath(cleared);
                yield return new WaitForSecondsRealtime(completionTime);
            }

            if (_controller.tutorialController == null)
                _controller.StartCoroutine(GameOverProcess());
        }

        private void OnGameOver()
        {
            _controller.pauseState = 1;
            // 1. Make sure the board (grid) can't be interactable
            _view.boardView.SetInteractability(false);
            //    Also, UI buttons
            if (_controller.tutorialController == null)
                _view.gameUIView.SetInteractability(false);
            //    Also, make sure player can't draw line
            _controller.boardController.ClearPathData();
            _view.boardView.line.enabled = false;

            // 2. Counter exit animation
            if (_controller.tutorialController == null)
            {
                _view.gameUIView.BuildOutCounter();

                // 3. Start animation process
                _controller.PopupMessage("Game Over!!!");
            }
            _controller.StartCoroutine(LastHura());
        }

        private IEnumerator GameOverProcess()
        {
            _view.boardView.ViewClear();
            yield return new WaitForSecondsRealtime(0.5f);

            _view.gameUIView.BuildOutHUD();
            yield return new WaitForSecondsRealtime(0.5f);

            _view.gameUIView.BuildInMask();
            yield return new WaitForSecondsRealtime(2f);

            _controller.isGameOver = true;
        }
    }
}