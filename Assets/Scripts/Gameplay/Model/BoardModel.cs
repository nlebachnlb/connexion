using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Utility;

namespace Connexion.Gameplay
{
    public class BoardModel : Model<GameplayApp>
    {
        public BoardSize size;
        public int[,] colorMap;

        public int moves { get; set; }

        public void Initialize() 
        {
            var size = (int) this.size;
            colorMap = new int[size, size];
        }
    }
}