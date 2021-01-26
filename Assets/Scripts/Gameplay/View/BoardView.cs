using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Utility;

namespace Connexion.Gameplay
{
    public class BoardView : View<GameplayApp>
    {
        // visual is a board 
        private Transform[,] _visual;
        private BoardModel _model;
        private bool hasCycle = false;

        public Transform[,] visual { get { return _visual; } }
        public bool interactable { get; private set; }
        public CellState[,] state;
        [HideInInspector] public GameObject[,] dots;

        [SerializeField] private GameObject _dotPrefab;
        [SerializeField] private GameObject _cellPrefab;

        [HideInInspector] public Theme theme;
        public LineRenderer line;

        public void Initialize()
        {
            interactable = true;
            int size = (int)app.model.boardModel.size;

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    GameObject go = Instantiate(_cellPrefab, transform);
                    go.transform.localPosition = new Vector2(j * Constants.DELTA, -i * Constants.DELTA);
                }
            }

            //Centralize the board
            float visualSize = (size - 1) * Constants.DELTA;
            Vector2 delta = new Vector2(visualSize / 2f, (-1) * visualSize / 2f);
            transform.position = (Vector2)transform.position - delta;
        }

        // Init board view
        public void OnStartGame()
        {
            // Get the current theme
            theme = GameObject.FindObjectOfType<Theme>();

            // Get the board model from model
            _model = app.model.boardModel;

            // Init visual board size
            var size = (int)_model.size;
            _visual = new Transform[size, size];
            state = new CellState[size, size];
            dots = new GameObject[size, size];

            Initialize();

            // Debug log
            Debug.Log(_model.size);
            Debug.Log((int)_model.size);

            var cell = GetComponentsInChildren<Transform>();
            int u, v;
            u = v = 0;

            for (int i = 1; i < cell.Length; ++i)
            {
                _visual[u, v] = cell[i];
                state[u, v] = CellState.Empty;
                dots[u, v] = null;

                //Debug log
                Debug.Log(_visual[u, v].gameObject.name);

                ++v;

                if (v == size)
                {
                    v = 0;
                    ++u;
                }
            }
        }

        ///<summary>
        /// Create a visual dot at position (u, v) on grid (board)
        ///</summary>
        private void CreateDotAt(int u, int v, Color color)
        {
            dots[u, v] = Instantiate(_dotPrefab);
            var start = _visual[u, v].transform.position;
            start.y += Constants.DELTA * (int)app.model.boardModel.size;
            dots[u, v].transform.position = start;
            dots[u, v].GetComponent<DotView>().MoveTo(_visual[u, v].transform.position);
            dots[u, v].GetComponent<DotView>().color = color;
            dots[u, v].GetComponent<DotView>().coordinate = new Vector2Int(u, v);
            state[u, v] = CellState.Filled;
        }

        ///<summary>
        /// Update behaviour of board view (controlled by board controller)
        ///</summary>
        public void ViewUpdate()
        {
            var size = (int)app.model.boardModel.size;

            for (var u = 0; u < size; ++u)
            {
                for (var v = 0; v < size; ++v)
                {
                    if (state[u, v] == CellState.Empty)
                    {
                        var theme = app.view.viewTheme;
                        var color = theme.colorScheme[_model.colorMap[u, v]];

                        CreateDotAt(u, v, color);
                        // Debug.Log(color);
                    }

                    dots[u, v].GetComponent<DotView>().state = DotState.Free;
                }
            }

            Debug.Log("View updated");
        }

        ///<summary>
        /// Clear the board (grid)
        ///</summary>
        public void ViewClear()
        {
            Debug.Log("VIEW CLEAR");
            int size = (int)app.model.boardModel.size;
            for (int u = 0; u < size; ++u)
                for (int v = 0; v < size; ++v)
                {
                    dots[u, v].GetComponent<DotView>().DestroyByAnimation();
                    dots[u, v] = null;
                    state[u, v] = CellState.Empty;
                }
        }

        ///<summary>
        /// Set interactability of board (grid)
        ///</summary>
        public void SetInteractability(bool enable)
        {
            interactable = enable;
            int size = (int)app.model.boardModel.size;
            for (int u = 0; u < size; ++u)
                for (int v = 0; v < size; ++v)
                    if (dots[u, v] != null)
                        dots[u, v].GetComponent<Collider2D>().enabled = enable;
        }

        ///<summary>
        /// Set idle animation for dots in UsingItems state
        ///</summary>
        public void SetIdle(bool enabled)
        {
            int size = (int)app.model.boardModel.size;
            for (int u = 0; u < size; ++u)
                for (int v = 0; v < size; ++v)
                    if (dots[u, v] != null)
                        dots[u, v].GetComponent<DotView>().animator.SetBool("Idle", enabled);
        }

        public void CatchDots()
        {
            InputHandler inputHandler = Persistent.Instance.inputHandler;
            BoardController controller = app.controller.boardController;

            if (inputHandler.state == InputState.Drag)
            {
                var m_obj = inputHandler.GetClickedObject();

                if (m_obj != null)
                {
                    var dot = m_obj.GetComponent<DotView>();

                    if (dot != null)
                    {
                        // Debug.Log("Catch: " + m_obj.name);
                        if (dot.state == DotState.Free && !hasCycle)
                            controller.AddDotToPath(dot);
                        else if (dot.state == DotState.OnPath)
                        {
                            if (controller.NotLastDot(dot))
                            {
                                if (controller.IsFirstDot(dot))
                                {
                                    if (!hasCycle)
                                    {
                                        if (controller.PathLength > 3)
                                        {
                                            controller.AddDotToPath(dot, true);
                                            hasCycle = true;
                                            Debug.Log("HasCycleOn");
                                        }
                                    }
                                }

                                if (dot.state != DotState.Cycle)
                                {
                                    Debug.Log("FirstDot");
                                    controller.PopDotFromPath(dot);
                                    hasCycle = false;
                                }
                            }
                        }
                    }
                }
            }

            if (inputHandler.state == InputState.Drop)
            {
                controller.CompletePath();
                hasCycle = false;
            }

            if (line.positionCount > 0)
            {
                var mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                line.SetPosition(line.positionCount - 1, mousePos);
            }
        }
    }
}