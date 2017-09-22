using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public static WorldGenerator instance = null;

    public enum colorType {
        Grayscale,
        Colors
    }

    public colorType colortype;

    [Range(2, 128)]
    public int length = 64;

    public int depth = 128;

    [Range(0, 128)]
    public int z = 0;

    public int waterLevel = 64;

    public float frequency = 10f;

    [Range(1, 8)]
    public int octaves = 2;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 2;

    public NoiseMethodType type;

    public Material Air;
    public Material Land;
    public Material Stone;
    public Material Bedrock;

    private Texture2D texture;

    private float[,] noiseMap;

    private void Awake(){
        if (instance == null)
            instance = this;
    }

    private void Start() {
        //SpawnBlock();
        //noiseMap = Noise.GenerateNoiseMap(length, length, depth, this.transform, frequency, octaves, lacunarity, persistence);
        Chunk chunk = new Chunk(0, 0);
        Chunk chunk2 = new Chunk(0, 64);
        //for (int z = 0; z < length; z++)
        //{
        //    for (int x = 0; x < length; x++)
        //    {
        //        for (int y = 50; y < depth; y++)
        //        {
        //            PlaceBlock(noiseMap[z, x], x, y, z);
        //        }
        //    }
        //}
    }

    private void OnEnable() {
        
    }

    private void Update() {
       
    }

    private void PlaceBlock(float noiseSample, int x, int y, int z) {

        int dirtLayer = Mathf.FloorToInt(noiseSample);
        int dirtThickness = dirtLayer / 32 - 6;
        int stoneLayer = dirtLayer + dirtThickness;

        if (y == 0) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Bedrock;
            cube.transform.position = new Vector3(x, y, z);
        } else if (y <= stoneLayer) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Stone;
            cube.transform.position = new Vector3(x, y, z);
        } else if (y <= dirtLayer) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Land;
            cube.transform.position = new Vector3(x, y, z);
        } else return;
    }
}
