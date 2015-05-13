using UnityEngine;
using System.Collections;

namespace Afterward {
    public class Crystal : MonoBehaviour {
        #region Properties
        [Header ("General")]
        [SerializeField]
        [Tooltip ("Rotation speed")]
        [Range (0.0f, 500.0f)]
        float _rotationSpeed = 100;
        #endregion

        #region Unity
        void FixedUpdate () {
            transform.Rotate (transform.up * _rotationSpeed * Time.fixedDeltaTime);
        }

        void OnTriggerEnter (Collider collider) {
            switch (collider.tag) {
                case "Crystal":
                    return;
                case "Enemy":
                    GameManager.instance.enemies.Remove (collider.GetComponent<Enemy> ());
                    collider.gameObject.Recycle ();
                    break;
            }
            gameObject.Recycle ();
        }
        #endregion
    }
}