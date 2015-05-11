using UnityEngine;
using System.Collections;

namespace DiosesModernos {
    public class PlayableCharacter : Character {
        #region Properties
        [Header ("General")]
        [SerializeField]
        [Tooltip ("Character id")]
        string _id;
        #endregion

        #region Getters
        public string id {
            get { return _id; }
            set { _id = value; }
        }
        #endregion

        #region Unity
        void Awake () {
            if (0 == _id.Length) {
                _id = SystemInfo.deviceUniqueIdentifier;
            }
        }
        #endregion
    }
}