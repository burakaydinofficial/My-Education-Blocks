using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeanButtonController : MonoBehaviour
{
    [SerializeField] private Text _text;
    private Action _clickCallback;

    public void Set(string text, Action callback)
    {
        if (_text) _text.text = text;
        _clickCallback = callback;
    }

    public void Click()
    {
        _clickCallback?.Invoke();
    }
}
