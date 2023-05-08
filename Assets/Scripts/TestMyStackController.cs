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

        _6ThGradeJengaBoxManager.Set(_data, ClickCallback);
        _7ThGradeJengaBoxManager.Set(_data, ClickCallback);
        _8ThGradeJengaBoxManager.Set(_data, ClickCallback);
        SetLoadingScreen(false);

        _started = true;
        _starting = false;
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
                    ClickCallback(controller.Data);
                }
                else
                {
                    Debug.LogError("Raycast interrupted");
                }
            }

            //RaycastHit[] hits = Physics.RaycastAll(ray, 100,);

            //foreach (var raycastHit in hits)
            //{

            //}
        }
    }


    private void ClickCallback(StackApiRequest.StackApiDataElement obj)
    {
        Debug.Log("Click");
        _boxInfoController.Set(obj);
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