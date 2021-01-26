using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;

namespace Connexion.Features
{
    public class FeaturesController : Controller<GameplayApp>
    {
        public BombDotController bombDot { get; private set; }
        public SweeperController sweeper { get; private set; }
        public SynchronizerController synchronizer { get; private set; }

        private void Awake() 
        {
            bombDot = GetComponentInChildren<BombDotController>();    
            sweeper = GetComponentInChildren<SweeperController>();    
            synchronizer = GetComponentInChildren<SynchronizerController>();    
        }
    }
}
