using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.Miscellaneous
{
    public class ParticleStartColorChanger : MonoBehaviour
    {
        private ParticleSystem[] _particleSystems;
        public bool childrenIncluded;
        public Color whichColor;
        public bool autoPlay;

        private void Start()
        {
            UpdateColor();
        }

        public void UpdateColor()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            var temp = childrenIncluded ? _particleSystems.Length : 0;

            for (int i = 0; i < temp; ++i)
                if (_particleSystems[i] != null)
                {
                    var main = _particleSystems[i].main;
                    main.startColor = whichColor;
                }

            if (autoPlay)
                foreach (var x in _particleSystems)
                    x.Play();
        }
    }
}