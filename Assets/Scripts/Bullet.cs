using UnityEngine;
using System.Collections;

namespace DiosesModernos {
    public class Bullet : MonoBehaviour {
        #region Properties
        [Header ("General")]
        [SerializeField]
        [Tooltip ("Speed of the bullet")]
        [Range (0.0f, 100.0f)]
        float speed = 10;
        [SerializeField]
        [Tooltip ("Damage of the bullet")]
        [Range (0, 100)]
        int damage = 1;
        [SerializeField]
        [Tooltip ("Seconds before death")]
        [Range (0.0f, 10.0f)]
        float duration = 1;

        [Header ("Impact")]
        [SerializeField]
        [Tooltip ("Impact duration")]
        [Range (0.0f, 3.0f)]
        float impactDuration = 0.5f;
        [SerializeField]
        [Tooltip ("Impact particles")]
        GameObject impactPrefab;
        #endregion

        #region Getters
        public Character launcher {
            get { return _launcher; }
            set { _launcher = value; }
        }
        #endregion

        #region Unity
        void Awake () {
            Destroy (gameObject, duration);
        }

        void FixedUpdate () {
            transform.Translate (Vector3.forward * speed * TimeManager.instance.timeScale * Time.fixedDeltaTime);
        }

        void OnCollisionEnter (Collision collision) {
            GameObject impactObject = Instantiate (impactPrefab, transform.position, transform.rotation) as GameObject;
            Destroy (impactObject, impactDuration);
            // Damage applied only if it's the player who has shooted the bullet
            if (_launcher == GameManager.instance.player) {
                switch (collision.gameObject.tag) {
                    case "Boss":
                        collision.gameObject.GetComponent<Boss> ().TakeDamage (damage);
                        break;
                    case "Enemy":
                        collision.transform.parent.Recycle ();
                        break;
                    case "Player":
                        //GameManager.instance.ResetGame ();
                        break;
                }
            }
            gameObject.Recycle ();
        }
        #endregion

        #region Private properties
        // The character that has shooted the bullet
        Character _launcher;
        #endregion
    }
}