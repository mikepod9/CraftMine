using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public Queue<Chunk> chunksToRenderQueue = new Queue<Chunk>();
    public Queue<Chunk> chunksToAddtoGridQueue = new Queue<Chunk>();

    private int generatedChunks;
    private int totalNumOfChunks;

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
        generatedChunks = 0;
        totalNumOfChunks = (numChunks * 2 + 1) * (numChunks * 2 + 1);
        new Thread(() => { CreateChunks(numChunks); }).Start();
    }

    private void OnEnable() {
    }

    private void Update() {
        if (chunksToAddtoGridQueue.Count > 0) {
            Chunk chunk = chunksToAddtoGridQueue.Dequeue();
            chunk.chunk = new GameObject("Chunk" + chunk.chunkID);
            chunk.chunk.tag = "Chunk";
            chunk.chunk.transform.position = chunk.chunkWorldPosition;
            chunks[(int)chunk.chunkGridPosition.x + numChunks, (int)chunk.chunkGridPosition.z + numChunks] = chunk;
            Debug.Log(chunk.chunk.name + " is done generating data");
            generatedChunks++;
        }

        if (generatedChunks == totalNumOfChunks) {
            generatedChunks = -1;
            new Thread(() => { RenderChunks(); }).Start();
        }

        if (chunksToRenderQueue.Count > 0) {
            Chunk chunk = chunksToRenderQueue.Dequeue();
            foreach(Chunk.MeshMaterial meshmat in chunk.finalMeshes) {
                GameObject meshType = new GameObject(meshmat.material.name + " blocks");
                Mesh mesh = new Mesh {
                    vertices = meshmat.mesh.GetVertices().ToArray(),
                    triangles = meshmat.mesh.GetTriangles().ToArray()
                };
                mesh.RecalculateNormals();
                meshType.AddComponent<MeshFilter>().sharedMesh = mesh;
                meshType.AddComponent<MeshRenderer>().material = meshmat.material;
                meshType.transform.parent = chunk.chunk.transform;
            }
        }
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
                new Chunk(x, z, i);
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
