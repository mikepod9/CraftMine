using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float depth = 0;

    private float[,] map;

	// Use this for initialization
	void Update () {
        GameObject quad = GameObject.Find("Quad");
        Renderer renderer = quad.GetComponent<Renderer>();
        GenerateMap();
        renderer.material.mainTexture = GenerateTexture();
	}

    void GenerateMap() {
        map = new float[width, height];

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                map[x, y] = Mathf.PerlinNoise((float)x / width * scale, (float)y / height * scale) + depth;
            }
        }
    }

    Texture GenerateTexture() {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Color color = TerrainType(x,y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    Color TerrainType(int x, int y) {

        float type = map[x, y];
        if(type <= 0.4) {
            return Color.blue;
        } else {
            return Color.green;
        }
    }
}
