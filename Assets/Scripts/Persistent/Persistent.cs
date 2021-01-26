using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Features;
using Connexion.Utility;
using Connexion.Audio;
using Connexion.Miscellaneous;

namespace Connexion
{
    public class Persistent : MonoBehaviour
    {
        #region Singleton
        public static Persistent Instance { get { return m_instance; } }
        private static Persistent m_instance;
        #endregion

        public InputHandler inputHandler { get { return m_inputHandler == null ? GetComponentInChildren<InputHandler>() : m_inputHandler; } }
        private InputHandler m_inputHandler;

        public GameManager gameManager { get { return m_gameManager == null ? GetComponentInChildren<GameManager>() : m_gameManager; } }
        private GameManager m_gameManager;

        public AudioManager audioManager { get { return m_audioManager == null ? GetComponentInChildren<AudioManager>() : m_audioManager; } }
        private AudioManager m_audioManager;

        public AudioController audioController { get { return m_audioController == null ? GetComponentInChildren<AudioController>() : m_audioController; } }
        private AudioController m_audioController;

        public TransitionHandler transitionHandler { get { return m_transitionHandler == null ? GetComponentInChildren<TransitionHandler>() : m_transitionHandler; } }
        private TransitionHandler m_transitionHandler;

        public FeaturesManager featuresManager { get { return m_featuresManager == null ? GetComponentInChildren<FeaturesManager>() : m_featuresManager; } }
        private FeaturesManager m_featuresManager;

        public VFXManager vfxManager { get { return m_vfxManager == null ? GetComponentInChildren<VFXManager>() : m_vfxManager; } }
        private VFXManager m_vfxManager;

        public ThemeController themeController { get { return m_themeController == null ? GetComponentInChildren<ThemeController>() : m_themeController; } }
        private ThemeController m_themeController;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            m_instance = this;

            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            Persistent[] dupplicates = GameObject.FindObjectsOfType<Persistent>();
            foreach (var dupplicate in dupplicates)
            {
                if (dupplicate != null && dupplicate != this)
                    Destroy(dupplicate.gameObject);
            }
        }
    }
}