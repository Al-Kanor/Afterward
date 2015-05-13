using UnityEngine;
using System.Collections;
using XInputDotNetPure;

namespace Afterward {
    public class InputManager : Singleton<InputManager> {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Max time between 2 taps")]
        [Range (0, 1)]
        float _doubleTapDelay = 0.2f;

        [Header ("Joystick Vibration")]
        [SerializeField]
        [Tooltip ("Enable vibrations ? (don't work while playing)")]
        bool _vibrationEnabled = true;
        [SerializeField]
        [Tooltip ("Power of the left motor of the joystick")]
        [Range (0, 1)]
        float _leftVibrationStrength = 0.6f;
        [SerializeField]
        [Tooltip ("Power of the right motor of the joystick")]
        [Range (0, 1)]
        float _rightVibrationStrength = 0.32f;
        #endregion

        #region Getters
        public float doubleTapDelay {
            get { return _doubleTapDelay; }
        }

        public bool joystickConnected {
            get { return _joystickConnected; }
        }

        public float leftVibrationStrength {
            get { return _leftVibrationStrength; }
        }

        public float rightVibrationStrength {
            get { return _rightVibrationStrength; }
        }
        #endregion

        #region Unity
        void Awake () {
            _joystickConnected = Input.GetJoystickNames ().Length > 0 && "" != Input.GetJoystickNames ()[0];

            if (!_vibrationEnabled) {
                _leftVibrationStrength = 0;
                _rightVibrationStrength = 0;
            }
        }
        #endregion

        #region Private properties
        // Is true if a joystick is plugged, else false
        bool _joystickConnected;
        #endregion
    }
}