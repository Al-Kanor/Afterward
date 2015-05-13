using UnityEngine;
using System.Collections;

namespace Afterward {
    public class Enemy : Character {
        #region Unity
        void FixedUpdate () {
            GetComponent<NavMeshAgent> ().destination = GameManager.instance.player.transform.position;
        }
        #endregion

        #region Private properties

        #endregion
    }
}