using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Utility;

namespace Connexion.Gameplay
{
    public class GameplayView : View<GameplayApp>
    {
        public BoardView boardView;
        public GameplayUIView gameUIView;
        [HideInInspector] public Theme viewTheme;

        private void Awake()
        {
            viewTheme = GameObject.FindObjectOfType<Theme>();
            boardView = GetComponentInChildren<BoardView>();
            gameUIView = GameObject.FindObjectOfType<GameplayUIView>();
            Debug.Log("BoardView" + boardView.gameObject.name);
            Debug.Log("Theme" + viewTheme);
        }
    }
}