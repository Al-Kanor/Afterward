using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Title : MonoBehaviour {
    #region Properties
    [Header ("Configuration")]
    [SerializeField]
    [Tooltip ("Colors of the title")]
    Color[] _colors;
    [SerializeField]
    [Tooltip ("Color change speed")]
    [Range(0, 100)]
    float _speed = 10;
    #endregion

    #region Unity
    void FixedUpdate () {
        if (0 == _colors.Length) return;
        GetComponent<Text> ().color = Color.Lerp (GetComponent<Text> ().color, _colorDst, _speed * Time.fixedDeltaTime);
        if (    Mathf.Abs (GetComponent<Text> ().color.r - _colorDst.r) < 0.01f
            &&  Mathf.Abs (GetComponent<Text> ().color.g - _colorDst.g) < 0.01f
            &&  Mathf.Abs (GetComponent<Text> ().color.b - _colorDst.b) < 0.01f) {
            // Clamp
            GetComponent<Text> ().color = _colorDst;
            _colorDst = _colors[Random.Range (0, _colors.Length)];
        }
    }

    void Start () {
        if (0 == _colors.Length) return;
        _colorDst = _colors[Random.Range (0, _colors.Length)];
        
    }
    #endregion

    #region Private properties
    Color _colorDst;
    #endregion
}
