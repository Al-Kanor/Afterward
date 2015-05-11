using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleNum : MonoBehaviour {
    #region Properties
    [Header ("Configuration")]
    [SerializeField]
    [Tooltip ("Colors of the number")]
    Color[] _colors;
    #endregion

    #region Unity
    void FixedUpdate () {
        if (0 == _colors.Length) return;
        _colorDst = _colors[Random.Range (0, _colors.Length)];
        GetComponent<Text> ().color = _colorDst;
    }
    #endregion

    #region Private properties
    Color _colorDst;
    #endregion
}
