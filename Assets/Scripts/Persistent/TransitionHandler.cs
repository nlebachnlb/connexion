using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Connexion.Miscellaneous;

namespace Connexion.Utility
{
    public class TransitionHandler : MonoBehaviour
    {
        public GameObject transitionMask { get; set; }
        public PathAnimator pathAnimator { get; private set; }
        [SerializeField] private GameObject _transitionMaskPrefab;
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneStart;
            pathAnimator = GetComponent<PathAnimator>();
        }

        private void OnSceneStart(Scene scene, LoadSceneMode mode)
        {
            var canvas = GameObject.FindGameObjectWithTag("Canvas");
            if (canvas == null) return;

            transitionMask = GameObject.Find("TransitionMask");
            FadeOutTransition();
        }

        public void FadeInTransition()
        {
            transitionMask.GetComponent<Image>().CrossFadeAlpha(1f, 0.75f, false);
        }

        public void FadeOutTransition()
        {
            transitionMask.GetComponent<Image>().CrossFadeAlpha(0f, 0.75f, false);
        }
    }
}