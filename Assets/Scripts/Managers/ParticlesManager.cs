using UnityEngine;
using System.Collections;

namespace Afterward {
    public class ParticlesManager : Singleton<ParticlesManager> {
        #region Properties
        [SerializeField]
        [Tooltip ("Attack particles")]
        GameObject _attackParticles;
        #endregion

        #region API
        public void PlayAttackParticles (Vector3 pos) {
            //ObjectPool.Spawn (_attackParticles, pos, Quaternion.identity);
        }
        #endregion
    }
}