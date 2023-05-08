using UnityEngine;

public class BoxInfoController : MonoBehaviour
{
    [SerializeField] private bool _hideOnAwake = true;

    private StackApiRequest.StackApiDataElement _data;

    private void Awake()
    {
        if (_hideOnAwake)
            Hide();
    }

    public void Set(StackApiRequest.StackApiDataElement data)
    {
        _data = data;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}