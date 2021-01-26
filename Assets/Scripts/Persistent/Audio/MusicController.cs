using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;
using Connexion.Utility;

namespace Connexion.Audio
{
    public class MusicController : MonoBehaviour
    {
        public enum PlayMode
        {
            Normal,
            InstrumentMix
        }

        private AudioSource _audio;
        public PlayMode mode = PlayMode.Normal;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        public void PlayBGM()
        {
            if (_audio.isPlaying) _audio.Stop();
            if (mode == PlayMode.Normal)
            {
                _audio.loop = true;
                _audio.volume = Persistent.Instance.gameManager.bgmVolume;
                _audio.clip = GameObject.FindObjectOfType<Theme>().backgroundMusic;
                _audio.Play();
            }
        }

        public void SyncVolume() { _audio.volume = Persistent.Instance.gameManager.bgmVolume; }
    }
}