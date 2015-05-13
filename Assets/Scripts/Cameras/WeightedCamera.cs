using UnityEngine;
using System.Collections;

namespace Afterward {
    public class WeightedCamera : MonoBehaviour {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Speed of the camera")]
        [Range (0.0f, 100.0f)]
        float _speed = 1;
        [SerializeField]
        [Tooltip ("Height from the targets")]
        [Range (0.0f, 100.0f)]
        float _height = 4.28f;
        [SerializeField]
        [Tooltip ("Distance from the targets")]
        [Range (0.0f, 100.0f)]
        float _distance = 4.58f;
        #endregion

        #region Unity
        void FixedUpdate () {
            UpdateTargetPosition ();
            transform.position = Vector3.Slerp (transform.position, _targetPos, _speed * Time.fixedDeltaTime);
        }

        void Start () {
            _targetPos = Vector3.zero;
            transform.position = _targetPos;
        }
        #endregion

        #region Private properties
        Vector3 _targetPos;
        #endregion

        #region Private methods
        void UpdateTargetPosition () {
            Player player = GameManager.instance.player;
            ArrayList enemies = GameManager.instance.enemies;
            int totalWeight = player.cameraWeight * enemies.Count;
            float x = player.transform.position.x * totalWeight;
            float z = player.transform.position.z * totalWeight;
            foreach (Enemy enemy in enemies) {
                x += enemy.transform.position.x * enemy.cameraWeight;
                z += enemy.transform.position.z * enemy.cameraWeight;
                totalWeight++;
            }
            x /= totalWeight;
            z /= totalWeight;
            _targetPos = new Vector3 (x, _height, z - _distance);
        }
        #endregion
    }
}