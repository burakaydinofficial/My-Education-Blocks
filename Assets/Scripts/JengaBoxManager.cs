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

    public string GradeTag { get; private set; }
    [NonSerialized] private List<JengaBoxController> _instances = new List<JengaBoxController>();

    public void Set(StackApiRequest.StackApiDataElement[] data, Action<JengaBoxController> clickCallback, string gradeTag = null)
    {
        var instanceIndex = 0;

        var rotatedRowOffset = new Vector3(_rowOffset.z, _rowOffset.y, _rowOffset.x);

        GradeTag = gradeTag ?? _defaultGradeTag;
        Debug.Log("Initializing " + GradeTag);

        for (var i = 0; i < data.Length; i++)
        {
            var element = data[i];
            if (!string.Equals(element.grade, GradeTag))
                continue;

            var layer = instanceIndex / 3;
            var inLayer = instanceIndex % 3;
            var rowOffsetForLayer = (layer % 2 == 0 ? _rowOffset : rotatedRowOffset);
            var position = _baseOffset + _layerOffset * layer + (inLayer - 1) * rowOffsetForLayer;
            JengaBoxController instance;
            if (instanceIndex >= _instances.Count)
            {
                instance = Instantiate(_jengaBoxPrefab, _root, false).GetComponent<JengaBoxController>();
                _instances.Add(instance);
            }
            else
            {
                instance = _instances[instanceIndex];
            }
            instance.transform.localPosition = position;
            instance.transform.localEulerAngles = new Vector3(0f, (layer % 2) * 90f, 0f);

            _instances[instanceIndex].Set(element, clickCallback);
            instanceIndex++;
        }

        if (_tagText)
            _tagText.text = GradeTag;

        Tested = false;
    }

    public bool Tested { get; private set; }

    public void Test()
    {
        foreach (var instance in _instances)
        {
            instance.Set(false);
            if (instance.Data.mastery == 0)
            {
                instance.Hide();
            }
        }
        Tested = true;
    }

    public void Hide()
    {
        foreach (var jengaBoxController in _instances)
        {
            jengaBoxController.Hide();
        }
    }
}