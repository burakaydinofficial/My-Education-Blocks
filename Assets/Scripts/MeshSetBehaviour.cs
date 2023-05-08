using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
sealed class MeshSetBehaviour : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshCollider[] _colliders;

    private void Start()
    {
        var mesh = _meshFilter.sharedMesh;
        if (!mesh)
            return;
        foreach (var collider in _colliders)
        {
            if (collider)
                collider.sharedMesh = mesh;
        }
    }
}