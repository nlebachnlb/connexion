using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Connexion.Gameplay;

namespace Connexion.Utility
{
    public class InputHandler : MonoBehaviour
    {
        public InputState state { get { return _state; } }
        private InputState _state = InputState.Drop;
        public UnityAction onPressBack;

        private Vector3 _startMousePos;

        public GameObject GetClickedObject()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Dots"));

            if (hit.collider == null) return null;
            return hit.collider.gameObject;
        }

        public GameObject GetTapped()
        {
            if (Input.GetMouseButtonDown(0)) return GetClickedObject();
            return null;
        }

        private void StateTransition()
        {
            if (Input.GetMouseButtonDown(0) && _state == InputState.Drop)
            {
                _startMousePos = Input.mousePosition;
                _state = InputState.Start;
            }

            if (_state == InputState.Start)
            {
                if (Input.mousePosition != _startMousePos)
                {
                    _state = InputState.Drag;
                }
            }

            if (Input.GetMouseButtonUp(0))
                _state = InputState.Drop;
        }

        private void Update()
        {
            StateTransition();

            if (Input.GetKeyDown(KeyCode.Escape))
                onPressBack();
        }
    }
}