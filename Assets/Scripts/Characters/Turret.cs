using UnityEngine;
using System.Collections;

namespace DiosesModernos {
    public class Turret : MonoBehaviour {
        #region properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Minimal delay between two shoots")]
        float _shootDelayMin = 0.1f;
        [SerializeField]
        [Tooltip ("Maximal delay between two shoots")]
        float _shootDelayMax = 2f;
        [SerializeField]
        [Tooltip ("Rotation speed min")]
        float _rotationSpeedMin = 0;
        [SerializeField]
        [Tooltip ("Rotation speed max")]
        float _rotationSpeedMax = 20;
        [SerializeField]
        [Tooltip ("Distance between the turret and the bullets shooted")]
        [Range (0.0f, 3.0f)]
        float _armLength = 0.3f;

        [Header ("SFX")]
        [SerializeField]
        [Tooltip ("Shoot SFX")]
        AudioClip _shootSFX;

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Bullet prefab")]
        GameObject _bulletPrefab;
        #endregion

        #region Unity
        void FixedUpdate () {
            transform.Rotate (Vector3.up * rotationSpeed * TimeManager.instance.timeScale);
        }

        void Start () {
            shootDelay = Random.Range (_shootDelayMin, _shootDelayMax);
            rotationSpeed = Random.Range (_rotationSpeedMin, _rotationSpeedMax);
            StartCoroutine ("UpdateShoot");
        }
        #endregion

        #region Private
        float shootDelay;
        float rotationSpeed;

        void Shoot () {
            ObjectPool.Spawn (_bulletPrefab, transform.position + transform.forward * _armLength, transform.rotation);
            SoundManager.instance.RandomizeSfx (_shootSFX);
        }

        IEnumerator UpdateShoot () {
            do {
                yield return new WaitForSeconds (shootDelay / TimeManager.instance.timeScale);
                Shoot ();
            } while (true);
        }
        #endregion
    }
}