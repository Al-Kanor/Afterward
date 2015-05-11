using UnityEngine;
using System.Collections;

namespace DiosesModernos {
    public class Ally : PlayableCharacter {
        #region API
        public void ClampPosition (Vector3 pos) {
            state = Vector3.Distance (pos, _lastKnownPos) < 0.01f ? State.IDLE : State.WALK;
            if (State.WALK == state) {
                transform.position = pos;
                _move = pos - _lastKnownPos;
                _move.Normalize ();
                _lastKnownPos = pos;
            }
        }

        public void InterpolatePosition (Vector3 pos) {
            _targetPos = pos;
        }
        #endregion

        #region Unity
        void Awake () {
            _lastKnownPos = transform.position;
        }

        void FixedUpdate () {
            //ExtrapolatePosition ();
            transform.position = Vector3.Lerp (transform.position, _targetPos, _speed * Time.fixedDeltaTime);
        }

        void Start () {
            _targetPos = transform.position;
        }
        #endregion

        #region Private enums
        enum State {
            IDLE,
            WALK
        }
        #endregion

        #region Private properties
        State state = State.IDLE;
        Vector3 _lastKnownPos;
        Vector3 _move;
        Vector3 _targetPos;
        #endregion

        #region Private methods
        /*
         * Extrapolates the position of the character to "guess" the position
         * between two updates from the server
         */
        /*void ExtrapolatePosition () {
            if (State.WALK == state) {
                transform.position += _move * _speed * TimeManager.instance.timeScale * Time.fixedDeltaTime;
            }
        }*/
        #endregion
    }
}