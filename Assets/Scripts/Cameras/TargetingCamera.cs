﻿using UnityEngine;
using System.Collections;

namespace Afterward {
    public class TargetingCamera : MonoBehaviour {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Speed of the camera")]
        [Range (0.0f, 100.0f)]
        float _speed = 6;
        [SerializeField]
        [Tooltip ("Height from the target")]
        [Range (0.0f, 100.0f)]
        float _height = 6;
        [SerializeField]
        [Tooltip ("Distance from the target")]
        [Range (0.0f, 100.0f)]
        float _distance = 9;
        [SerializeField]
        [Tooltip ("Target to follow")]
        Transform _target;    // Target to follow
        #endregion

        #region Unity
        void Awake () {
            _camera = GetComponent<Camera> ();
        }

        void FixedUpdate () {
            Vector3 dest = new Vector3 (_target.transform.position.x, _target.transform.position.y + _height, _target.transform.position.z - _distance);
            transform.position = Vector3.Slerp (transform.position, dest, _speed * Time.fixedDeltaTime);
        }
        #endregion

        #region Private properties
        Camera _camera;
        #endregion
    }
}