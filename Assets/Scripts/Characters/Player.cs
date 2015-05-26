using UnityEngine;
using System.Collections;
using XInputDotNetPure;

namespace Afterward {
    public class Player : Character {
        #region Properties
        [Header ("Energy System")]
        [SerializeField]
        [Tooltip ("Energy of the player")]
        [Range (0, 100)]
        float _energy = 99;

        #region Dash
        [Header ("Dash")]
        [SerializeField]
        [Tooltip ("Speed while dashing")]
        [Range (0.0f, 100.0f)]
        float _dashSpeed = 50;
        [SerializeField]
        [Tooltip ("Acceleration while dashing")]
        [Range (0.0f, 10.0f)]
        float _dashAcceleration = 3.81f;
        [SerializeField]
        [Tooltip ("Duration of a dash")]
        [Range (0.0f, 1.0f)]
        float _dashDuration = 0.28f;
        [SerializeField]
        [Tooltip ("Seconds between two dashes")]
        [Range (0.0f, 3.0f)]
        float _dashDelay = 0.1f;
        [SerializeField]
        [Tooltip ("Delay when player starts dashing")]
        [Range (0.0f, 1.0f)]
        float _dashStartDelay = 0.1f;
        [SerializeField]
        [Tooltip ("Delay when player ends dashing")]
        [Range (0.0f, 1.0f)]
        float _dashEndDelay = 0.4f;
        [SerializeField]
        [Tooltip ("Duration of the particles emitted by a dash")]
        [Range (0.0f, 5.0f)]
        float _dashParticlesDuration = 1;
        [SerializeField]
        [Tooltip ("Dash line")]
        LineRenderer _dashLine;
        [SerializeField]
        [Tooltip ("Dash particles")]
        GameObject _dashParticlesPrefab;
        #endregion

        #region Physical attack
        [Header ("Physical Attack")]
        /*[SerializeField]
        [Tooltip ("")]
        float _attackStrength = 1000;
        [SerializeField]
        [Tooltip ("")]
        float _attackRadius = 4;*/
        [SerializeField]
        [Tooltip ("Distance traveled by the player while attacking")]
        float _attackStepDist = 1;
        [SerializeField]
        [Tooltip ("Speed of the player while attacking")]
        float _attackStepSpeed = 10;
        [SerializeField]
        [Tooltip ("Attack Duration")]
        float _attackDuration = 0.18f;
        [SerializeField]
        [Tooltip ("Attack Cooldown")]
        float _attackCooldown = 0.18f;
        [SerializeField]
        [Tooltip ("Attack zone")]
        GameObject _attackZone;
        [SerializeField]
        [Tooltip ("Attack particles")]
        ParticleSystem _attackParticles;
        #endregion

        #region Crystal patterns
        [Header ("Crystal Patterns")]
        [SerializeField]
        [Tooltip ("Default pattern launched (bottleneck or arc)")]
        string _defaultPattern = "bottleneck";
        [SerializeField]
        [Tooltip ("Seconds between display of cursor")]
        [Range (0, 5)]
        float _patternStartDelay = 0.5f;
        [SerializeField]
        [Tooltip ("Pattern loading particles")]
        ParticleSystem _patternLoadingParticles;
        [SerializeField]
        [Tooltip ("Pattern ready particles")]
        ParticleSystem _patternReadyParticles;
        #endregion

        #region Shoot
        /*[Header ("Shoot")]
        [SerializeField]
        [Tooltip ("Seconds between two shoots")]
        [Range (0.0f, 1.0f)]
        float _shootDelay = 0.1f;*/
        #endregion

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Layer of the floor")]
        LayerMask _floorMask;
        #endregion

        #region Getters
        public float energy {
            get { return _energy; }
            set {
                _energy = Mathf.Clamp (value, 0, 99);
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
                _speed = Mathf.Min (_speed + _dashAcceleration, _dashSpeed);
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
            //transform.position = newPos;
            GetComponent<Rigidbody> ().MovePosition (newPos);
            // Physic hack to avoid player auto move
            //GetComponent<Rigidbody> ().velocity = Vector3.zero;
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

            #region Crystal patterns
            if (Input.GetButtonDown ("Switch Pattern")) {
                _defaultPattern = "arc" == _defaultPattern ? "bottleneck" : "arc";
            }

            if (!_isLoadingPattern && Input.GetButtonDown ("Crystal Pattern")) {
                StartCoroutine ("LaunchCrystalPattern");
            }
            else if (Input.GetButtonUp ("Crystal Pattern")) {
                StopCoroutine ("LaunchCrystalPattern");
                if (_canLaunchPattern) {
                    PatternManager pm = PatternManager.instance;
                    energy -= pm.Cost (_defaultPattern);
                    pm.GeneratePattern (_defaultPattern);
                }
                _patternLoadingParticles.Stop ();
                _patternReadyParticles.Stop ();
                _isLoadingPattern = false;
                _canLaunchPattern = false;
                _canMove = true;
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

        bool _isLoadingPattern = false;
        bool _canLaunchPattern = false;

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
            //GetComponent<Rigidbody> ().AddForce (transform.forward * _attackStep);
            StartCoroutine ("AttackStep");
            /*/ Test
            SerializedObject so = new SerializedObject (_attackParticles);
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

        IEnumerator AttackStep () {
            Vector3 targetPos = transform.position + transform.forward * _attackStepDist;
            float targetTime = _attackStepDist / _attackStepSpeed;
            do {
                transform.position = Vector3.Lerp (transform.position, targetPos, _attackStepSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate ();
            //} while (Vector3.Distance (transform.position, targetPos) > 0.1f);
                targetTime -= Time.fixedDeltaTime;
            } while (targetTime > 0);
        }

        IEnumerator Dash () {
            //Debug.Log ("prepare dash");
            _canMove = false;
            _canDash = false;
            //_dashLine.SetPosition (1, new Vector3 (0, transform.rotation.eulerAngles.y, 3));
            float initialSpeed = _speed;
            yield return new WaitForSeconds (_dashStartDelay / TimeManager.instance.timeScale);
            //Debug.Log ("dash");
            _isDashing = true;
            Camera.main.GetComponent<DashingCamera> ().StartCoroutine ("StartRecoil");
            GetComponent<CapsuleCollider> ().enabled = false;
            _dashDirection = transform.forward;
            _vibrationLeft = InputManager.instance.leftVibrationStrength;
            GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
            yield return new WaitForSeconds (_dashDuration / TimeManager.instance.timeScale);
            //Debug.Log ("rest");
            Camera.main.GetComponent<DashingCamera> ().StartCoroutine ("StopRecoil");
            _speed = initialSpeed;
            _isDashing = false;
            GetComponent<CapsuleCollider> ().enabled = true;
            _vibrationLeft = 0;
            GamePad.SetVibration (0, _vibrationLeft, _vibrationRight);
            yield return new WaitForSeconds (_dashEndDelay / TimeManager.instance.timeScale);
            //Debug.Log ("end dashing");
            _canMove = true;
            _lastDash = Time.time;
            yield return new WaitForSeconds (_dashDelay / TimeManager.instance.timeScale);
            _canDash = true;
        }

        IEnumerator LaunchCrystalPattern () {
            _isLoadingPattern = true;
            _canMove = false;
            _patternLoadingParticles.Play ();
            yield return new WaitForSeconds (_patternStartDelay);
            if (_energy >= PatternManager.instance.Cost (_defaultPattern)) {
                _patternReadyParticles.Play ();
                _canLaunchPattern = true;
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