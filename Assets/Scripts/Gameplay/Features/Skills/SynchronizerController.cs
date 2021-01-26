using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;
using Connexion.Utility;

namespace Connexion.Features
{
    public class SynchronizerController : Controller<GameplayApp>
    {
        public void Trigger(Vector2Int coord0, Vector2Int coord1)
        {
            int color0 = app.model.boardModel.colorMap[coord0.x, coord0.y];
            int color1 = app.model.boardModel.colorMap[coord1.x, coord1.y];
            StartCoroutine(SyncProgress(color0, color1));
            var clip = Persistent.Instance.audioManager.special[3];
            Persistent.Instance.audioController.sfx.PlaySfx(clip);
        }

        private IEnumerator SyncProgress(int color0, int color1)
        {
            int size = (int)app.model.boardModel.size;

            for (int u = 0; u < size; ++u)
                for (int v = 0; v < size; ++v)
                    if (app.model.boardModel.colorMap[u, v] == color0)
                    {
                        app.model.boardModel.colorMap[u, v] = color1;
                        var targetColor = app.view.viewTheme.colorScheme[color1];
                        var dot = app.view.boardView.dots[u, v].GetComponent<DotView>();
                        dot.ChangeColor(targetColor);
                        dot.animator.SetTrigger("OnPath");
                        yield return new WaitForSeconds(Constants.ClearPathAnimTime);
                    }
        }
    }
}