using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    [SerializeField]
    TextMeshPro _text;

    public void SetText(string text) {
        _text.text = text;
    }
}
