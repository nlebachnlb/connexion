using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Utility;
using Connexion.Features;
using Connexion.Miscellaneous;

namespace Connexion.Gameplay
{
    public class DotView : View<GameplayApp>
    {
        private SpriteRenderer _sprite;
        private SpriteRenderer _sprSelected;
        public Animator animator { get; private set; }
        [SerializeField] private AnimationCurve animationCurve;
        public Color32 color { get; set; }
        public Vector2Int coordinate { get; set; }
        public DotState state { get; set; }
        public GameObject drill { get; set; }

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _sprSelected = GetComponentsInChildren<SpriteRenderer>()[1];
            animator = GetComponent<Animator>();
            drill = null;
        }

        private void Start()
        {
            _sprite.color = color;
            _sprSelected.color = color;
            SwitchState(DotState.Free);
        }

        public void BecomeDrill(int initialDrillDirection = 0, bool isCatched = false, bool isCrossDrill = false)
        {
            if (drill != null) return;

            var prefab = Persistent.Instance.featuresManager.dotDrill;
            if (isCrossDrill) prefab = Persistent.Instance.featuresManager.crossDrill;
            drill = Instantiate(prefab);
            drill.transform.SetParent(transform);
            drill.transform.localPosition = Vector2.zero;

            drill.GetComponent<SpriteRenderer>().color = color;

            var drillCom = drill.GetComponent<Drill>();
            drillCom.direction = initialDrillDirection;
            drillCom.catched = isCatched;

            GameObject light;
            light = Instantiate(Persistent.Instance.vfxManager.drillLight);
            light.transform.SetParent(transform);
            light.transform.localPosition = Vector2.zero;
            light.GetComponent<ParticleStartColorChanger>().whichColor = color;
        }

        #region Animation
        // Move animation
        private IEnumerator Move(Vector2 m_origin, Vector2 m_dest, float duration)
        {
            // Make sure player can not interact while animation is running
            GetComponent<Collider2D>().enabled = false;

            yield return new WaitForSeconds(0.3f);

            float elapsed = 0f;

            while (true)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / duration);

                float curvePercent = animationCurve.Evaluate(percent);
                transform.position = Vector2.LerpUnclamped(m_origin, m_dest, curvePercent);

                if (elapsed > duration)
                {
                    // Done animation, player now can interact
                    GetComponent<Collider2D>().enabled = app.view.boardView.interactable;
                    break;
                }

                yield return null;
            }
        }

        ///<summary>
        /// Move to a destination (animation)
        ///</summary>
        public void MoveTo(Vector2 m_dest)
        {
            StartCoroutine(Move(transform.position, m_dest, 0.5f));
        }

        private IEnumerator CrossFadeColor(Color32 m_origin, Color32 m_dest, float duration)
        {
            // Make sure player can not interact while animation is running
            GetComponent<Collider2D>().enabled = false;
            float elapsed = 0f;

            while (true)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / duration);
                _sprite.color = Color32.Lerp(m_origin, m_dest, percent);
                _sprSelected.color = _sprite.color;

                if (elapsed > duration)
                {
                    // Done animation, player now can interact
                    GetComponent<Collider2D>().enabled = app.view.boardView.interactable;
                    break;
                }

                yield return null;
            }
        }

        public void ChangeColor(Color32 m_dest)
        {
            color = m_dest;
            if (drill != null)
            {
                drill.GetComponent<SpriteRenderer>().color = m_dest;
                drill.GetComponent<Drill>().UpdateColor();
                var light = GetComponentInChildren<ParticleStartColorChanger>();
                light.whichColor = color;
                light.UpdateColor();
            }
                
            StartCoroutine(CrossFadeColor(GetComponent<SpriteRenderer>().color, m_dest, 0.5f));
        }
        #endregion
        public void SwitchState(DotState toState, bool catchDrill = false)
        {
            if (toState == DotState.OnPath)
                animator.SetTrigger("OnPath");

            state = toState;

            if (drill == null) return;
            var temp = drill.GetComponent<Drill>();
            if (state != DotState.Free)
                temp.catched = true;
            else if (!catchDrill)
                temp.catched = false;
        }

        public void AnimateSelected()
        {
            animator.SetTrigger("OnPath");
        }

        public void DestroyByAnimation()
        {
            GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("Clear");

            if (drill != null)
                drill.GetComponent<Drill>().Trigger();
        }

        public override void OnAnimationEnd()
        {
            Destroy(gameObject);
        }
    }
}
