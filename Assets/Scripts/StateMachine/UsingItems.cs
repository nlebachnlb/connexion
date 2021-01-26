using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Connexion.Gameplay;
using Connexion.StateMachine;

namespace Connexion.Gameplay.State
{
    public class UsingItems : IState
    {
        private GameplayController _controller;
        private GameplayUIView _ui;
        private UnityAction<Vector2Int>[] _itemsAction;
        private int _numberOfTaps = 0;
        private Vector2Int _prevCoord;

        string defaultInstruction, syncInstruction;

        public UsingItems(GameplayController controller, GameplayUIView ui)
        {
            _controller = controller;
            _ui = ui;
            _itemsAction = new UnityAction<Vector2Int>[4];

            _itemsAction[0] = x => BombItem(x);
            _itemsAction[1] = x => SweeperItem(x);
            _itemsAction[2] = x => CrossShooterItem(x);
            _itemsAction[3] = x => SynchronizerItem(x);

            // Lazy access
            var view = _controller.app.view.boardView;
            view.SetIdle(true);
            defaultInstruction = "Tap on a dot to trigger item";
            syncInstruction = "Tap another dot to change color";
            _ui.ShowItemInstruction(true, defaultInstruction);
        }

        private void BombItem(Vector2Int coord)
        {
            _ui.ShowItemInstruction(true, defaultInstruction);
            _controller.featuresController.bombDot.ChangeToBombAt(coord);
            _controller.featuresController.bombDot.TriggerBomb();
            _numberOfTaps++;
            _controller.app.model.items[0]--;
            _controller.SwitchState(Utility.GameState.Play);
        }

        private void SweeperItem(Vector2Int coord)
        {
            _ui.ShowItemInstruction(true, defaultInstruction);
            _controller.featuresController.sweeper.Trigger(coord);
            _numberOfTaps++;
            _controller.app.model.items[1]--;
            _controller.SwitchState(Utility.GameState.Play);
        }

        private void CrossShooterItem(Vector2Int coord)
        {
            _ui.ShowItemInstruction(true, defaultInstruction);
            // Lazy access to view
            var view = _controller.app.view.boardView;
            int u = coord.x;
            int v = coord.y;
            var dot = view.dots[u, v].GetComponent<DotView>();
            dot.BecomeDrill(0, true, true);
            Vector2Int[] cleared = new Vector2Int[1] { coord };
            _controller.boardController.CompletePath(cleared);
            // _controller.ScoresUpdate(achievedScores);
            _numberOfTaps++;
            _controller.app.model.items[2]--;
            _controller.SwitchState(Utility.GameState.Play);
        }

        private void SynchronizerItem(Vector2Int coord)
        {
            // Lazy access to view
            var view = _controller.app.view.boardView;

            // First tap
            if (_numberOfTaps == 0)
            {
                _ui.ShowItemInstruction(true, "Choose destination dot to synchronize to");
                _numberOfTaps++;
                _prevCoord = coord;
                int u = coord.x;
                int v = coord.y;
                view.dots[u, v].GetComponent<DotView>().animator.SetTrigger("OnPath");
                return;
            }

            // Second tap
            if (_numberOfTaps == 1)
            {
                _numberOfTaps++;
                _controller.featuresController.synchronizer.Trigger(_prevCoord, coord);
                _controller.app.model.items[3]--;
                _controller.SwitchState(Utility.GameState.Play);
            }
        }

        void IState.Enter()
        {
            Debug.Log("Enter Using Items state");
        }

        void IState.Execute()
        {
            if (_ui.GetSelectedItem() == -1)
                _controller.SwitchState(Utility.GameState.Play);

            var inputHandler = Persistent.Instance.inputHandler;
            GameObject tapped = inputHandler.GetTapped();

            if (tapped != null)
            {
                var dot = tapped.GetComponent<DotView>();
                if (dot != null)
                {
                    Debug.Log("Tap at: " + dot.coordinate.x + ", " + dot.coordinate.y);
                    _itemsAction[_ui.GetSelectedItem()](dot.coordinate);
                }
            }
        }

        void IState.Exit()
        {
            _controller.app.view.boardView.SetIdle(false);
            _ui.ShowItemInstruction(false);
            _ui.ItemsUpdate();
            Debug.Log("Exit Using Items state");
        }
    }
}