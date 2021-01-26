using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Utility;

namespace Connexion.Gameplay
{
    public class GameplayModel : Model<GameplayApp>
    {
        public int moves { get; private set; }
        public float counterValue { get; set; }
        public float startCounterValue { get; private set; }
        public int[] scores { get; private set; } = new int[5];
        public int[] items { get; private set; } = new int[5];
        public GameMode mode;
        public MoveMode moveMode;
        public BoardModel boardModel;

        private void Awake()
        {
            boardModel = GetComponentInChildren<BoardModel>();
            Debug.Log("BoardModel" + boardModel.gameObject.name);
        }

        private void Start()
        {
            mode = Persistent.Instance.gameManager.playMode;
        }

        public void InitData(bool withBooster = false)
        {
            for (int i = 0; i < 5; ++i) scores[i] = 0;
            for (int i = 0; i < Constants.numberOfItems; ++i)
                items[i] = Persistent.Instance.gameManager.items[i];
            moves = 0;
            counterValue = mode == GameMode.Moves ? Constants.DefaultMoves : Constants.DefaultTime;
            counterValue *= withBooster ? 2 : 1;
            startCounterValue = counterValue;
        }

        public void ScoreUpdate(int index, int delta)
        {
            scores[index] += delta;
        }

        public void MovesUpdate(int delta)
        {
            moves += delta;
        }

        public int TotalScore()
        {
            int result = 0;
            foreach (var x in scores) result += x;
            return result;
        }
    }
}
