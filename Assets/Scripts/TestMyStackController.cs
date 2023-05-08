using System;
using System.Collections;
using UnityEngine;

public class TestMyStackController : MonoBehaviour
{
    private bool _starting = false;
    private bool _started = false;

    private StackApiRequest.StackApiDataElement[] _data;

    [SerializeField] private GameObject _loadingScreenRoot;
    [SerializeField] private GameObject _errorScreenRoot;

    [SerializeField] private JengaBoxManager _6ThGradeJengaBoxManager;
    [SerializeField] private JengaBoxManager _7ThGradeJengaBoxManager;
    [SerializeField] private JengaBoxManager _8ThGradeJengaBoxManager;

    [SerializeField] BoxInfoController _boxInfoController;
    [SerializeField] private LayerMask _clickLayerMask;

    [SerializeField] private bool _highlightWhileSelected = true;

    [NonSerialized] private JengaBoxController _selected;
    [NonSerialized] private JengaBoxController _highlighted;

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

        _6ThGradeJengaBoxManager.Set(_data, ClickCallback);
        _7ThGradeJengaBoxManager.Set(_data, ClickCallback);
        _8ThGradeJengaBoxManager.Set(_data, ClickCallback);
        SetLoadingScreen(false);

        _started = true;
        _starting = false;
    }

    private static int Sorter(StackApiRequest.StackApiDataElement e1, StackApiRequest.StackApiDataElement e2)
    {
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