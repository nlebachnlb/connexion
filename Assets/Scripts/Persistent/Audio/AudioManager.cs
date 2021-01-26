using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Sound Effect")]
        public AudioClip dotSelect;
        public AudioClip[] buttonTapped = new AudioClip[2];
        public AudioClip buttonBack;
        public AudioClip closedConnection;
        // public AudioClip combo;
        public AudioClip[] special;
        // public AudioClip gameOver;
    }
}
