using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;

namespace Connexion.Features
{
    public class BombDot : Entity<GameplayApp>
    {
        [SerializeField] private GameObject burstFx;
        private void Awake()
        {
            GetComponent<DotView>().color = app.view.viewTheme.colorScheme[5];
        }

        private void Start()
        {
            GameObject go = Instantiate(burstFx);
            go.transform.position = transform.position;

            var audioController = Persistent.Instance.audioController;
            var audioManager = Persistent.Instance.audioManager;
            var gameManager = Persistent.Instance.gameManager;

            audioController.sfx.PlaySfx(audioManager.special[0], gameManager.sfxVolume);
        }
    }
}