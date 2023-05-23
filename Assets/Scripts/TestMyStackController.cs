using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class TestMyStackController : MonoBehaviour
{
    private bool _starting = false;
    private bool _started = false;

    private StackApiRequest.StackApiDataElement[] _data;

    [SerializeField] private GameObject _loadingScreenRoot;
    [SerializeField] private GameObject _errorScreenRoot;

    [SerializeField] private Transform _stageRoot;
    [SerializeField] private GameObject _stagePrefab;

    [NonSerialized] private List<JengaBoxManager> _boxManagers = new List<JengaBoxManager>();

    [SerializeField] BoxInfoController _boxInfoController;
    [SerializeField] private LayerMask _clickLayerMask;

    [SerializeField] private bool _highlightWhileSelected = true;

    [NonSerialized] private JengaBoxController _selected;
    [NonSerialized] private JengaBoxController _highlighted;

    [NonSerialized] private TurnTable _turnTable;

    [Range(0f, 1f)]
    [SerializeField] private float _turnTablePosition = 0f;
    [SerializeField] private int _turnTableIndex = 0;

    [SerializeField] private TopMenuManager _topMenuManager;

    private IEnumerator Start()
    {
        _starting = true;
        SetErrorScreen(false);
        SetLoadingScreen(true);
        var stackApiRequest = new StackApiRequest();
        var request = stackApiRequest.Start();
        while (request.MoveNext())
        {
            yield return null;
        }

        if (!stackApiRequest.Success)
        {
            for (int i = 0; i < 2; i++)
            {
                request = stackApiRequest.Start();
                while (request.MoveNext())
                {
                    yield return null;
                }
                if (stackApiRequest.Success)
                    break;
            }

            if (!stackApiRequest.Success)
            {
                SetLoadingScreen(false);
                SetErrorScreen(true);
                _starting = false;
                yield break;
            }
        }

        _data = stackApiRequest.Data;
        Array.Sort(_data, Sorter);

        HashSet<string> grades = new HashSet<string>();
        foreach (var stackApiDataElement in _data)
        {
            grades.Add(stackApiDataElement.grade);
        }

        _turnTable = new TurnTable(_stageRoot, 180f, grades.Count * 30f, grades.Count, Vector3.forward);

        foreach (var grade in grades)
        {
            var go = Instantiate(_stagePrefab, _stageRoot, false);

            _turnTable.Deploy(go.transform);

            var manager = go.GetComponent<JengaBoxManager>();
            _boxManagers.Add(manager);
            manager.Set(_data, ClickCallback, grade);
        }

        _turnTable.Select(0, true);
        Select(0);
        SetLoadingScreen(false);
        if (_topMenuManager)
            _topMenuManager.Set(grades.ToArray(), Select);

        _started = true;
        _starting = false;
    }

    private void Select(string obj)
    {
        var index = _boxManagers.FindIndex(x => x.GradeTag == obj);
        Select(index);
    }

    [NonSerialized] private int _selectionIndex = 0;
    private void Select(int index)
    {
        if (_selectionIndex != index)
        {
            if (_boxManagers[_selectionIndex].Tested)
            {
                _boxManagers[_selectionIndex].Freeze();
            }

            if (_boxManagers[index].Tested)
            {
                _boxManagers[index].ResetFreeze();
            }

            ClickOutside();
        }
        _selectionIndex = index;
        _turnTable.Select(index);
    }

    private static int Sorter(StackApiRequest.StackApiDataElement e1, StackApiRequest.StackApiDataElement e2)
    {
        var g = string.CompareOrdinal(e1.grade, e2.grade);
        if (g != 0) return g;
        var d = string.CompareOrdinal(e1.domain, e2.domain);
        if (d != 0) return d;
        var c = string.CompareOrdinal(e1.cluster, e2.cluster);
        if (c != 0) return c;
        var s = string.CompareOrdinal(e1.standardid, e2.standardid);
        return s;
    }

    private void Update()
    {
        if (_started && !_starting)
            CheckClicks();

        if (_turnTable != null)
        {
            _turnTable.UpdatePosition();
            if (_turnTable.PositionVel > 0.01f)
                _boxManagers[_selectionIndex].Freeze();
            else
                _boxManagers[_selectionIndex].ResetFreeze();
        }
    }

    public void Test()
    {
        var target = _boxManagers[_selectionIndex];
        if (target.Tested)
            target.Set(_data, ClickCallback, target.GradeTag);
        else target.Test();
    }

    private int _clickDownFrame = -1;
    private void CheckClicks()
    {
        if (Input.GetMouseButtonDown(0))
            _clickDownFrame = Time.frameCount;

        if (Input.GetMouseButtonUp(0))
        {
            int frame = Time.frameCount;

            if (frame - _clickDownFrame > 5)
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, _clickLayerMask))
            {
                var controller = hit.transform.GetComponent<JengaBoxController>();
                if (controller)
                {
                    ClickCallback(controller);
                }
                else
                {
                    Debug.LogError("Raycast interrupted");
                }
            }
            else
            {
                ClickOutside();
            }

            //RaycastHit[] hits = Physics.RaycastAll(ray, 100,);

            //foreach (var raycastHit in hits)
            //{

            //}
        }
        else
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, _clickLayerMask))
            {
                var controller = hit.transform.GetComponent<JengaBoxController>();
                if (controller)
                {
                    Highlight(controller, true);
                }
                else
                {
                    Debug.LogError("Raycast interrupted");
                }
            }
            else
            {
                Highlight(null, false);
            }
        }
    }


    private void ClickCallback(JengaBoxController controller)
    {
        if (_selected == controller)
        {
            ClickOutside();
            return;
        }
        ClickOutside();
        Debug.Log("Click", this);
        SetHighlighted(null);
        _boxInfoController.Set(controller.Data, ClickOutside);
        _selected = controller;
        controller.SetOutline(1);
    }

    private void Highlight(JengaBoxController controller, bool active)
    {
        if (active)
        {
            if (_selected == null || _selected != controller)
            {
                SetHighlighted(controller);
            }
            else
            {
                SetHighlighted(null);
            }
        }
        else
        {
            if (_selected == null)
            {
                SetHighlighted(null);
            }
            else
            {
                SetHighlighted(null);
            }
        }
    }

    private void ClickOutside()
    {
        Debug.Log("Clicked outside");
        if (_selected != null)
        {
            _boxInfoController.Hide();
            _selected.SetOutline(-1);
            _selected = null;
        }
    }

    private void SetHighlighted(JengaBoxController controller)
    {
        if (_highlighted == controller)
            return;

        if (_highlighted)
            _highlighted.SetOutline(-1);
        _highlighted = null;
        if (!_selected)
            if (controller)
            {
                _highlighted = controller;
                controller.SetOutline(0);
                _boxInfoController.Set(controller.Data, ClickOutside);
            }
            else
            {
                _boxInfoController.Hide();
            }
        else if (controller && _highlightWhileSelected)
        {
            _highlighted = controller;
            controller.SetOutline(0);
        }
    }

    public void Restart()
    {
        if (!_starting && !_started)
        {
            StartCoroutine(Start());
        }
    }

    private void SetLoadingScreen(bool active)
    {
        if (_loadingScreenRoot)
            _loadingScreenRoot.SetActive(active);
    }

    private void SetErrorScreen(bool active)
    {
        if (_errorScreenRoot)
            _errorScreenRoot.SetActive(active);
    }
}