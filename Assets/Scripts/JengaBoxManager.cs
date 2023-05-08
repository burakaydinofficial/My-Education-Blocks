using System;
using System.Collections.Generic;
using UnityEngine;

public class JengaBoxManager : MonoBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private Vector3 _layerOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private Vector3 _rowOffset = new Vector3(2.5f, 0f, 0f);
    [SerializeField] private GameObject _jengaBoxPrefab;
    [SerializeField] private string _gradeTag = "6th Grade";

    [NonSerialized] private List<JengaBoxController> _instances = new List<JengaBoxController>();

    public void Set(StackApiRequest.StackApiDataElement[] data, Action<StackApiRequest.StackApiDataElement> clickCallback)
    {
        var instanceIndex = 0;
        for (var i = 0; i < data.Length; i++)
        {
            var element = data[i];
            if (!string.Equals(element.grade, _gradeTag))
                continue;

            if (instanceIndex >= _instances.Count)
            {
                var newInstance = Instantiate(_jengaBoxPrefab, _root, false);
                var layer = instanceIndex / 3;
                var inLayer = instanceIndex & 3;
                var position = _layerOffset * layer + ((inLayer - 1) * _rowOffset);
                newInstance.transform.localPosition = position;
                newInstance.transform.localEulerAngles = new Vector3(0f, (layer % 2) * 90f, 0f);
                _instances.Add(newInstance.GetComponent<JengaBoxController>());
            }

            _instances[instanceIndex].Set(element, clickCallback);
        }
    }

    public void Hide()
    {
        foreach (var jengaBoxController in _instances)
        {
            jengaBoxController.Hide();
        }
    }
}