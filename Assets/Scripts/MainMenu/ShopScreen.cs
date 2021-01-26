using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Connexion.Miscellaneous
{
    public class ShopScreen : MenuScreen
    {
        public Text[] itemsText;
        public Button[] itemsBuyButton;
        public int[] itemsPrice;

        private void Start()
        {
            for (int i = 0; i < itemsText.Length; ++i)
                itemsText[i].text = Persistent.Instance.gameManager.items[i].ToString();

            itemsPrice = new int[itemsBuyButton.Length];
            for (int i = 0; i < itemsBuyButton.Length; ++i)
            {
                itemsPrice[i] = itemsBuyButton[i].GetComponent<PriceButton>().price;
            }
        }

        public void UpdateItems(int delta, int index)
        {
            Persistent.Instance.gameManager.items[index] += delta;
            itemsText[index].text = Persistent.Instance.gameManager.items[index].ToString();

        }

        public void UpdateButtons()
        {
            for (int i = 0; i < itemsBuyButton.Length; ++i)
                if (Persistent.Instance.gameManager.dots < itemsPrice[i])
                    itemsBuyButton[i].interactable = false;
        }
    }
}