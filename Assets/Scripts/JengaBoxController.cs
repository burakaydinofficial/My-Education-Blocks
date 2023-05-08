using System;
using System.Threading.Tasks;
using UnityEngine;
using static StackApiRequest;

public class JengaBoxController : MonoBehaviour
{
    [SerializeField] private bool _hideOnAwake = true;
    [SerializeField] private JengaBoxConfig _config;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Rigidbody _rigidbody;

    public StackApiDataElement Data { get; private set; }
    private Action<StackApiDataElement> _clickCallback;

    private void Awake()
    {
        if (_hideOnAwake)
            Hide();
    }

    public void Set(bool kinematic)
    {
        _rigidbody.isKinematic = kinematic;
    }

    public void Set(StackApiDataElement data, Action<StackApiDataElement> clickCallback)
    {
        if (!_config)
        {
            Debug.LogError("Config is missing");
        }

        Data = data;
        _clickCallback = clickCallback;

        Set(true);
        gameObject.SetActive(true);
        var mat = _config.GetMaterial(data.mastery);
        if (mat)
            _renderer.material = mat;
        else
            Debug.LogError("There is no material matching mastery " + data.mastery);
        _rigidbody.mass = _config.GetWeight(data.mastery);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Click()
    {
        _clickCallback?.Invoke(Data);
    }
}