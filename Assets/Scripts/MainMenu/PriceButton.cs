using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class PriceButton : MonoBehaviour
    {
        private Button _button;
        public int price;
        [Range(0, 4)]
        public int[] itemIndex;
        public int[] itemQuantity;

        public ShopScreen shop;
        public MenuController controller;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(Buy);
            _button.gameObject.GetComponentInChildren<Text>().text = price.ToString();

            if (Persistent.Instance.gameManager.dots < price)
                _button.interactable = false;
        }

        private void Buy()
        {
            for (int i = 0; i < itemIndex.Length; ++i)
                shop.UpdateItems(+itemQuantity[i], itemIndex[i]);

            controller.UpdateDotsBank(-price);
            shop.UpdateButtons();
            var clip = Persistent.Instance.audioManager.buttonTapped[1];
            Persistent.Instance.audioController.sfx.PlaySfx(clip);
        }
    }
}