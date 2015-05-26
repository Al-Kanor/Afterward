using UnityEngine;
using System.Collections;

namespace Afterward {
    public class DashingCamera : MonoBehaviour {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Camera recoil")]
        [Range (0, 180)]
        float _recoil = 86;
        [SerializeField]
        [Tooltip ("Recoil speed")]
        [Range (0, 10)]
        float _recoilSpeed = 6;
        #endregion

        #region Unity
        void Start () {
            _camera = GetComponent<Camera> ();
            _initialFOV = _camera.fieldOfView;
        }
        #endregion

        #region Private properties
        Camera _camera;
        float _initialFOV;
        #endregion

        #region Private methods
        IEnumerator StartRecoil () {
            do {
                _camera.fieldOfView = Mathf.Lerp (_camera.fieldOfView, _recoil, _recoilSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate ();
            } while (Mathf.Abs (_camera.fieldOfView - _recoil) > 0.01f);
        }

        IEnumerator StopRecoil () {
            StopCoroutine ("StartRecoil");
            do {
                _camera.fieldOfView = Mathf.Lerp (_camera.fieldOfView, _initialFOV, _recoilSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate ();
            } while (Mathf.Abs (_camera.fieldOfView - _initialFOV) > 0.01f);
        }
        #endregion
    }
}