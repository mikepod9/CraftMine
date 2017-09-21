using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour{

    private WorldGenerator WG = WorldGenerator.instance;

    private int chunkWidth;
    private int chunkLength;
    private int chunkDepth;

    private Block[,,] Blocks;
    private float[,] noiseMap;

    private GameObject chunk;

    public Chunk(int posX, int posZ)
    {
        chunkWidth = WG.length;
        chunkLength = WG.length;
        chunkDepth = WG.depth;
        chunk = new GameObject();
        chunk.AddComponent<Transform>();
        chunk.transform.position = new Vector3(posX, 0, posZ);
        this.noiseMap = Noise.GenerateNoiseMap(chunkWidth, chunkLength, chunkDepth, chunk.transform, WG.frequency, WG.octaves, WG.lacunarity, WG.persistence);
        Start();
    }

    // Use this for initialization
    void Start () {
        GenerateChunk();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateChunk(){
        for (int z = 0; z < chunkWidth; z++){
            for (int x = 0; x < chunkLength; x++){
                for (int y = 50; y < chunkDepth; y++){
                    PlaceBlock(noiseMap[z, x], x, y, z);
                }
            }
        }
    }

    private void PlaceBlock(float noiseSample, int x, int y, int z){

        int dirtLayer = Mathf.FloorToInt(noiseSample);
        int dirtThickness = dirtLayer / 32 - 6;
        int stoneLayer = dirtLayer + dirtThickness;

        if (y == 0)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = WorldGenerator.instance.Bedrock;
            cube.transform.position = new Vector3(x, y, z);
        }
        else if (y <= stoneLayer)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = WorldGenerator.instance.Stone;
            cube.transform.position = new Vector3(x, y, z);
        }
        else if (y <= dirtLayer)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = WorldGenerator.instance.Land;
            cube.transform.position = new Vector3(x, y, z);
        }
        else return;
    }
}
