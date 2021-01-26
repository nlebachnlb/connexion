using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.StateMachine
{
    // Interface of state: execute will be called during update method
    public interface IState
    {
        void Enter();
        void Execute();
        void Exit();
    }

    public class StateMachine
    {
        public IState currentState { get; private set; }

        public void SwitchState(IState newState)
        {
            if (currentState != null)
                currentState.Exit();

            currentState = newState;
            currentState.Enter();
        }

        public void Update()
        {
            if (currentState != null) currentState.Execute();
        }
    }
}
