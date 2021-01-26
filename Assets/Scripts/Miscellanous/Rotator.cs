using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.Miscellaneous
{
    public class Rotator : MonoBehaviour
    {
        public AnimationCurve animationCurve;

        private IEnumerator Rotate(float m_origin, float m_dest, float duration)
        {
            float elapsed = 0f;

            while (true)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / duration);

                float curvePercent = animationCurve.Evaluate(percent);
                var dest = Mathf.LerpUnclamped(m_origin, m_dest, curvePercent);

                Quaternion rotation = transform.rotation;
                rotation.eulerAngles = new Vector3(0f, 0f, dest);
                transform.rotation = rotation;

                yield return null;
            }
        }

        public void RotateTo(float m_origin, float m_dest, float duration)
        {
            StartCoroutine(Rotate(m_origin, m_dest, duration));
        }
    }
}