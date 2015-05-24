using UnityEngine;
using System.Collections;

namespace Afterward {
    public class PatternManager : Singleton<PatternManager> {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Distance between the player and the pattern")]
        [Range (0, 4)]
        float _distanceFromPlayer = 2;
        [SerializeField]
        [Tooltip ("Distance between the crystals in a line")]
        [Range (0, 4)]
        float _distanceBetweenCrystals = 1;
        [SerializeField]
        [Tooltip ("Distance between the lines")]
        [Range (0, 4)]
        float _initialSpace = 1.5f;
        [SerializeField]
        [Tooltip ("Angle of the pattern (degrees)")]
        [Range (0, 360)]
        float _angle = 45;
        [SerializeField]
        [Tooltip ("Number of crystals in a line")]
        [Range (1, 10)]
        int _nbCrystalsByLine = 4;
        [SerializeField]
        [Tooltip ("Height of the crystal")]
        [Range (0, 2)]
        float _crystalHeight = 0.5f;

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Crystal casted by the player")]
        GameObject _crystalPrefab;
        #endregion

        #region API
        public void GenerateBottleNeck () {
            Player p = GameManager.instance.player;
            if (p.energy < _nbCrystalsByLine * 2) return;
            float radianAngle = _angle * Mathf.Deg2Rad;
            GameObject currentCrystal;
            float x, z;
            Vector3 tmpPos = Vector3.zero;
            int factor = -1;    // Used to switch between the lines
            for (int i = 0; i < 2; ++i) {   // For each line
                for (int j = 0; j < _nbCrystalsByLine; ++j) {   // For each crystal on a line
                    if (0 == j) {   // First crystal of the line
                        x = p.transform.position.x + _initialSpace / 2 * factor;
                        z = p.transform.position.z + _distanceFromPlayer;
                    }
                    else {
                        x = tmpPos.x + Mathf.Sin (radianAngle / 2) * _distanceBetweenCrystals * factor;
                        z = tmpPos.z + Mathf.Cos (radianAngle / 2) * _distanceBetweenCrystals;
                    }
                    tmpPos = new Vector3 (x, _crystalHeight, z);
                    currentCrystal = ObjectPool.Spawn (_crystalPrefab, tmpPos, Quaternion.identity);
                    currentCrystal.transform.RotateAround (p.transform.position, p.transform.up, p.transform.rotation.eulerAngles.y);
                    p.energy--;
                }
                factor *= -1;
            }
        }
        #endregion
    }
}