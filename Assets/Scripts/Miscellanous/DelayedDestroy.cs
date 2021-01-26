using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.Miscellaneous
{
    public class DelayedDestroy : MonoBehaviour
    {
        public float delayedTime;
        void Start()
        {
            Destroy(gameObject, delayedTime);
        }
    }
}