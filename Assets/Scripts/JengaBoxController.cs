using System;
using System.Threading.Tasks;
using cakeslice;
using UnityEngine;
using static StackApiRequest;

public class JengaBoxController : MonoBehaviour
{
    [SerializeField] private bool _hideOnAwake = true;
    [SerializeField] private JengaBoxConfig _config;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Outline _outline;
    [SerializeField] private Collider _physicsCollider;
    [SerializeField] private TMPro.TextMeshPro[] _texts;
    [SerializeField] private string[] _masterTexts = new string[]
    {
        "", "LEARNED", "MASTERED"
    };

    public StackApiDataElement Data { get; private set; }
    private Action<JengaBoxController> _clickCallback;

    private void Awake()
    {
        if (_hideOnAwake)
            Hide();
    }

    public void Set(bool kinematic)
    {
        _rigidbody.isKinematic = kinematic;

        if (!kinematic)
        {
            if (Data.mastery == 0)
                Hide();
        }
    }

    public void Set(StackApiDataElement data, Action<JengaBoxController> clickCallback)
    {
        if (!_config)
        {
            Debug.LogError("Config is missing");
        }

        Data = data;
        _clickCallback = clickCallback;

        Set(true);
        gameObject.SetActive(true);
        var text = _masterTexts.Length > data.mastery ? _masterTexts[data.mastery] : "";
        for (var i = 0; i < _texts.Length; i++)
        {
            if (_texts[i])
                _texts[i].text = text;
        }

        var mat = _config.GetMaterial(data.mastery);
        if (mat)
            _renderer.material = mat;
        else
            Debug.LogError("There is no material matching mastery " + data.mastery);
        _rigidbody.mass = _config.GetWeight(data.mastery);
    }

    public void SetOutline(int outline)
    {
        if (!_outline)
            return;

        if (outline < 0)
        {
            _outline.enabled = false;
        }
        else
        {
            _outline.color = outline;
            _outline.enabled = true;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        SetOutline(-1);
    }

    public void Click()
    {
        _clickCallback?.Invoke(this);
    }
}