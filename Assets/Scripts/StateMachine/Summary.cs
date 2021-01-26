using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;
using Connexion.StateMachine;

namespace Connexion.Gameplay.State
{
    public class Summary : IState
    {
        private Connexion.Utility.GameManager _manager;
        private GameplayModel _model;
        public Summary(Connexion.Utility.GameManager manager, GameplayModel model)
        {
            _manager = manager;
            _model = model;
        }

        void IState.Enter()
        {
            _manager.dots += _model.TotalScore();
            _manager.items = _model.items;
        }

        void IState.Execute()
        {

        }

        void IState.Exit()
        {

        }
    }
}