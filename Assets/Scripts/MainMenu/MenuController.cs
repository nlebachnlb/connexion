using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Connexion.Miscellaneous
{
    public class MenuController : MonoBehaviour
    {
        public MenuScreen[] screens;
        public int currentScreen { get; private set; }

        public CanvasGroup tabGroup;
        public Toggle[] tabs;
        public Text dotsBank;

        private void Start()
        {
            currentScreen = 1;
            // screens[currentScreen].FadeAlphaTo(1f, 0.25f);

            for (int i = 0; i < tabs.Length; ++i)
                tabs[i].onValueChanged.AddListener(TabChange);

            UpdateDotsBank();
        }

        public void UpdateDotsBank(int delta = 0)
        {
            Persistent.Instance.gameManager.dots += delta;
            dotsBank.text = Persistent.Instance.gameManager.dots.ToString();
            // Debug.Log("1" + dotsBank == null);
        }

        private IEnumerator OnScreenChanged(int destScreen)
        {
            tabGroup.interactable = false;
            screens[currentScreen].FadeAlphaTo(0f, 0.25f);
            yield return new WaitForSecondsRealtime(0.25f);
            screens[currentScreen].gameObject.SetActive(false);

            currentScreen = destScreen;
            screens[currentScreen].gameObject.SetActive(true);
            screens[currentScreen].FadeAlphaTo(1f, 0.25f);
            yield return new WaitForSecondsRealtime(0.25f);
            tabGroup.interactable = true;
        }

        private void ChangeScreen()
        {
            int destScreen = currentScreen;

            for (int i = 0; i < tabs.Length; ++i)
                if (tabs[i].isOn)
                {
                    destScreen = i;
                    Debug.Log("Screen " + i);
                }

            if (destScreen != currentScreen)
                StartCoroutine(OnScreenChanged(destScreen));
        }

        private void TabChange(bool isOn)
        {
            ChangeScreen();
        }
    }
}