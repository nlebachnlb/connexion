using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Gameplay;

namespace Connexion.Miscellaneous
{
    [RequireComponent(typeof(Text))]
    public class Compliment : Entity<GameplayApp>
    {
        private Text _textHolder;

        public string[] compliments;
        public int[] scoreLevel;

        private void Start()
        {
            _textHolder = GetComponent<Text>();
            int score = app.model.TotalScore();
            int index = -1;
            for (int i = 0; i < scoreLevel.Length; ++i)
                if (score <= scoreLevel[i])
                {
                    index = i;
                    break;
                }

            if (index == -1) index = scoreLevel.Length - 1;
            _textHolder.text = compliments[index];
        }
    }
}