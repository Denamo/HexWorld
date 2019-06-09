using game;
using logic.util;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject cursor;

    public const int CHUNK_SIZE = 16;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Mesh mesh;
    MeshCollider meshCollider = null;
    Material material;

    public LogicWorldConfig config = new LogicWorldConfig();

    public LogicWorld world = null;

    void Start()
    {
        world = new LogicWorld();

        world.Start(config);

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        material = meshRenderer.material;
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        
        Render();
    }

    public void UpdateWorld()
    {
        updateWorld = true;
    }

    bool updateWorld;
    int seedStepVelocity = 0;
    void Update()
    {
        /*if (Input.GetKey(KeyCode.LeftArrow))
        {
            seedStepVelocity -= 100;
            if (seedStepVelocity < -5000)
                seedStepVelocity = -5000;

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            seedStepVelocity += 100;
            if (seedStepVelocity > 5000)
                seedStepVelocity = 5000;
        }
        else
        {
            seedStepVelocity = seedStepVelocity / 2;
        }*/

        if (seedStepVelocity != 0)
        {
            updateWorld = true;
            config.seed += seedStepVelocity;
        }

        if (updateWorld)
        {
            updateWorld = false;
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

        { //Mesh creation (splitting in chunks, triangulation etc.)
            int widthInChunks = renderTiles.width / CHUNK_SIZE;
            int heightInChunks = renderTiles.height / CHUNK_SIZE;

            if (chunks == null)
            {
                chunks = new LogicArray2<Chunk>(widthInChunks, heightInChunks, null);
            }

            for (int cy = 0; cy < heightInChunks; ++cy)
            {
                for (int cx = 0; cx < widthInChunks; ++cx)
                {
                    DynamicMesh dynamicMesh = new DynamicMesh();

                    //Chunk bounds
                    int startY = cy * CHUNK_SIZE;
                    int endY = startY + CHUNK_SIZE;
                    int startX = cx * CHUNK_SIZE;
                    int endX = startX + CHUNK_SIZE;

                    if (cx == widthInChunks - 1)
                        endX--;

                    if (cy == widthInChunks - 1)
                        endY--;


                    for (int y = startY; y < endY; ++y)
                    {
                        for (int x = startX; x < endX; ++x)
                        {
                            RenderTile tile0 = renderTiles.Get(x, y);
                            RenderTile tile1 = renderTiles.Get(x + 1, y);
                            RenderTile tile2 = renderTiles.Get(x, y + 1);
                            RenderTile tile3 = renderTiles.Get(x + 1, y + 1);

                            RenderTriangle(dynamicMesh, tile0, tile1, tile2);
                            RenderTriangle(dynamicMesh, tile2, tile1, tile3);
                        }
                    }

                    Mesh mesh = dynamicMesh.Create();
                    Chunk chunk = GetChunk(cy, cx);
                    chunk.SetMesh(mesh);
                }
            }
        }
        
        //Debug.Log("Triangles:" + dynamicMesh.triangles.Count + "Vertices:" + dynamicMesh.vertices.Count);

        //Mesh mesh = dynamicMesh.Create();

        //meshFilter.mesh = mesh;

        //meshCollider.sharedMesh = mesh;
        //Debug.Log("NavigationBlob::MeshUpdate duration=" + (Time.realtimeSinceStartup - timer));
        

    }

    private LogicArray2<Chunk> chunks;
    private Chunk GetChunk(int cx, int cy)
    {
        Chunk chunk = chunks.Get(cx, cy);
        if(chunk==null)
        {
            chunk = Chunk.Create(transform, string.Format("Chunk {0}:{1}", cx, cy));
            chunks.Set(cx, cy, chunk);
        }

        return chunk;
    }

    private void DestroyAllChunks()
    {
        for(int i = 0; i < chunks.array.Count; ++i)
        {
            Chunk chunk = chunks.array[i];
            if (chunk != null)
            {
                Destroy(chunk.gameObject);
            }
        }
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
        return new Vector3(p.x * tileScaleFactor, p.y * tileScaleFactor - 14f, p.z * tileScaleFactor);
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

