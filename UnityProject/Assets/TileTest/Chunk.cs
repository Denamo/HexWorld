using game;
using logic.util;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer = null;
    public MeshFilter meshFilter = null;
    public MeshCollider meshCollider = null;
    
    void Start()
    {
    }

    public void SetMesh(Mesh mesh)
    {
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public static Chunk Create(Transform parent, string name)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.parent = parent;

        Chunk chunk = gameObject.AddComponent<Chunk>();
        chunk.meshRenderer = gameObject.AddComponent<MeshRenderer>();
        chunk.meshFilter = gameObject.AddComponent<MeshFilter>();
        chunk.meshCollider = gameObject.AddComponent<MeshCollider>();

        MeshRenderer parentRenderer = parent.GetComponent<MeshRenderer>();
        if (parentRenderer)
            chunk.meshRenderer.sharedMaterial = parentRenderer.sharedMaterial;

        return chunk;
    }

}
