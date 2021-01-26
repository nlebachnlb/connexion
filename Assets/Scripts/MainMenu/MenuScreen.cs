using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.Miscellaneous
{
    public class MenuScreen : MonoBehaviour
    {
        protected virtual void Awake()
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
        }
        private IEnumerator Fade(float srcAlpha, float destAlpha, float duration)
        {
            float time = 0f;
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;

            while (time <= duration)
            {
                time += Time.deltaTime;
                float percent = Mathf.Clamp01(time / duration);
                canvasGroup.alpha = Mathf.Lerp(srcAlpha, destAlpha, percent);
                yield return null;
            }

            canvasGroup.interactable = true;
        }

        public void FadeAlphaTo(float alpha, float duration)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            StartCoroutine(Fade(canvasGroup.alpha, alpha, duration));
        }
    }
}