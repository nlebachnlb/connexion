using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Connexion.Gameplay;
using Connexion.Utility;

namespace Connexion.Features
{
    public class BombDotController : Controller<GameplayApp>
    {
        private HashSet<Vector2Int> _cleared = new HashSet<Vector2Int>();
        public bool cleaning { get; private set; }

        private void Awake()
        {
            cleaning = false;
        }

        public void ChangeToBombAt(Vector2Int coordinate)
        {
            int u = coordinate.x;
            int v = coordinate.y;
            var dot = app.view.boardView.dots[u, v].GetComponent<DotView>();
            int direction = -1;
            if (dot.drill != null)
            {
                direction = dot.drill.GetComponent<Drill>().direction;
                Destroy(dot.drill);
                dot.drill = null;
            }
            dot.DestroyByAnimation();

            GameObject bomb = Instantiate(Persistent.Instance.featuresManager.bombDot);
            bomb.transform.position = app.view.boardView.visual[u, v].transform.position;
            bomb.GetComponent<DotView>().coordinate = coordinate;
            if (direction != -1) bomb.GetComponent<DotView>().BecomeDrill(direction, true);
            app.view.boardView.dots[u, v] = bomb;

            int[] hor = { 0, 1, 0, -1, -1, -1, 1, 1 };
            int[] ver = { 1, 0, -1, 0, -1, 1, -1, 1 };

            for (int k = 0; k < 8; ++k)
            {
                u = coordinate.x + ver[k];
                v = coordinate.y + hor[k];

                int size = (int)app.model.boardModel.size;
                if (0 <= u && u < size && 0 <= v && v < size)
                    _cleared.Add(new Vector2Int(u, v));
            }
            _cleared.Add(coordinate);
        }

        public void TriggerBomb()
        {
            StartCoroutine(AnimateTrigger());
        }

        private IEnumerator AnimateTrigger()
        {
            cleaning = true;
            foreach (var coord in _cleared)
            {
                int u = coord.x;
                int v = coord.y;
                var dot = app.view.boardView.dots[u, v].GetComponent<DotView>();
                dot.ChangeColor(app.view.viewTheme.colorScheme[5]);
            }
            yield return new WaitForSeconds(0.5f);
            app.controller.boardController.CompletePath(Enumerable.ToArray(_cleared));

            // app.controller.ScoresUpdate(achievedScores);

            _cleared.Clear();
            cleaning = false;
        }
    }
}