using UnityEngine;

namespace DiosesModernos {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        #region Getters
        public static T instance {
            get {
                if (null == _instance) {
                    _instance = (T)FindObjectOfType (typeof (T));
                }
                return _instance;
            }
        }
        #endregion

        #region Private properties
        private static T _instance;
        #endregion
    }
}