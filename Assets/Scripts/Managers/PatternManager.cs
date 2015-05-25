using UnityEngine;
using System.Collections;

namespace Afterward {
    public class PatternManager : Singleton<PatternManager> {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Height of the crystal")]
        [Range (0, 2)]
        float _crystalHeight = 0.5f;

        [Header ("Bottleneck")]
        [SerializeField]
        [Tooltip ("Distance between the player and the pattern")]
        [Range (0, 4)]
        float _b_distanceFromPlayer = 2;
        [SerializeField]
        [Tooltip ("Distance between the crystals in a line")]
        [Range (0, 4)]
        float _b_distanceBetweenCrystals = 1;
        [SerializeField]
        [Tooltip ("Distance between the lines")]
        [Range (0, 4)]
        float _b_initialSpace = 1.5f;
        [SerializeField]
        [Tooltip ("Angle of the pattern (degrees)")]
        [Range (0, 360)]
        float _b_angle = 45;
        [SerializeField]
        [Tooltip ("Number of crystals in a line")]
        [Range (1, 50)]
        int _b_nbCrystalsByLine = 4;

        [Header ("Arc")]
        [SerializeField]
        [Tooltip ("Distance between the player and the pattern")]
        [Range (0, 4)]
        float _a_distanceFromPlayer = 2;
        [SerializeField]
        [Tooltip ("Distance between the crystals in the arc")]
        [Range (0, 4)]
        float _a_distanceBetweenCrystals = 0.4f;
        [SerializeField]
        [Range (0, 10)]
        float _a_radius = 4;
        [SerializeField]
        [Tooltip ("Number of crystals in a line (must be odd)")]
        [Range (1, 49)]
        int _a_nbCrystals = 7;

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Crystal casted by the player")]
        GameObject _crystalPrefab;
        #endregion

        #region Getters
        /*public int cost {
            get { return _nbCrystalsByLine * 2; }
        }*/
        #endregion

        #region API
        public int Cost (string name) {
            switch (name) {
                case "bottleneck":
                    return _b_nbCrystalsByLine * 2;
                case "arc":
                    return _a_nbCrystals;
            }
            return 0;
        }

        public void GeneratePattern (string name) {
            switch (name) {
                case "bottleneck":
                    GenerateBottleNeck ();
                    break;
                case "arc":
                    GenerateArc ();
                    break;
            }
        }
        #endregion

        #region Private methods
        void GenerateArc () {
            Player p = GameManager.instance.player;
            if (0 == _a_nbCrystals % 2) {   // Even number
                _a_nbCrystals++;    // Adding one to become odd
            }
            float angle = Mathf.Asin (_a_distanceBetweenCrystals / _a_radius * 2) * Mathf.Rad2Deg;
            GameObject currentCrystal;
            float x, z;
            int factor = -1;
            for (int i = 0; i < _a_nbCrystals; ++i) {
                x = p.transform.position.x;
                z = p.transform.position.z +_a_radius;
                currentCrystal = ObjectPool.Spawn (_crystalPrefab, new Vector3 (x, _crystalHeight, z), Quaternion.identity);
                if (1 == i % 2) {
                    currentCrystal.transform.RotateAround (p.transform.position + Vector3.forward * _a_distanceFromPlayer, p.transform.up, angle * i * factor);
                }
                else {
                    currentCrystal.transform.RotateAround (p.transform.position + Vector3.forward * _a_distanceFromPlayer, p.transform.up, angle * i * factor);
                }
                currentCrystal.transform.RotateAround (p.transform.position, p.transform.up, p.transform.rotation.eulerAngles.y);
                factor *= -1;
            }
            /*Collider[] colliders = Physics.OverlapSphere (p.transform.position, 100);
            foreach (Collider collider in colliders) {
                if ("Crystal" == collider.tag) {
                    Rigidbody rb = collider.GetComponent<Rigidbody> ();
                    if (null != rb) rb.AddExplosionForce (100, p.transform.position, 100);
                }
            }*/
        }

        void GenerateBottleNeck () {
            Player p = GameManager.instance.player;
            float radianAngle = _b_angle * Mathf.Deg2Rad;
            GameObject currentCrystal;
            float x, z;
            Vector3 tmpPos = Vector3.zero;
            int factor = -1;    // Used to switch between the lines
            for (int i = 0; i < 2; ++i) {   // For each line
                for (int j = 0; j < _b_nbCrystalsByLine; ++j) {   // For each crystal on a line
                    if (0 == j) {   // First crystal of the line
                        x = p.transform.position.x + _b_initialSpace / 2 * factor;
                        z = p.transform.position.z + _b_distanceFromPlayer;
                    }
                    else {
                        x = tmpPos.x + Mathf.Sin (radianAngle / 2) * _b_distanceBetweenCrystals * factor;
                        z = tmpPos.z + Mathf.Cos (radianAngle / 2) * _b_distanceBetweenCrystals;
                    }
                    tmpPos = new Vector3 (x, _crystalHeight, z);
                    currentCrystal = ObjectPool.Spawn (_crystalPrefab, tmpPos, Quaternion.identity);
                    currentCrystal.transform.RotateAround (p.transform.position, p.transform.up, p.transform.rotation.eulerAngles.y);
                }
                factor *= -1;
            }
        }
        #endregion
    }
}