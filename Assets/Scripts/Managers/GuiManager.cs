using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Afterward {
    public class GuiManager : Singleton<GuiManager> {
        #region Properties
        [Header ("Links")]
        /*[SerializeField]
        [Tooltip ("Score text")]
        Text _scoreText;*/
        [SerializeField]
        [Tooltip ("Energy text")]
        Text _energyText;
        [SerializeField]
        [Tooltip ("Play button")]
        Button _playButton;
        /*[SerializeField]
        [Tooltip ("Time bar UI")]
        Image _timeBarUI;
        [SerializeField]
        [Tooltip ("Time Factor UI")]
        Text _timeFactorUI;*/
        #endregion

        #region API
        public void UpdateAll () {
            LanguageManager lm = LanguageManager.instance;
            _playButton.transform.GetChild (0).GetComponent<Text> ().text = lm.GetText ("play");
            UpdateEnergy ();
            //UpdateScore ();
        }

        public void UpdateEnergy () {
            _energyText.text = GameManager.instance.player.energy.ToString ();
        }

        public void UpdateScore () {
            //_scoreText.text = LanguageManager.instance.GetText ("kills") + " : " + GameManager.instance.score;
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