using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class ThemeCard : MonoBehaviour
    {
        public Image background;
        public Sprite thumbnail;
        public Image[] schemes;
        public Text themeName;
        public Color textColor;
        public Theme theme;
        public int themeIndex;

        private void Start()
        {
            for (int i = 0; i < schemes.Length; ++i)
            {
                schemes[i].color = (Color)theme.colorScheme[i];
            }

            themeName.color = textColor;
            themeName.text = theme.themeName;
            background.sprite = thumbnail;
            Toggle toggle = GetComponent<Toggle>();
            toggle.group = GetComponentInParent<ToggleGroup>();
            toggle.isOn = themeIndex == Persistent.Instance.gameManager.currentTheme;
        }
    }
}