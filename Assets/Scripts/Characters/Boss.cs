using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DiosesModernos {
    public class Boss : Enemy {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Seconds between two eventual target switches")]
        [Range (0, 3)]
        float _targetSwitchDelay = 1;
        [SerializeField]
        [Tooltip ("Probability of target switch (%)")]
        [Range (0, 100)]
        float _targetSwitchProba = 30;
        #endregion

        #region Getters
        public override int health {
            get {
                return base.health;
            }
            set {
                base.health = value;
                if (0 == _health) {
                    GameManager.instance.score++;
                    gameObject.Recycle ();
                }
            }
        }

        public Transform target {
            get { return _target; }
            set { _target = value; }
        }
        #endregion

        #region API
        public void AddTarget (Transform target) {
            _targets.Add (target);
            if (1 == _targets.Count) {
                // First target
                _target = target;
            }
        }

        public void RemoveTarget (Transform target) {
            _targets.Remove (target);
            if (0 == _targets.Count) {
                // No more target !
                _target = transform;
            }
            else if (_target == target) {
                // The removed target was the current target
                // The boss follows a new target
                SelectRandomTarget ();
            }
        }

        public override void TakeDamage (int damage) {
            base.TakeDamage (damage);
        }
        #endregion

        #region Unity
        void FixedUpdate () {
            FollowTarget ();
        }

        void Start () {
            _target = GameManager.instance.player.transform;
        }
        #endregion

        #region Private properties
        // List of possible targets
        List<Transform> _targets = new List<Transform> ();
        // Target to follow
        Transform _target;
        #endregion

        #region Private methods
        void FollowTarget () {
            GetComponent<NavMeshAgent> ().destination = _target.position;
        }

        void SelectRandomTarget () {
            _target = _targets[Random.Range (0, _targets.Count)];
        }

        IEnumerator UpdateTarget () {
            do {
                if (_targets.Count > 1) {
                    // Evenual target switch
                    if (Random.Range (0, 100) < _targetSwitchProba) {
                        Transform oldTarget = _target;
                        do {
                            SelectRandomTarget ();
                        } while (_target == oldTarget);
                    }
                }
                yield return new WaitForSeconds (_targetSwitchDelay);
            } while (true);
        }
        #endregion
    }
}