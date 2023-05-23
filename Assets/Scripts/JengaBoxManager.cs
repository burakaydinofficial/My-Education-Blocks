using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class JengaBoxManager : MonoBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private Vector3 _baseOffset = new Vector3(0f, 0.75f, 0f);
    [SerializeField] private Vector3 _layerOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private Vector3 _rowOffset = new Vector3(0f, 0f, 2.5f);
    [SerializeField] private GameObject _jengaBoxPrefab;
    [SerializeField, FormerlySerializedAs("_gradeTag")] private string _defaultGradeTag = "6th Grade";
    [SerializeField] private TextMeshProUGUI _tagText;

    [NonSerialized] private string m_gradeTag;
    [NonSerialized] private List<JengaBoxController> _instances = new List<JengaBoxController>();

    public void Set(StackApiRequest.StackApiDataElement[] data, Action<JengaBoxController> clickCallback, string gradeTag = null)
    {
        var instanceIndex = 0;

        var rotatedRowOffset = new Vector3(_rowOffset.z, _rowOffset.y, _rowOffset.x);

        m_gradeTag = gradeTag ?? _defaultGradeTag;
        Debug.Log("Initializing " + m_gradeTag);

        for (var i = 0; i < data.Length; i++)
        {
            var element = data[i];
            if (!string.Equals(element.grade, m_gradeTag))
                continue;

            if (instanceIndex >= _instances.Count)
            {
                var newInstance = Instantiate(_jengaBoxPrefab, _root, false);
                var layer = instanceIndex / 3;
                var inLayer = instanceIndex % 3;
                var rowOffsetForLayer = (layer % 2 == 0 ? _rowOffset : rotatedRowOffset);
                var position = _baseOffset + _layerOffset * layer + (inLayer - 1) * rowOffsetForLayer;
                newInstance.transform.localPosition = position;
                newInstance.transform.localEulerAngles = new Vector3(0f, (layer % 2) * 90f, 0f);
                _instances.Add(newInstance.GetComponent<JengaBoxController>());
            }

            _instances[instanceIndex].Set(element, clickCallback);
            instanceIndex++;
        }

        if (_tagText)
            _tagText.text = m_gradeTag;
    }

    public void Hide()
    {
        foreach (var jengaBoxController in _instances)
        {
            jengaBoxController.Hide();
        }
    }
}