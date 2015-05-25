using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Afterward {
    public class GameManager : Singleton<GameManager> {
        #region Properties
        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Player")]
        Player _player;
        [SerializeField]
        [Tooltip ("Base difficulty")]
        DifficultyLevel _difficulty;
        #endregion

        #region Public enums
        public enum DifficultyLevel {
            HUMAN,
            CYBORG,
            GOD
        };
        #endregion

        #region Getters
        public DifficultyLevel difficulty {
            get { return _difficulty; }
        }

        public ArrayList enemies {
            get { return _enemies; }
        }

        public Player player {
            get { return _player; }
        }

        public int score {
            get { return _score; }
            set {
                _score = value;
                PlayerPrefs.SetInt ("score", _score);
                GuiManager.instance.UpdateScore ();
            }
        }
        #endregion

        #region API
        public void AddEnemy (Enemy enemy) {
            if (null == enemy) return;
            _enemies.Add (enemy);
        }

        public void Awake () {
            _enemies = new ArrayList ();
        }

        public void ResetGame () {
            Application.LoadLevel (Application.loadedLevel);
        }
        #endregion

        #region Unity
        void Start () {
            if (InputManager.instance.joystickConnected) {
                Cursor.visible = false;
            }
            _score = PlayerPrefs.GetInt ("score");
            GuiManager.instance.UpdateAll ();
        }

        void Update () {
            if (Input.GetKeyDown (KeyCode.Escape)) {
                Application.Quit ();
            }
        }
        #endregion

        #region Private properties
        int _score = 0;
        ArrayList _enemies;
        #endregion
    }
}