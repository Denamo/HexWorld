using System.Collections.Generic;
using UnityEngine;

public class DynamicMesh
{
    public List<Vector3> vertices;
    public List<Vector3> normals;
    public List<int> triangles;
    public List<Color32> colors;
    public List<Vector2> uv;

    public DynamicMesh()
    {
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<int>();
        colors = new List <Color32>();
        uv = new List<Vector2>();
    }

    public void SimpleTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        SimpleTriangle(a, b, c, Color.white, Color.white, Color.white);
    }

    public void SimpleTriangle(Vector3 a, Vector3 b, Vector3 c, Color32 aColor, Color32 bColor, Color32 cColor )
	{
		SimpleTriangle(a, b, c, aColor, bColor, cColor, Vector3.up, Vector3.up, Vector3.up);
	}

	public void SimpleTriangle(Vector3 a, Vector3 b, Vector3 c, Color32 aColor, Color32 bColor, Color32 cColor, Vector3 aNormal, Vector3 bNormal, Vector3 cNormal)
	{
		int index = vertices.Count;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        colors.Add(aColor);
        colors.Add(bColor);
        colors.Add(cColor);

        uv.Add(new Vector2(a.x, a.z));
        uv.Add(new Vector2(b.x, b.z));
        uv.Add(new Vector2(c.x, c.z));

        normals.Add(aNormal);
        normals.Add(bNormal);
        normals.Add(cNormal);

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);
    }

    public Mesh Create()
    {
        Mesh mesh = new Mesh();
        if (vertices.Count > 65535)
        {
            Debug.LogWarning("Vertex count is high, forced to use 32 bit index! Try changeing World.CHUNK_SIZE");
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors32 = colors.ToArray();
        mesh.uv = uv.ToArray();

        mesh.RecalculateBounds();

        return mesh;
    }

}
