using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class UISchemeAdapter : Adapter
    {
        [Range(0, 1)]
        public float darken = 1f;

        private void Start()
        {
            OnThemeChange();
        }

        public override void OnThemeChange()
        {
            Color color = (Color)Persistent.Instance.themeController.current.GetComponent<Theme>().UIScheme;
            color.b *= darken;
            color.r *= darken;
            color.g *= darken;

            GetComponent<Image>().color = color;
        }
    }
}