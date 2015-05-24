using UnityEngine;
using System.Collections;

namespace Afterward {
    public class Collectible : MonoBehaviour {
        #region Unity
        void OnTriggerEnter (Collider collider) {
            if ("Player" == collider.tag) {
                collider.GetComponent<Player> ().energy++;
                gameObject.Recycle ();
            }
        }
        #endregion
    }
}