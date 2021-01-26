using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;

namespace Connexion.Miscellaneous
{
    public class PathAnimator : MonoBehaviour
    {
        public void StartAnimating(DotView[] dots, float delayEachdot)
        {
            StartCoroutine(Animate(dots, delayEachdot));
        }

        private IEnumerator Animate(DotView[] dots, float delayEachdot)
        {
            for (int i = 0; i < dots.Length; ++i)
                if (dots[i] != null)
                {
                    GameObject go = Instantiate(Persistent.Instance.vfxManager.pathHint);
                    go.transform.position = dots[i].transform.position;
                    go.GetComponent<ParticleStartColorChanger>().whichColor = dots[i].color;
                    yield return new WaitForSeconds(delayEachdot);
                }
        }
    }
}