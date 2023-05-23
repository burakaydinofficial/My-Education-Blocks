using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Transform _buttonRoot;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private RectTransform _buttonBar;
    [SerializeField] private Vector2 _buttonBarBaseSize;
    [SerializeField] private Vector2 _buttonBarMaxSize;
    [SerializeField] private Vector2 _buttonBarPerButton;

    [NonSerialized] private List<LeanButtonController> _usedButtons = new List<LeanButtonController>();
    [NonSerialized] private Stack<LeanButtonController> _unusedButtons = new Stack<LeanButtonController>();

    void Awake()
    {
        Hide();
    }

    public void Set(string[] options, Action<string> callback)
    {
        Hide();
        var barSize = CalcButtonBarSize(options.Length);
        if (_buttonBar) _buttonBar.sizeDelta = barSize;
        foreach (var s in options)
        {
            var button = GetFreeButton();
            button.Set(s, () => callback?.Invoke(s));
            _usedButtons.Add(button);
        }
        _root.SetActive(true);
    }

    public void Hide()
    {
        _root.SetActive(false);
        foreach (var button in _usedButtons)
        {
            button.gameObject.SetActive(false);
            _unusedButtons.Push(button);
        }
        _usedButtons.Clear();
    }

    private LeanButtonController GetFreeButton()
    {
        if (_unusedButtons.Count > 0)
        {
            var button = _unusedButtons.Pop();
            button.gameObject.SetActive(true);
            return button;
        }

        var go = Instantiate(_buttonPrefab, _buttonRoot);
        var co = go.GetComponent<LeanButtonController>();
        return co;
    }

    private Vector2 CalcButtonBarSize(int count)
    {
        return Vector2.Min(_buttonBarMaxSize, _buttonBarBaseSize + (count * _buttonBarPerButton));
    }
}
