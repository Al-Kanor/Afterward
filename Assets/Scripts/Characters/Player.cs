using UnityEngine;
using System.Collections;
using XInputDotNetPure;

namespace DiosesModernos {
    public class Player : Character {
        #region Properties
        [Header ("Dash")]
        [SerializeField]
        [Tooltip ("Speed while dashing (a too high value can let the player dash trough walls)")]
        [Range (0.0f, 100.0f)]
        float _dashSpeed = 48;
        [SerializeField]
        [Tooltip ("Duration of a dash")]
        [Range (0.0f, 1.0f)]
        float _dashDuration = 0.12f;
        [SerializeField]
        [Tooltip ("Seconds between two dashes")]
        [Range (0.0f, 3.0f)]
        float _dashDelay = 0.24f;
        [SerializeField]
        [Tooltip ("Duration of the particles emitted by a dash")]
        [Range (0.0f, 5.0f)]
        float _dashParticlesDuration = 1;
        [SerializeField]
        [Tooltip ("Dash particles")]
        GameObject _dashParticlesPrefab;

        [Header ("Shoot")]
        [SerializeField]
        [Tooltip ("Seconds between two shoots")]
        [Range (0.0f, 1.0f)]
        float _shootDelay = 0.1f;

        [Header ("Crystals")]
        [SerializeField]
        [Tooltip ("Distance between the player and the cristals casted")]
        [Range (0.0f, 10.0f)]
        float _crystalDistance = 1;
        [SerializeField]
        [Tooltip ("Crystal casted by the player")]
        GameObject _crystalPrefab;

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Layer of the floor")]
        LayerMask _floorMask;
        #endregion

        #region Getters
        public float energy {
            get { return _energy; }
        }
        #endregion

        #region Unity
        void FixedUpdate () {
            /*
            #region Time scaling
            // Time change speed is 4x if time scale > 1
            float speedFactor = TimeManager.Instance.TimeScale > 1 ? 4 : 1;

            if (Input.GetButton ("Time Down") && Input.GetButton ("Time Up")) {
                TimeManager.Instance.TimeScale = 1;
            }
            else if (Input.GetButton ("Time Down")) {
                TimeManager.Instance.TimeScale = Mathf.Max (TimeManager.Instance.TimeScale - TimeManager.Instance.TimeDownSpeed * speedFactor * Time.fixedDeltaTime, TimeManager.Instance.TimeScaleMin);
            }
            else if (Input.GetButton ("Time Up")) {
                TimeManager.Instance.TimeScale = Mathf.Min (TimeManager.Instance.TimeScale + TimeManager.Instance.TimeDownSpeed * speedFactor * Time.fixedDeltaTime, TimeManager.Instance.TimeScaleMax);
            }

            energy = Mathf.Clamp (
                energy + (TimeManager.Instance.TimeScale - 1) / speedFactor,
                0, 100
            );

            if (energy < 0.01f) {
                TimeManager.Instance.TimeScale = 1;
            }
            #endregion
            */

            #region Movement
            float h, v;
            if (_speed == _dashSpeed) {
                // Player is dashing
                h = _dashDirection.x;
                v = _dashDirection.z;
                GameObject dashParticlesObject = Instantiate (_dashParticlesPrefab, transform.position, transform.rotation) as GameObject;
                Destroy (dashParticlesObject, _dashParticlesDuration);
            }
            else {
                // Normal move
                h = Input.GetAxis ("Horizontal");
                v = Input.GetAxis ("Vertical");
            }
            Vector3 move = new Vector3 (h, 0, v);
            move.Normalize ();
            Vector3 newPos = transform.position + move * _speed * TimeManager.instance.timeScale * Time.fixedDeltaTime;
            if (InputManager.instance.joystickConnected) {
                transform.LookAt (newPos);
            }
            transform.position = newPos;
            // Physic hack to avoid player auto move
            GetComponent<Rigidbody> ().velocity = Vector3.zero;
            #endregion

            #region Static rotation
            if (InputManager.instance.joystickConnected) {
                // Rotation with joystick
                float hr = Input.GetAxis ("Horizontal Rotation");
                float vr = Input.GetAxis ("Vertical Rotation");
                Vector3 direction = new Vector3 (hr, 0, vr);
                Vector3 targetPos = transform.position + direction;
                transform.LookAt (targetPos);
            }
            else {
                // Rotation with mouse
                Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit floorHit;

                if (Physics.Raycast (camRay, out floorHit, _camRayLength, _floorMask)) {
                    Vector3 playerToMouse = floorHit.point - transform.position;
                    playerToMouse.y = 0f;
                    Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
                    GetComponent<Rigidbody> ().MoveRotation (newRotation);
                    //transform.rotation = newRotation;
                }
            }
            #endregion

            #region Dash
            GamePadState state = GamePad.GetState (0);
            if (!_isDashing && state.Triggers.Left > 0 || Input.GetButton ("Dash")) {
                StartCoroutine ("Dash");
                _isDashing = true;
            }
            else if (_isDashing && state.Triggers.Left == 0 || !Input.GetButton ("Dash")) {
                _isDashing = false;
            }
            #endregion

            #region Shoot
            if (!_isShooting && state.Triggers.Right > 0 || Input.GetButton ("Shoot")) {
                StartCoroutine ("UpdateShoot");
                _isShooting = true;
            }
            else if (_isShooting && state.Triggers.Right == 0 || !Input.GetButton ("Shoot") || _isDashing) {
                StopCoroutine ("UpdateShoot");
                _vibrationRight = 0;
                GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
                _isShooting = false;
            }
            #endregion

            #region Animation
            string clipName = 0 == h && 0 == v ? "Idle" : "Walk";
            _animation.Play (clipName);
            _animation[clipName].speed = TimeManager.instance.timeScale;
            #endregion
        }

        void Start () {
            transform.position = new Vector3 (Random.Range (-20, 20), 0.5f, Random.Range (-20, 20));
        }

        void Update () {
            #region Time scale reset
            if (Input.GetButtonDown ("Time Down")) {
                if (Time.time - _lastTimeDownTap <= InputManager.instance.doubleTapDelay) {
                    TimeManager.instance.timeScale = TimeManager.instance.timeScaleMin;
                }
                else {
                    _lastTimeDownTap = Time.time;
                }
            }
            else if (Input.GetButtonDown ("Time Up")) {
                if (Time.time - _lastTimeUpTap <= InputManager.instance.doubleTapDelay) {
                    TimeManager.instance.timeScale = TimeManager.instance.timeScaleMax;
                }
                else {
                    _lastTimeUpTap = Time.time;
                }
            }
            #endregion

            #region Crystal cast
            if (Input.GetButtonDown ("Fire2")) {
                ObjectPool.Spawn (_crystalPrefab, transform.position + transform.forward * _crystalDistance, Quaternion.identity);
            }
            #endregion
        }
        #endregion

        #region Private properties
        // Current player energy (%)
        float _energy = 100;

        bool _isDashing = false;
        float _lastDash = 0;
        Vector3 _dashDirection;

        bool _isShooting = false;
        float _lastShoot = 0;

        float _vibrationLeft = 0;
        float _vibrationRight = 0;

        float _camRayLength = 100;

        float _lastTimeDownTap = 0;
        float _lastTimeUpTap = 0;
        #endregion

        #region Private methods
        IEnumerator Dash () {
            if (Time.time - _lastDash >= _dashDelay / TimeManager.instance.timeScale) {
                _lastDash = Time.time;
                _dashDirection = transform.forward;
                float initialSpeed = _speed;
                _speed = _dashSpeed;
                _vibrationLeft = InputManager.instance.leftVibrationStrength;
                GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
                yield return new WaitForSeconds (_dashDuration / TimeManager.instance.timeScale);
                _speed = initialSpeed;
                _vibrationLeft = 0;
                GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
            }
        }

        /*void Shoot () {
            MultiplayerManager.instance.SendPlayerShoot ();
            GameObject b = ObjectPool.Spawn (_bulletPrefab, transform.position + transform.forward * _armLength, transform.rotation);
            b.GetComponent<Bullet> ().launcher = this;
            b = ObjectPool.Spawn (_bulletPrefab, transform.position + transform.forward * _armLength, transform.rotation);
            b.GetComponent<Bullet> ().launcher = this;
            b.transform.position += transform.right * 0.2f;
            b.transform.Rotate (transform.up * 5);
            b = ObjectPool.Spawn (_bulletPrefab, transform.position + transform.forward * _armLength, transform.rotation);
            b.GetComponent<Bullet> ().launcher = this;
            b.transform.position -= transform.right * 0.2f;
            b.transform.Rotate (transform.up * -5);
            SoundManager.instance.RandomizeSfx (_shootSFX);
        }*/

        IEnumerator UpdateShoot () {
            _vibrationRight = InputManager.instance.rightVibrationStrength;
            GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
            do {
                if (Time.time - _lastShoot >= _shootDelay / TimeManager.instance.timeScale) {
                    Shoot ();
                    _lastShoot = Time.time;
                }
                yield return new WaitForSeconds (_shootDelay / TimeManager.instance.timeScale);
            } while (true);
        }
        #endregion
    }
}