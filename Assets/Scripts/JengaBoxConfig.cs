using UnityEngine;

[CreateAssetMenu]
public class JengaBoxConfig : ScriptableObject
{
    [SerializeField] private Material[] _materials;

    public Material GetMaterial(int mastery)
    {
        return _materials.Length > mastery ? _materials[mastery] : null;
    }
}