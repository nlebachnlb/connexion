using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Connexion.Miscellaneous;
using Connexion.Utility;
using Connexion.Features;

namespace Connexion.Gameplay
{
    public class BoardController : Controller<GameplayApp>
    {
        private Stack<DotView> _path;

        #region DEBUG
        public void LogBoard()
        {
            for (int i = 0; i < (int)app.model.boardModel.size; ++i)
            {
                var row = "";
                for (int j = 0; j < (int)app.model.boardModel.size; ++j)
                    row += app.model.boardModel.colorMap[i, j] + "  ";

                Debug.Log(row);
            }

        }
        #endregion


        private void Awake()
        {
            _path = new Stack<DotView>();
        }

        private void Start()
        {
            app.model.mode = Persistent.Instance.gameManager.playMode;
        }

        public void InitializeGameplay()
        {
            GenerateBoard();
        }

        public void GenerateBoard()
        {
            var model = app.model.boardModel;
            var size = (int)model.size;

            for (var i = 0; i < size; ++i)
                for (var j = 0; j < size; ++j)
                    model.colorMap[i, j] = Random.Range(0, 5);

            // If there is no moves, re-generate the board
            if (NoMoreMoves())
            {
                Debug.Log("Out of moves");
                GenerateBoard();
                return;
            }

            Debug.Log("Generate board");
            app.view.boardView.ViewUpdate();
        }

        // Check whether two coordinates are adjacent
        private bool adjacent(Vector2Int a, Vector2Int b)
        {
            int i = a.x;
            int j = a.y;

            int u = b.x;
            int v = b.y;

            Vector2Int delta = a - b;

            bool result;

            if (app.controller.moveMode == MoveMode.DiagonalIncluded)
                result = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y)) <= 1;
            else
                result = (Mathf.Abs(delta.x) + Mathf.Abs(delta.y)) <= 1;

            return result;
        }

        public int PathLength { get { return _path.Count; } }

        public void AddDotToPath(DotView dot, bool hasCycle = false)
        {
            if (dot.state == DotState.Free || (hasCycle && dot.state == DotState.OnPath))
            {
                var view = app.view.boardView;
                if (hasCycle) Debug.Log("Cycle");

                var audioController = Persistent.Instance.audioController;
                var audioManager = Persistent.Instance.audioManager;
                var gameManager = Persistent.Instance.gameManager;

                // If this is the first dot
                if (_path.Count == 0 && view.line.positionCount == 0)
                {
                    view.line.positionCount += 1;
                    view.line.SetPosition(0, dot.transform.position);
                    view.line.positionCount += 1;

                    if (hasCycle)
                    {
                        dot.SwitchState(DotState.Cycle);
                        _path.Push(dot);
                    }
                    else
                    {
                        dot.SwitchState(DotState.OnPath);
                        _path.Push(dot);
                    }

                    Color lineColor = dot.color;
                    view.line.startColor = view.line.endColor = lineColor;
                    audioController.sfx.PlaySfx(audioManager.dotSelect, gameManager.sfxVolume);
                }
                else
                {
                    var model = app.model.boardModel;
                    var top = _path.Peek();

                    int dotColor = model.colorMap[dot.coordinate.x, dot.coordinate.y];
                    int topColor = model.colorMap[top.coordinate.x, top.coordinate.y];

                    var condition = (dotColor == topColor && adjacent(dot.coordinate, top.coordinate));
                    if (condition)
                    {
                        // Play sfx with increasing pitch
                        audioController.sfx.PlaySfx(audioManager.dotSelect, gameManager.sfxVolume, 1f + ((float)_path.Count / 10f));

                        if (hasCycle)
                        {
                            audioController.sfx.PlaySfx(audioManager.closedConnection, gameManager.sfxVolume);
                            dot.SwitchState(DotState.Cycle);
                            _path.Push(dot);
                            Debug.Log("" + PathLength);
                            for (int i = 0; i < 5; ++i)
                            {
                                view.line.SetPosition(view.line.positionCount - 1, dot.transform.position);
                                view.line.positionCount += 1;
                            }
                        }
                        else
                        {
                            dot.SwitchState(DotState.OnPath);
                            _path.Push(dot);

                            for (int i = 0; i < 5; ++i)
                            {
                                view.line.SetPosition(view.line.positionCount - 1, dot.transform.position);
                                view.line.positionCount += 1;
                            }
                        }
                    }
                }
            }
        }

        public bool IsFirstDot(DotView dot)
        {
            if (_path.Count == 0) return false;
            var toArr = _path.ToArray();
            return dot == toArr.Last();
        }

        public bool NotLastDot(DotView dot)
        {
            if (_path.Count == 0) return false;
            return dot != _path.Peek();
        }

        /// <summary> 
        /// Pop a dot from path
        /// Return value:
        /// 0. fail
        /// 1. normal pop
        /// 2. pop and break cycle
        /// </summary>
        public int PopDotFromPath(DotView dot)
        {
            if (_path.Count < 1 || dot.state == DotState.Free) return 0;

            var top = _path.Peek();
            List<DotView> toList = new List<DotView>(_path.ToArray());

            var index = toList.IndexOf(dot);
            bool breakCycle = false;

            if (index != -1)
            {
                if (index - 1 == 0)
                {
                    // _path.Pop().state = DotState.Free;
                    Debug.Log(index + "" + toList.IndexOf(dot, 1));

                    breakCycle = _path.Peek() == _path.Last();
                    _path.Pop().SwitchState(breakCycle ? DotState.OnPath : DotState.Free);

                    if (breakCycle) Debug.Log("Breakcycle" + _path.Last().state);

                    var view = app.view.boardView;
                    view.line.positionCount -= 5;
                    return 1 + (breakCycle ? 1 : 0);
                }
            }

            return 0;
        }

        // Clear all dots data and reset their state
        public void ClearPathData(bool catchDrill = false)
        {
            while (_path.Count > 0)
            {
                var temp = _path.Pop();
                temp.SwitchState(DotState.Free, catchDrill);
            }
        }

        // The main method for Completing Path
        public void CompletePath()
        {
            var view = app.view.boardView;

            if (_path.Count < Constants.ValidPath)
            {
                ClearPathData();
                view.line.positionCount = 0;
                return;
            }

            var toArray = _path.ToArray();
            List<Vector2Int> coords = new List<Vector2Int>();

            foreach (var x in toArray)
            {
                coords.Add(x.coordinate);
            }

            int[] achievedScores = { 0, 0, 0, 0, 0 };
            bool hasCycle = (coords[0] == coords.Last());

            // If there is cycle, trigger bombs instead
            if (hasCycle)
            {
                coords.ForEach
                (
                    delegate (Vector2Int x)
                    {
                        app.controller.featuresController.bombDot.ChangeToBombAt(x);
                    }
                );
                app.controller.featuresController.bombDot.TriggerBomb();
            }
            else
            {
                // Long path forms a Drill dots
                // Very long path (10 dots) forms a 4-directions drill
                if (coords.Count >= 5)
                {
                    int u = coords.First().x;
                    int v = coords.First().y;

                    if (view.dots[u, v].GetComponent<DotView>().drill == null)
                    {
                        view.dots[u, v].GetComponent<DotView>().BecomeDrill(Random.Range(0, 2), false, coords.Count >= 10);
                        coords.RemoveAt(0);
                    }
                }

                CompletePath(coords.ToArray());
            }

            // Clear path storage
            ClearPathData(true);
            view.line.positionCount = 0;

            // Count number of moves
            var model = app.model.boardModel;
            model.moves += 1;

            // Call OnEndOfMove event (auto-call in CompletePathProgress)
        }

        #region Completing Path
        private HashSet<Vector2Int> CheckForDrillDots(Vector2Int[] m_cleared, out bool drillExisted)
        {
            var view = app.view.boardView;
            var model = app.model.boardModel;
            int size = (int)model.size;
            drillExisted = false;

            // Mark drill dots for the first time
            HashSet<Vector2Int> addition = new HashSet<Vector2Int>();
            bool[,] mark = new bool[size, size];
            for (int i = 0; i < size; ++i)
                for (int j = 0; j < size; ++j)
                    mark[i, j] = false;

            foreach (var pos in m_cleared)
            {
                int u = (int)pos.x;
                int v = (int)pos.y;

                if (model.colorMap[u, v] == -1) continue;

                var dot = view.dots[u, v].GetComponent<DotView>();
                if (dot.drill != null && !mark[u, v])
                {
                    int direction = dot.drill.GetComponent<Drill>().direction;
                    mark[u, v] = true;
                    if (!drillExisted) drillExisted = true;
                    dot.drill.GetComponent<Drill>().catched = true;

                    if (dot.drill.GetComponent<Drill>().isCrossDrill)
                    {
                        for (int k = 0; k < (int)model.size; ++k)
                        {
                            if (k != u) addition.Add(new Vector2Int(k, v));
                            if (k != v) addition.Add(new Vector2Int(u, k));
                        }

                        continue;
                    }

                    for (int k = 0; k < (int)model.size; ++k)
                        if (direction == 0 && k != u)
                            addition.Add(new Vector2Int(k, v));
                        else if (direction == 1 && k != v)
                            addition.Add(new Vector2Int(u, k));
                }
            }

            HashSet<Vector2Int> finalCleared = new HashSet<Vector2Int>(m_cleared);
            finalCleared.UnionWith(addition);

            // Mark other drill dots that is relevant with first-drill-dots
            while (true)
            {
                HashSet<Vector2Int> nextAddition = new HashSet<Vector2Int>();
                foreach (var pos in addition)
                {
                    int u = (int)pos.x;
                    int v = (int)pos.y;

                    if (model.colorMap[u, v] == -1) continue;

                    var dot = view.dots[u, v].GetComponent<DotView>();
                    if (dot.drill != null && !mark[u, v])
                    {
                        mark[u, v] = true;
                        int direction = dot.drill.GetComponent<Drill>().direction;
                        dot.drill.GetComponent<Drill>().catched = true;
                        dot.drill.GetComponent<Rotator>().enabled = false;

                        if (dot.drill.GetComponent<Drill>().isCrossDrill)
                        {
                            for (int k = 0; k < (int)model.size; ++k)
                            {
                                if (k != u) nextAddition.Add(new Vector2Int(k, v));
                                if (k != v) nextAddition.Add(new Vector2Int(u, k));
                            }

                            continue;
                        }

                        for (int k = 0; k < (int)model.size; ++k)
                            if (direction == 0 && k != u)
                                nextAddition.Add(new Vector2Int(k, v));
                            else if (direction == 1 && k != v)
                                nextAddition.Add(new Vector2Int(u, k));
                    }
                }

                if (nextAddition.Count == 0)
                {
                    break;
                }
                finalCleared.UnionWith(nextAddition);
                addition = nextAddition;
            }

            return finalCleared;
        }

        // Update model and view simutaneously
        private void ModelViewUpdate()
        {
            var view = app.view.boardView;
            var model = app.model.boardModel;
            int size = (int)model.size;

            // Push the above dots down
            for (int j = 0; j < size; ++j)
            {
                for (int i = size - 1; i >= 0; --i)
                {
                    if (model.colorMap[i, j] == -1)
                    {
                        int bottom = i;
                        int hang = i;
                        while (hang >= 0 && model.colorMap[hang, j] == -1) --hang;

                        if (hang != i)
                        {
                            while (hang >= 0 && bottom >= 0)
                            {
                                if (model.colorMap[hang, j] != -1)
                                {
                                    Vector2 m_pos = view.visual[bottom, j].position;
                                    view.dots[hang, j].GetComponent<DotView>().MoveTo(m_pos);

                                    view.dots[bottom, j] = view.dots[hang, j];
                                    // Remember to update the coordinate (on board) of dot
                                    view.dots[bottom, j].GetComponent<DotView>().coordinate = new Vector2Int(bottom, j);
                                    model.colorMap[bottom, j] = model.colorMap[hang, j];

                                    view.dots[hang, j] = null;
                                    model.colorMap[hang, j] = -1;

                                    bottom -= 1;
                                }

                                hang -= 1;
                            }
                        }

                        break;
                    }
                }
            }

        }
        #endregion

        ///<summary>
        /// Complete the path player drew, update the grid (board)
        ///</summary>
        public float CompletePath(Vector2Int[] m_cleared)
        {
            var view = app.view.boardView;
            var model = app.model.boardModel;
            int size = (int)model.size;
            int[] achievedScores = new int[5] { 0, 0, 0, 0, 0 };

            // Special: check for drill dots
            bool drillExisted;
            HashSet<Vector2Int> finalCleared = CheckForDrillDots(m_cleared, out drillExisted);
            float completionTime = finalCleared.Count * (Constants.ClearPathAnimTime * 2f) + (drillExisted ? 0.5f : 0f) + 0.4f;

            // 1. Clear the path
            foreach (var pos in finalCleared)
            {
                int u = (int)pos.x;
                int v = (int)pos.y;

                if (model.colorMap[u, v] == -1) continue;

                int colorScheme = model.colorMap[u, v];
                achievedScores[colorScheme] += 1;
            }

            StartCoroutine(CompletePathProgress(achievedScores, finalCleared, drillExisted));
            return completionTime;
        }

        public IEnumerator CompletePathProgress(int[] achievedScores, HashSet<Vector2Int> finalCleared, bool drillExisted)
        {
            app.controller.timePaused = true;

            var view = app.view.boardView;
            var model = app.model.boardModel;
            int size = (int)model.size;

            foreach (var pos in finalCleared)
            {
                int u = (int)pos.x;
                int v = (int)pos.y;

                if (model.colorMap[u, v] == -1) continue;

                yield return new WaitForSeconds(Constants.ClearPathAnimTime);
                var dot = view.dots[u, v].GetComponent<DotView>();
                dot.DestroyByAnimation();
                model.colorMap[u, v] = -1;
            }

            yield return new WaitForSeconds(drillExisted ? 0.5f : 0f);

            // 2. Dots above the path fall down
            ModelViewUpdate();
            LogBoard();

            // 3. Re-generate color
            for (int i = 0; i < size; ++i)
                for (int j = 0; j < size; ++j)
                    if (model.colorMap[i, j] == -1)
                    {
                        model.colorMap[i, j] = Random.Range(0, 5);
                        view.state[i, j] = CellState.Empty;
                    }

            // 4. Update the view
            StartCoroutine(UpdView(0f));
            app.controller.timePaused = false;
            app.controller.OnEndOfMove(achievedScores);
        }

        private IEnumerator UpdView(float delay)
        {
            yield return new WaitForSeconds(delay);
            app.view.boardView.ViewUpdate();
        }

        // DFS: used for checking available moves
        private Vector2Int[] DepthFirstSearch(Vector2Int source, int numberOfDirections)
        {
            //Initialize
            int[] hor = { 0, 1, 0, -1, -1, -1, 1, 1 };
            int[] ver = { 1, 0, -1, 0, -1, 1, -1, 1 };

            int u = source.x;
            int v = source.y;
            var model = app.model.boardModel;

            int size = (int)model.size;
            bool[,] visited = new bool[size, size];
            for (int i = 0; i < size; ++i)
                for (int j = 0; j < size; ++j)
                    visited[i, j] = false;

            //Depth First Search
            Stack<int> uCoord = new Stack<int>();
            Stack<int> vCoord = new Stack<int>();

            // Push source to stack
            uCoord.Push(u);
            vCoord.Push(v);
            visited[u, v] = true;

            // Start searching
            while (uCoord.Count > 0)
            {
                u = uCoord.Peek();
                v = vCoord.Peek();
                visited[u, v] = true;

                // Search on adjacent cells
                bool found = false;
                for (int k = 0; k < numberOfDirections; ++k)
                {
                    int row = u + ver[k];
                    int col = v + hor[k];

                    if (0 <= row && row < size && 0 <= col && col < size)
                    {
                        if (model.colorMap[u, v] == model.colorMap[row, col] && !visited[row, col])
                        {
                            uCoord.Push(row);
                            vCoord.Push(col);
                            found = true;
                            break;
                        }
                    }
                }

                // Backtrack
                if (!found || uCoord.Count >= Constants.ValidPath)
                {
                    // If found a valid path
                    if (uCoord.Count >= Constants.ValidPath)
                    {
                        List<Vector2Int> result = new List<Vector2Int>();
                        while (uCoord.Count > 0)
                        {
                            result.Add(new Vector2Int(uCoord.Pop(), vCoord.Pop()));
                        }

                        string temp = "";
                        foreach (var x in result) temp += "(" + x.x + ", " + x.y + ") ";
                        Debug.Log("Path: " + temp);

                        return result.ToArray();
                    }

                    uCoord.Pop();
                    vCoord.Pop();
                }
            }

            return null;
        }

        ///<summary>
        /// Check Out of Moves case
        ///</summary>
        public bool NoMoreMoves()
        {
            int size = (int)app.model.boardModel.size;
            for (int u = 0; u < size; ++u)
                for (int v = 0; v < size; ++v)
                    if (DepthFirstSearch(new Vector2Int(u, v), (int)app.controller.moveMode) != null)
                        return false;

            return true;
        }

    }
}