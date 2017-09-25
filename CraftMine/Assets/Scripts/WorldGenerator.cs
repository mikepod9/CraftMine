using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    private static WorldGenerator instance = null;

    [Range(2, 128)]
    public int length = 64;

    public int depth = 128;

    public int waterLevel = 64;

    [Range(0, 10)]
    public int numChunks = 0;

    public float frequency = 10f;

    [Range(1, 8)]
    public int octaves = 2;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    public NamedMaterial[] materials;

    public Dictionary<string, Material> materialDictionary = new Dictionary<string, Material>();

    public Chunk[,] chunks;

    [System.Serializable]
    public struct NamedMaterial {
        public string name;
        public Material material;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        PopulateDictionary();
    }

    public void Start() {
        CreateChunks(numChunks);
        RenderChunks();
    }

    private void OnEnable() {
    }

    private void Update() {

    }

    public static WorldGenerator getInstance() {
        return instance;
    }

    public void PopulateDictionary() {
        for (int i = 0; i < materials.Length; i++) {
            materialDictionary.Add(materials[i].name, materials[i].material);
        }
    }

    public void CreateChunks(int numChunks) {
        chunks = new Chunk[1 + 2 * numChunks, 1 + 2 * numChunks];
        int i = 0;
        for (int x = -numChunks; x <= numChunks; x++) {
            for (int z = -numChunks; z <= numChunks; z++) {
                chunks[x + numChunks, z + numChunks] = new Chunk(x, z, i);
                i++;
            }
        }
    }

    public void RenderChunks() {
        for (int x = 0; x < chunks.GetLength(1); x++) {
            for (int z = 0; z < chunks.GetLength(0); z++) {
                chunks[x, z].RenderChunk();
            }
        }
    }
}
