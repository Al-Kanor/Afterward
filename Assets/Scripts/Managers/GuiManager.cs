using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DiosesModernos {
    public class GuiManager : Singleton<GuiManager> {
        #region Properties
        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Score text")]
        Text scoreText;
        [SerializeField]
        [Tooltip ("Online button")]
        Button onlineButton;
        [SerializeField]
        [Tooltip ("Offline button")]
        Button offlineButton;
        /*[SerializeField]
        [Tooltip ("Time bar UI")]
        Image _timeBarUI;
        [SerializeField]
        [Tooltip ("Time Factor UI")]
        Text _timeFactorUI;*/
        #endregion

        #region API
        public void ActivateOnlineButton () {
            onlineButton.interactable = true;
            UpdateScore ();
        }

        public void UpdateAll () {
            LanguageManager lm = LanguageManager.instance;
            onlineButton.transform.GetChild (0).GetComponent<Text> ().text = lm.GetText ("play-online");
            offlineButton.transform.GetChild (0).GetComponent<Text> ().text = lm.GetText ("play-offline");
            UpdateScore ();
        }

        public void UpdateScore () {
            scoreText.text = LanguageManager.instance.GetText ("kills") + " : " + GameManager.instance.score;
        }
        #endregion

        #region Unity
        /*void Awake () {
            timeBarWidthMax = _timeBarUI.rectTransform.sizeDelta.x;
            timeBarPadding = _timeBarUI.rectTransform.anchoredPosition.x - timeBarWidthMax / 2;
        }*/

        /*void FixedUpdate () {
            float timeScale = TimeManager.instance.timeScale;
            float timeScaleMin = TimeManager.instance.timeScaleMin;
            float timeScaleMax = TimeManager.instance.timeScaleMax;
            float energy = GameManager.instance.player.energy;

            _timeFactorUI.text = "x" + decimal.Round (new decimal (timeScale), 2);
            _timeFactorUI.color = timeScale < 1 ?
                Color.Lerp (Color.green, Color.blue,
                    (timeScale - timeScaleMin) * timeScaleMax / (timeScaleMax - 1)
                ) :
                Color.Lerp (Color.blue, Color.red,
                    (timeScale - 1) / (timeScaleMax - 1)
                )
            ;

            _timeBarUI.rectTransform.sizeDelta = new Vector2 (
                timeBarWidthMax * energy / 100,
                _timeBarUI.rectTransform.sizeDelta.y
            );

            _timeBarUI.rectTransform.anchoredPosition = new Vector2 (
                _timeBarUI.rectTransform.sizeDelta.x / 2 + timeBarPadding,
                _timeBarUI.rectTransform.anchoredPosition.y
            );

            _timeBarUI.color = Color.Lerp (Color.red, Color.yellow, energy / 100);
        }*/
        #endregion

        #region Private properties
        //float timeBarWidthMax;
        //float timeBarPadding;
        #endregion
    }
}