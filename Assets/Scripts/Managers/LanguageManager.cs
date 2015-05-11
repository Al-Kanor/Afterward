using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DiosesModernos {
    public class LanguageManager : Singleton<LanguageManager> {
        #region Properties
        [SerializeField]
        [Tooltip ("languages.csv")]
        TextAsset csvFile;
        #endregion

        #region API
        public string GetText (string key) {
            if (null == dictionnary || 0 == dictionnary.Count) return "";
            string text = "";
            dictionnary.TryGetValue (key, out text);
            return text;
        }
        #endregion

        #region Unity
        void Start () {
            BuildDictionnary (Application.systemLanguage.ToString ());
        }
        #endregion

        #region Private methods
        void BuildDictionnary (string language) {
            string[] lines = csvFile.text.Split ("\n"[0]);
            dictionnary = new Dictionary<string, string> (lines.Length);
            string[] lineData;
            int column = -1;
            for (int l = 0; l < lines.Length; ++l) {
                lineData = (lines[l].Trim ()).Split (";"[0]);
                for (int c = 0; c < lineData.Length; ++c) {
                    if (0 == l && lineData[c] == language) {
                        // First line
                        column = c;
                        break;
                    }
                    if (column == c) {
                        dictionnary.Add (lineData[0], lineData[c]);
                    }
                }
            }
        }
        #endregion

        #region Private properties
        Dictionary<string, string> dictionnary = null;
        #endregion
    }
}