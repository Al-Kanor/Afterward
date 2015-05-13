using UnityEngine;
using System.Collections;

namespace Afterward {
    public class TimeManager : Singleton<TimeManager> {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Minimal time scale")]
        float _timeScaleMin = 0.25f;
        [SerializeField]
        [Tooltip ("Maximal time scale")]
        float _timeScaleMax = 4;
        [SerializeField]
        [Tooltip ("Time down speed")]
        float _timeDownSpeed = 0.75f;
        [SerializeField]
        [Tooltip ("Time up speed")]
        float _timeUpSpeed = 1.5f;
        #endregion

        #region Getters
        public float timeDownSpeed {
            get { return _timeDownSpeed; }
            set { _timeDownSpeed = value; }
        }

        public float timeUpSpeed {
            get { return _timeUpSpeed; }
            set { _timeUpSpeed = value; }
        }

        public float timeScale {
            get { return _timeScale; }
            set { _timeScale = value; }
        }

        public float timeScaleMax {
            get { return _timeScaleMax; }
            set { _timeScaleMax = value; }
        }

        public float timeScaleMin {
            get { return _timeScaleMin; }
            set { _timeScaleMin = value; }
        }
        #endregion

        #region Unity
        /*
    void FixedUpdate () {
        float speedFactor = timeScale > 1 ? 4 : 1;
        timeScale = Mathf.Min (timeScale + timeUpSpeed * Time.fixedDeltaTime, timeScaleMax);
    }
    */
        void Start () {
            //timeUpSpeed = 0.01f;
            //timeScale = timeScaleMax;
            switch (GameManager.instance.difficulty) {
                case GameManager.DifficultyLevel.HUMAN:
                    _timeScale = 1;
                    break;
                case GameManager.DifficultyLevel.CYBORG:
                    _timeScale = 2.5f;
                    break;
                case GameManager.DifficultyLevel.GOD:
                    _timeScale = 4;
                    break;
            }
        }
        #endregion

        #region Private properties
        float _timeScale = 1;
        #endregion
    }
}