using game;
using logic.util;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject cursor;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Mesh mesh;
    MeshCollider meshCollider = null;
    Material material;
    
    public LogicWorld world = new LogicWorld();

    void Start()
    {
        world.Start();

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        material = meshRenderer.material;
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        
        Render();
    }

    int seedStepVelocity = 0;
    void Update()
    {

        bool updateWorld = false;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            updateWorld = true;
            world.StepForward();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            updateWorld = true;
            world.StepBackward();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            seedStepVelocity -= 100;
            if (seedStepVelocity < -10000)
                seedStepVelocity = -10000;

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            seedStepVelocity += 100;
            if (seedStepVelocity > 10000)
                seedStepVelocity = 10000;
        }
        else
        {
            seedStepVelocity = seedStepVelocity / 2;
        }

        if (seedStepVelocity != 0)
        {
            updateWorld = true;
            world.SetSeed(world.GetSeed() + seedStepVelocity);
        }

        if (updateWorld)
        {
            world.UpdateMap();
            Render();
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (meshCollider!=null)
            {
                RaycastHit hit;

                if (meshCollider.Raycast(ray, out hit,1000f))
                {
                    if (cursor != null)
                        cursor.transform.position = hit.point;
                    LogicHex hex = WorldToHexPos(hit.point);
                    //Debug.Log(hit.point + " => " + WorldToHexPos(hit.point));

                    if(Input.GetMouseButton(0))
                        world.TouchHex(hex);
                    else
                        world.Touch2Hex(hex);

                    Render();
                }

            }

        }

		
	}
    
	void Render()
	{
        LogicArray2<int> tiles = world.map;

		LogicArray2<RenderTile> renderTiles = new LogicArray2<RenderTile>(tiles.width,tiles.height,null);

		DynamicMesh dynamicMesh = new DynamicMesh();

		{ //Vertex creation
			int index = 0;
			for (int y = 0; y < tiles.height; ++y)
			{
				for (int x = 0; x < tiles.width; ++x)
				{
					int tile = tiles.Get(x, y);
					Vector3 pos = HexToWorldPos(new LogicHex(x, y));

					Color32 color = TerrainTypeToColor(LogicTile.GetTerrain(tile));
					renderTiles.Set(x, y, new RenderTile(tile, pos, Vector3.zero, color));

					++index;
				}
			}
		}

		{ //Smooth normal calculations
			int index = 0;
			for (int y = 1; y < renderTiles.height - 1; ++y) //Skip edge tiles
			{
				for (int x = 1; x < renderTiles.width - 1; ++x)
				{
					RenderTile tile = renderTiles.Get(x, y);

					Vector3 p0 = tile.position;
					for (int h=0; h < 6; ++h)
					{
                        LogicHex dir1 = LogicHex.Direction(h);
                        LogicHex dir2 = LogicHex.Direction(h + 1);

                        Vector3 p1 = renderTiles.Get(x + dir1.q, y + dir1.r).position;
						Vector3 p2 = renderTiles.Get(x + dir2.q, y + dir2.r).position;

						Vector3 side1 = p1 - p0;
						Vector3 side2 = p2 - p0;
						Vector3 perp = Vector3.Cross(side1, side2);

						//float perpLength = perp.magnitude;
						//perp /= perpLength;

						tile.normal += perp;
					}

					float normLength = tile.normal.magnitude;
					tile.normal /= normLength;

					++index;
				}
			}
		}
		
		{ //Triangle creation
			int index = 0;
			for (int y = 0; y < renderTiles.height - 1; ++y)
			{
				for (int x = 0; x < renderTiles.width - 1; ++x)
				{
					RenderTile tile0 = renderTiles.Get(x, y);
					RenderTile  tile1 = renderTiles.Get(x + 1, y);
					RenderTile  tile2 = renderTiles.Get(x, y + 1);
					RenderTile tile3 = renderTiles.Get(x + 1, y + 1);

                    RenderTriangle(dynamicMesh, tile0, tile1, tile2);
                    RenderTriangle(dynamicMesh, tile2, tile1, tile3);

					++index;
				}
			}

		}
		
        Mesh mesh = new Mesh();
        mesh.vertices = dynamicMesh.vertices.ToArray();
        mesh.normals = dynamicMesh.normals.ToArray();
        mesh.triangles = dynamicMesh.triangles.ToArray();
        mesh.colors32 = dynamicMesh.colors.ToArray();
        mesh.uv = dynamicMesh.uv.ToArray();

        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

        meshCollider.sharedMesh = mesh;
        //Debug.Log("NavigationBlob::MeshUpdate duration=" + (Time.realtimeSinceStartup - timer));

    }

    public static void RenderTriangle(DynamicMesh mesh, RenderTile a, RenderTile b, RenderTile c)
    {
        TerrainType tA = a.GetTerrain();
        TerrainType tB = b.GetTerrain();
        TerrainType tC = c.GetTerrain();

        if (tA == tB && tA == tC)
        {
            //No terrain transition
            mesh.SimpleTriangle(a.position, b.position, c.position, a.color, b.color, c.color, a.normal, b.normal, c.normal);
        }
        else
        {
            //NOTE: Currently I assume height goes: sand, dirt, grass. Height transitions could also be added

            //Terrain transition. TODO: cache all combination
            if (tB == tC)
            {
                //A unique
                if(tA == TerrainType.Grass && tB == TerrainType.Dirt)
                {
                    GrassToDirt(mesh, a, b, c);
                    return;
                }
                else if(tA == TerrainType.Dirt && tB == TerrainType.Grass)
                {
                    DirtToGrass(mesh, a, b, c);
                    return;
                }
            }
            else if(tA == tC)
            {
                //B unique
                if (tB == TerrainType.Grass && tC == TerrainType.Dirt)
                {
                    GrassToDirt(mesh, b, c, a);
                    return;
                }
                else if (tB == TerrainType.Dirt && tC == TerrainType.Grass)
                {
                    DirtToGrass(mesh, b, c, a);
                    return;
                }
            }
            else if(tA == tB)
            {
                //C unique
                if (tC == TerrainType.Grass && tA == TerrainType.Dirt)
                {
                    GrassToDirt(mesh, c, a, b);
                    return;
                }
                else if (tC == TerrainType.Dirt && tA == TerrainType.Grass)
                {
                    DirtToGrass(mesh, c, a, b);
                    return;
                }
            }
            else
            {
                //All unique

            }

            mesh.SimpleTriangle(a.position, b.position, c.position, a.color, b.color, c.color, a.normal, b.normal, c.normal);
        }

    }

    //a==grass
    public static void GrassToDirt(DynamicMesh mesh, RenderTile a, RenderTile b, RenderTile c)
    {
        Vector3 abMid = (a.position + b.position) * 0.5f;
        Vector3 acMid = (a.position + c.position) * 0.5f;

        Vector3 abGround = new Vector3(abMid.x, b.position.y, abMid.z);
        Vector3 acGround = new Vector3(acMid.x, c.position.y, acMid.z);


        mesh.SimpleTriangle(a.position, abMid, acMid, a.color, a.color, a.color, a.normal, a.normal, a.normal);

        mesh.SimpleTriangle(acMid, abMid, abGround, a.color, a.color, b.color, c.normal, b.normal, b.normal);
        mesh.SimpleTriangle(acMid, abGround, acGround, a.color, b.color, c.color, c.normal, b.normal, c.normal);

        mesh.SimpleTriangle(acGround, abGround, b.position, c.color, b.color, b.color, c.normal, b.normal, b.normal);
        mesh.SimpleTriangle(acGround, b.position, c.position, c.color, b.color, c.color, c.normal, b.normal, c.normal);

    }

    //a==dirt
    public static void DirtToGrass(DynamicMesh mesh, RenderTile a, RenderTile b, RenderTile c)
    {
        Vector3 abMid = (a.position + b.position) * 0.5f;
        Vector3 acMid = (a.position + c.position) * 0.5f;

        Vector3 abGround = new Vector3(abMid.x, a.position.y, abMid.z);
        Vector3 acGround = new Vector3(acMid.x, a.position.y, acMid.z);

        mesh.SimpleTriangle(a.position, abGround, acGround, a.color, a.color, a.color, a.normal, a.normal, a.normal);

        mesh.SimpleTriangle(acGround, abGround, abMid, a.color, a.color, b.color, c.normal, b.normal, b.normal);
        mesh.SimpleTriangle(acGround, abMid, acMid, a.color, b.color, c.color, c.normal, b.normal, c.normal);

        mesh.SimpleTriangle(acMid, abMid, b.position, c.color, b.color, b.color, c.normal, b.normal, b.normal);
        mesh.SimpleTriangle(acMid, b.position, c.position, c.color, b.color, c.color, c.normal, b.normal, c.normal);
    }


    static Color32 mainTextureColor = new Color32(0, 0, 0, 0);
    static Color32 secondTextureColor = new Color32(255, 0, 0, 0);
    static Color32 thirdTextureColor = new Color32(0, 255, 0, 0);
    static Color32 fourthTextureColor = new Color32(0, 0, 255, 0);
    
    static Color32 TerrainTypeToColor(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.Sand:
                return secondTextureColor;
            case TerrainType.Dirt:
                return thirdTextureColor;
            case TerrainType.Grass:
                return fourthTextureColor;
            case TerrainType.Rock:
            default:
                return mainTextureColor;
        }
    }


    const float tileScaleFactor = 1.0f/LogicWorld.TILE_SIZE;

    Vector3 HexToWorldPos(LogicHex hex)
    {
        LogicPoint3 p = world.HexToLogicPos(hex);
        return new Vector3(p.x * tileScaleFactor, p.y * tileScaleFactor, p.z * tileScaleFactor);
    }

    LogicHex WorldToHexPos(Vector3 pos)
    {
        LogicPoint3 p = new LogicPoint3( (int)(pos.x/tileScaleFactor) + LogicWorld.HALF_TILE_SIZE, (int)(pos.y/tileScaleFactor) + LogicWorld.HALF_TILE_SIZE, (int)(pos.z/tileScaleFactor) + LogicWorld.HALF_TILE_SIZE);
        return LogicWorld.LogicToHexPos(p);
    }


}

public class RenderTile
{
	public int tile;
	public Vector3 position;
	public Vector3 normal;
	public Color32 color;

    public TerrainType GetTerrain()
    {
        return LogicTile.GetTerrain(tile);
    }

	public RenderTile(int tile, Vector3 position, Vector3 normal, Color32 color)
	{
		this.tile = tile;
		this.position = position;
		this.normal = normal;
		this.color = color;
	}
}

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


}
