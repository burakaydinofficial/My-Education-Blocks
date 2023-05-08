using UnityEngine;

[CreateAssetMenu]
public class JengaBoxConfig : ScriptableObject
{
    [SerializeField] private Material[] _materials;
    [SerializeField] private float[] _weights;

    public Material GetMaterial(int mastery)
    {
        return _materials.Length > mastery ? _materials[mastery] : null;
    }

    public float GetWeight(int mastery)
    {
        return _weights.Length > mastery ? _weights[mastery] : 1f;
    }
}