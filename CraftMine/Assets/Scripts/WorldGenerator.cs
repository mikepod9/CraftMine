using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public int seed;

    public int width = 256;
    public int height = 256;
    public int maxDepth = 256;
    public float scale = 20f;
    public int depth = 0;

    [Range(1,5)]
    public int octaves = 4;
    [Range(0,1)]
    public float persistance = 0.5f;
    [Range(0, 3)]
    public float lacunarity = 2f;

    private float[,] map;
    
	void Update () {
        GameObject quad = GameObject.Find("Quad");
        Renderer renderer = quad.GetComponent<Renderer>();
        GenerateMap();
        renderer.material.mainTexture = GenerateTexture();
	}

    void GenerateMap() {

        map = new float[width, height];

        Vector2[] octaveOffSets = new Vector2[octaves];
        System.Random prng = new System.Random(seed);
        for (int i =0; i < octaves; i++) {
            float offSetX = prng.Next(-10000, 10000);
            float offSetY = prng.Next(-10000, 10000);
            octaveOffSets[i] = new Vector2(offSetX, offSetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = x / scale * frequency + octaveOffSets[i].x;
                    float sampleY = y / scale * frequency + octaveOffSets[i].y;

                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleY) * maxDepth + depth;
                    noiseHeight += noiseValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                map[x, y] = noiseHeight;
            }
        }
        //for (int y = 0; y < height; y++) {
        //    for (int x = 0; x < width; x++) {
        //        map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
        //    }
        //}

    }

    Texture GenerateTexture() {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Color color = TerrainType(x,y);
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    Color TerrainType(int x, int y) {
        float type = map[x, y];

        if(type <= 0.4 * maxDepth) {
            return Color.blue;
        }
        if (type <= 0.6 * maxDepth) {
            return Color.green;
        }
        if (type <= 0.8 * maxDepth) {
            return Color.black;
        }
        return Color.white;
    }
}
