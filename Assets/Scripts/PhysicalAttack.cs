using UnityEngine;
using System.Collections;

namespace Afterward {
    public class PhysicalAttack : MonoBehaviour {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Damage inflicted by the attack")]
        [Range (0, 50)]
        int _damage = 10;
        [SerializeField]
        [Tooltip ("Strength of the attack (physic)")]
        [Range (0, 1000)]
        float _strength = 300;
        #endregion

        #region Unity
        /*void OnEnable () {
            _done = false;
        }*/

        void OnTriggerEnter (Collider collider) {
            //if (_done) return;
            switch (collider.tag) {
                case "Crystal":
                    collider.GetComponent<Crystal> ().TakeDamage (_damage);
                    //collider.Recycle ();
                    //GameManager.instance.player.energy++;
                    break;
                case "Enemy":
                    collider.GetComponent<Rigidbody> ().AddForce (GameManager.instance.player.transform.forward * _strength);
                    collider.GetComponent<Enemy> ().TakeDamage (_damage);
                    break;
            }
            //_done = true;
        }
        #endregion

        #region Private properties
        //bool _done = false;
        #endregion
    }
}