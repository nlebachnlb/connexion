using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.Utility
{
    public enum GameMode
    {   
        Timed = 0,
        Rhythm = 1,
        Moves = 2
    }

    public enum BoardSize
    {
        Small  = 4,
        Medium = 6,
        Large  = 8
    }

    public enum CellState
    {
        Empty,
        Filled
    }

    public enum DotState
    {
        Free, 
        OnPath,
        Cycle
    }

    public enum InputState
    {
        Start, 
        Drag, 
        Drop
    }

    public enum MoveMode
    {
        FourDirections = 4,
        DiagonalIncluded = 8
    }

    public class Constants
    {
        public static float DELTA = 0.65f;
        public static int ValidPath = 3;
        public static float DefaultTime = 60f;
        public static int DefaultMoves = 17;
        public static float ClearPathAnimTime = 0.025f;
        public static int numberOfItems = 5;
        public static int DefaultDots = 1000;
        public static int DefaultBoosters = 2;
    }

    public enum SpecialDots
    {
        Bomb = 10,
        Drill = 11
    }

    public enum GameState
    {
        None,
        Play, 
        UsingItems, 
        GameOver,
        Summary
    }
}