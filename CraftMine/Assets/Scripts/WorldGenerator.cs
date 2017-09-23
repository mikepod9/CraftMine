using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    private static WorldGenerator instance = null;

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

    public NamedMaterial[] materials;

    public Dictionary<string, Material> materialDictionary = new Dictionary<string, Material>();

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

    private void Start() {
       new Chunk(0, 0);
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
}
