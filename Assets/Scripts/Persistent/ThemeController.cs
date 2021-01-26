using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.Utility
{
    public class ThemeController : MonoBehaviour
    {
        public GameObject current { get; private set; }

        private void Start()
        {
            ChangeTheme(Persistent.Instance.gameManager.currentTheme, true);
        }

        public void ChangeTheme(int themeIndex, bool force = false)
        {
            if (!force && themeIndex == Persistent.Instance.gameManager.currentTheme) return;

            GameObject target = Persistent.Instance.gameManager.themes[themeIndex];
            if (current != null) Destroy(current);
            current = Instantiate(target);
            current.transform.SetParent(transform);

            var adapters = Resources.FindObjectsOfTypeAll(typeof(Miscellaneous.Adapter)) as Miscellaneous.Adapter[];

            foreach (var x in adapters)
                x.OnThemeChange();
            Persistent.Instance.gameManager.currentTheme = themeIndex;
        }
    }
}