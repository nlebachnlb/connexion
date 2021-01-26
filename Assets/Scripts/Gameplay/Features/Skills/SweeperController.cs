using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Connexion.Gameplay;

namespace Connexion.Features
{
    public class SweeperController : Controller<GameplayApp>
    {
        private HashSet<Vector2Int> _cleared = new HashSet<Vector2Int>();
        private bool cleaning = false;

        public void Trigger(int color)
        {
            var model = app.model.boardModel;

            for (int i = 0; i < (int)model.size; ++i)
                for (int j = 0; j < (int)model.size; ++j)
                    if (model.colorMap[i, j] == color)
                        _cleared.Add(new Vector2Int(i, j));

            StartCoroutine(AnimateTrigger());
        }

        public void Trigger(Vector2Int coord) 
        {
            var model = app.model.boardModel;
            int u = coord.x;
            int v = coord.y;

            Trigger(model.colorMap[u, v]);
        }

        private IEnumerator AnimateTrigger()
        {
            var clip = Persistent.Instance.audioManager.special[1];
            Persistent.Instance.audioController.sfx.PlaySfx(clip);

            cleaning = true;
            foreach (var coord in _cleared)
            {
                int u = coord.x;
                int v = coord.y;
                var dot = app.view.boardView.dots[u, v].GetComponent<DotView>();
                dot.AnimateSelected();
            }
            yield return new WaitForSeconds(0.5f);
            app.controller.boardController.CompletePath(Enumerable.ToArray(_cleared));

            // app.controller.ScoresUpdate(achievedScores);

            _cleared.Clear();
            cleaning = false;
        }
    }
}