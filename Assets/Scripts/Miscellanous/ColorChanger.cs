using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class ColorChanger : MonoBehaviour
    {
        private Image _sprite;
        private Theme _theme; 
        private int _currColor = 0;
        public float _interval = 4f;

        private void Awake()
        {
            _sprite = GetComponent<Image>();
            _theme = FindObjectOfType<Theme>();
        }

        private IEnumerator Start()
        {
            float time = 0f;

            while (true)
            {
                yield return new WaitForEndOfFrame();

                time += Time.deltaTime;
                float percent = Mathf.Clamp01(time / _interval);
                _sprite.color = (Color) Color32.Lerp(_sprite.color, _theme.colorScheme[(_currColor + 1) % 5], percent);

                if (time > _interval)
                {
                    _currColor = (_currColor + 1) % 5;
                    time = 0f;
                }
            }
        }
    }
}