using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Gameplay;
using Connexion.Miscellaneous;

namespace Connexion.Features
{
    public class Drill : Entity<GameplayApp>
    {
        public int direction { get; set; }
        public float timeInterval;
        public bool catched { get; set; }
        public bool isCrossDrill;

        private GameObject _light;

        private Rotator _rotator;

        private void Awake()
        {
            direction = 0;
            catched = false;
            _rotator = GetComponent<Rotator>();
        }
        private void Start()
        {
            _rotator.RotateTo(transform.rotation.eulerAngles.z, direction * 90f, 0.1f);
            StartCoroutine(ChangeDirection(timeInterval));

            var sprCols = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprCols.Length; ++i)
                sprCols[i].color = sprCols[0].color;
        }

        public void UpdateColor()
        {
            var sprCols = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprCols.Length; ++i)
                sprCols[i].color = sprCols[0].color;
        }

        private IEnumerator ChangeDirection(float timeInterval)
        {
            float time = 0f;

            while (true)
            {
                // bool condition = app.controller.pauseState == 0;
                bool condition = true;
                condition = condition && !catched;

                time += condition ? Time.deltaTime : 0f;
                // time += Time.deltaTime;

                if (time > timeInterval)
                {
                    time = 0f;
                    _rotator.RotateTo(transform.rotation.eulerAngles.z, transform.rotation.eulerAngles.z + 90f, 0.1f);
                    direction = 1 - direction;
                    Debug.Log(transform.rotation.eulerAngles.z);
                }

                yield return null;
            }
        }

        public void Trigger()
        {
            Persistent.Instance.audioController.sfx.PlaySfx(Persistent.Instance.audioManager.special[2], Persistent.Instance.gameManager.sfxVolume / 2f);
            var rot = new Vector3(0f, 0f, (1 - direction) * 90f);

            var fx = Persistent.Instance.vfxManager.drillBurst;
            if (isCrossDrill) fx = Persistent.Instance.vfxManager.crossDrillBurst;
            GameObject go = Instantiate(fx, transform.position, transform.rotation);
            go.GetComponent<ParticleStartColorChanger>().whichColor = GetComponent<SpriteRenderer>().color;
            var rotation = go.transform.rotation;
            rotation.eulerAngles = rot;
            go.transform.rotation = rotation;
        }
    }
}