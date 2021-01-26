using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class TextSchemeAdapter : Adapter
    {
        public enum WhichText
        {
            darkText,
            lightText
        }

        public WhichText darkOrLight;

        private void Start()
        {
            OnThemeChange();
        }

        public override void OnThemeChange()
        {
            var temp = GetComponentsInChildren<Text>();
            var scheme = Persistent.Instance.themeController.current.GetComponent<Theme>();
            foreach (var x in temp)
                x.color = darkOrLight == WhichText.darkText ? scheme.darkTextColor : scheme.lightTextColor;
        }
    }
}