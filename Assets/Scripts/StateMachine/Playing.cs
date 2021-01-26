using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;
using Connexion.Utility;
using Connexion.StateMachine;

namespace Connexion.Gameplay.State
{
    public class Playing : IState
    {
        private GameplayController _controller;
        private BoardView _view;
        private GameplayUIView _ui;
        public Playing(GameplayController controller, BoardView view, GameplayUIView ui)
        {
            _controller = controller;
            _view = view;
            _ui = ui;
        }

        void IState.Enter()
        {
            Debug.Log("Enter Playing state");
            if (_controller.tutorialController == null)
                _ui.ResetItems();
        }
        void IState.Execute()
        {
            _view.CatchDots();

            if (_controller.tutorialController != null) return;

            if (_ui.GetSelectedItem() != -1)
                _controller.SwitchState(GameState.UsingItems);

            if (Input.GetKeyDown(KeyCode.Space))
                _view.dots[Random.Range(0, 5), Random.Range(0, 5)].GetComponent<DotView>().BecomeDrill(0, false, true);
        }
        void IState.Exit()
        {
            Debug.Log("Exit Playing state");
        }
    }
}
