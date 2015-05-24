using UnityEngine;
using System.Collections;

namespace Afterward {
    public class Crystal : MonoBehaviour {
        #region Properties
        [Header ("General")]
        [SerializeField]
        [Tooltip ("Health")]
        [Range (0, 50)]
        int _health = 20;
        [SerializeField]
        [Tooltip ("Rotation speed")]
        [Range (0.0f, 500.0f)]
        float _rotationSpeed = 100;

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Energy collectible")]
        GameObject _collectible;
        #endregion

        #region Getters
        public int health {
            get { return _health; }
            set {
                _health = Mathf.Max (0, value);
                if (0 == _health) Die ();
            }
        }
        #endregion

        #region API
        public void TakeDamage (int damage) {
            health -= damage;
        }
        #endregion

        #region Unity
        void FixedUpdate () {
            transform.Rotate (transform.up * _rotationSpeed * Time.fixedDeltaTime);

            /*if (Vector3.Distance (GetComponent<Rigidbody> ().velocity, Vector3.zero) > 0) {
                gameObject.Recycle ();
            }*/
        }

        /*void OnTriggerEnter (Collider collider) {
            switch (collider.tag) {
                case "Crystal":
                    return;
                case "Enemy":
                    GameManager.instance.enemies.Remove (collider.GetComponent<Enemy> ());
                    collider.gameObject.Recycle ();
                    break;
            }
            gameObject.Recycle ();
        }*/
        #endregion

        #region Private methods
        void Die () {
            ObjectPool.Instantiate (_collectible, transform.position, transform.rotation);
            gameObject.Recycle ();
        }
        #endregion
    }
}