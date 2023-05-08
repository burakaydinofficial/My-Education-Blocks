using System;
using System.Threading.Tasks;
using UnityEngine;
using static StackApiRequest;

public class JengaBoxController : MonoBehaviour
{
    [SerializeField] private bool _hideOnAwake = true;
    [SerializeField] private JengaBoxConfig _config;
    [SerializeField] private MeshRenderer _renderer;

    private StackApiDataElement _data;
    private Action<StackApiDataElement> _clickCallback;

    private void Awake()
    {
        if (_hideOnAwake)
            Hide();
    }

    public void Set(StackApiDataElement data, Action<StackApiDataElement> clickCallback)
    {
        if (!_config)
        {
            Debug.LogError("Config is missing");
        }

        _data = data;
        _clickCallback = clickCallback;

        gameObject.SetActive(true);
        var mat = _config.GetMaterial(data.mastery);
        if (mat)
            _renderer.materials[0] = mat;
        else
            Debug.LogError("There is no material matching mastery " + data.mastery);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Click()
    {
        _clickCallback?.Invoke(_data);
    }
}