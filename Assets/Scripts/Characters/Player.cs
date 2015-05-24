using UnityEngine;
using UnityEditor;
using System.Collections;
using XInputDotNetPure;

namespace Afterward {
    public class Player : Character {
        #region Properties
        [Header ("Energy System")]
        [SerializeField]
        [Tooltip ("Energy of the player")]
        [Range (0, 100)]
        float _energy = 20;

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
        float _dashDelay = 1f;
        [SerializeField]
        [Tooltip ("Delay when player starts dashing")]
        [Range (0.0f, 1.0f)]
        float _dashStartDelay = 0.24f;
        [SerializeField]
        [Tooltip ("Delay when player ends dashing")]
        [Range (0.0f, 1.0f)]
        float _dashEndDelay = 0.24f;
        [SerializeField]
        [Tooltip ("Duration of the particles emitted by a dash")]
        [Range (0.0f, 5.0f)]
        float _dashParticlesDuration = 1;
        [SerializeField]
        [Tooltip ("Dash particles")]
        GameObject _dashParticlesPrefab;

        [Header ("Physical Attack")]
        /*[SerializeField]
        [Tooltip ("")]
        float _attackStrength = 1000;
        [SerializeField]
        [Tooltip ("")]
        float _attackRadius = 4;*/
        [SerializeField]
        [Tooltip ("Attack Duration")]
        float _attackDuration = 0.2f;
        [SerializeField]
        [Tooltip ("Attack Cooldown")]
        float _attackCooldown = 1;
        [SerializeField]
        [Tooltip ("Attack zone")]
        GameObject _attackZone;
        [SerializeField]
        [Tooltip ("Attack particles")]
        ParticleSystem _attackParticles;

        /*[Header ("Shoot")]
        [SerializeField]
        [Tooltip ("Seconds between two shoots")]
        [Range (0.0f, 1.0f)]
        float _shootDelay = 0.1f;*/

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Layer of the floor")]
        LayerMask _floorMask;
        #endregion

        #region Getters
        public float energy {
            get { return _energy; }
            set {
                _energy = Mathf.Max (0, value);
                GuiManager.instance.UpdateEnergy ();
            }
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
            float h = 0, v = 0;
            if (_isDashing) {
                // Player is dashing
                h = _dashDirection.x;
                v = _dashDirection.z;
                GameObject dashParticlesObject = Instantiate (_dashParticlesPrefab, transform.position, transform.rotation) as GameObject;
                Destroy (dashParticlesObject, _dashParticlesDuration);
            }
            else if (_canMove) {
            //if (!_isDashing) {
                //float h, v;
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
            //}
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

            /*#region Dash
            GamePadState state = GamePad.GetState (0);
            if (!_isDashing && state.Triggers.Left > 0 || Input.GetButton ("Dash")) {
                StartCoroutine ("Dash");
                _isDashing = true;
            }
            else if (_isDashing && state.Triggers.Left == 0 || !Input.GetButton ("Dash")) {
                _isDashing = false;
            }
            #endregion*/

            #region Shoot
            /*if (!_isShooting && state.Triggers.Right > 0 || Input.GetButton ("Shoot")) {
                StartCoroutine ("UpdateShoot");
                _isShooting = true;
            }
            else if (_isShooting && state.Triggers.Right == 0 || !Input.GetButton ("Shoot") || _isDashing) {
                StopCoroutine ("UpdateShoot");
                _vibrationRight = 0;
                GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
                _isShooting = false;
            }*/
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
            #region Attack
            if (!_isAttacking && Input.GetButtonDown ("Attack")) {
                StartCoroutine ("Attack");
            }
            #endregion

            #region Time scale shortcut
            /*if (Input.GetButtonDown ("Time Down")) {
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
            }*/
            #endregion

            #region Crystal cast
            if (Input.GetButtonDown ("Fire2")) {
                PatternManager.instance.GenerateBottleNeck ();
            }
            #endregion

            #region Dash
            if (_canDash && Input.GetButtonDown ("Dash")) {
                StartCoroutine ("Dash");
                
            }
            /*else if (_isDashing && !Input.GetButton ("Dash")) {
                _isDashing = false;
            }*/
            #endregion
        }
        #endregion

        #region Private properties
        bool _isAttacking = false;

        bool _canMove = true;

        bool _isDashing = false;
        bool _canDash = true;
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
        IEnumerator Attack () {
            _canMove = false;
            _isAttacking = true;
            _attackZone.SetActive (true);

            SerializedObject so = new SerializedObject (_attackParticles);
            /*/ Test
            SerializedProperty it = so.GetIterator ();
            while (it.Next (true))
                Debug.Log (it.propertyPath);//*/
            /*so.FindProperty ("").floatValue = _attackDuration;
            so.ApplyModifiedProperties ();*/
            _attackParticles.Play ();

            yield return new WaitForSeconds (_attackDuration);
            _canMove = true;
            _attackZone.SetActive (false);
            yield return new WaitForSeconds (_attackCooldown);
            _isAttacking = false;
            /*Collider[] colliders = Physics.OverlapSphere (transform.position, _attackRadius);
            foreach (Collider collider in colliders) {
                if ("Crystal" == collider.tag) {
                    collider.Recycle ();
                    energy++;
                    continue;
                }
                Rigidbody rb = collider.GetComponent<Rigidbody> ();
                if (null != rb) rb.AddExplosionForce (_attackStrength, transform.position, _attackRadius);
            }*/
        }

        IEnumerator Dash () {
            //if (Time.time - _lastDash >= _dashDelay / TimeManager.instance.timeScale) {
            Debug.Log ("prepare dash");
            _canMove = false;
            _canDash = false;
            
            float initialSpeed = _speed;
            _speed = _dashSpeed;
            yield return new WaitForSeconds (_dashStartDelay / TimeManager.instance.timeScale);
            Debug.Log ("dash");
            _isDashing = true;
            _dashDirection = transform.forward;
            _vibrationLeft = InputManager.instance.leftVibrationStrength;
            GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
            yield return new WaitForSeconds (_dashDuration / TimeManager.instance.timeScale);
            Debug.Log ("rest");
            _speed = initialSpeed;
            _isDashing = false;
            _vibrationLeft = 0;
            GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
            yield return new WaitForSeconds (_dashEndDelay / TimeManager.instance.timeScale);
            Debug.Log ("end dashing"); 
            _canMove = true;
            _lastDash = Time.time;
            yield return new WaitForSeconds (_dashDelay / TimeManager.instance.timeScale);
            _canDash = true;
            //}
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

        /*IEnumerator UpdateShoot () {
            _vibrationRight = InputManager.instance.rightVibrationStrength;
            GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
            do {
                if (Time.time - _lastShoot >= _shootDelay / TimeManager.instance.timeScale) {
                    Shoot ();
                    _lastShoot = Time.time;
                }
                yield return new WaitForSeconds (_shootDelay / TimeManager.instance.timeScale);
            } while (true);
        }*/
        #endregion
    }
}