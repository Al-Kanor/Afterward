using UnityEngine;
using System.Collections;

namespace DiosesModernos {
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
            gameObject.Recycle ();
        }
        #endregion
    }
}