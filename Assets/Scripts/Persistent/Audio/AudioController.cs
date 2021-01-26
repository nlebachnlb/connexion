using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;

namespace Connexion.Audio
{
    public class AudioController : MonoBehaviour
    {
        public SfxController sfx { get; private set; }
        public MusicController music { get; private set; }

        private void Awake()
        {
            sfx = GetComponentInChildren<SfxController>();
            music = GetComponentInChildren<MusicController>();
        }
    }
}