using UnityEngine;
using System.Collections;

namespace DiosesModernos {
    public class Character : MonoBehaviour {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Life points of the character")]
        [Range (0.0f, 1000.0f)]
        protected int _health = 100;
        [SerializeField]
        [Tooltip ("Speed of the character")]
        [Range (0.0f, 100.0f)]
        protected float _speed = 6;

        [Header ("Shoot")]
        [SerializeField]
        [Tooltip ("Distance between the player and the bullets shooted (a too high value can let the player shooting through walls)")]
        [Range (0.0f, 3.0f)]
        float _armLength = 0.9f;
        [SerializeField]
        [Tooltip ("Bullet shooted by the player")]
        GameObject _bulletPrefab;

        [Header ("SFX")]
        [SerializeField]
        [Tooltip ("Shoot SFX")]
        AudioClip _shootSFX;

        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Animation of the character (drag & drop the character model in child)")]
        protected Animation _animation;
        #endregion

        #region Getters
        public virtual int health {
            get { return _health; }
            set {
                _health = value;
            }
        }
        #endregion

        #region API
        public void Shoot () {
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
        }

        public virtual void TakeDamage (int damage) {
            health = Mathf.Clamp (_health, 0, _health - damage);
        }
        #endregion

        #region Unity
        /*void Awake () {
            _user = GetComponent<User> ();
        }*/
        #endregion

        #region Private properties
        #endregion

        #region Private methods
        
        #endregion
    }
}