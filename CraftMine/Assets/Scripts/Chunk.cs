using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk{

    private WorldGenerator WG = WorldGenerator.getInstance();

    private int chunkWidth;
    private int chunkLength;
    private int chunkDepth;

    private Block[,,] blocks;
    private float[,] noiseMap;

    private GameObject chunk;
    private int blockCount = 0;

    private Dictionary<string, List<Mesh>> meshesPerBlockType = new Dictionary<string, List<Mesh>>();

    public Chunk(int posX, int posZ, int num)
    {
        chunkWidth = WG.length;
        chunkLength = WG.length;
        chunkDepth = WG.depth;
        blocks = new Block[chunkWidth, chunkDepth, chunkLength];
        chunk = new GameObject("Chunk" + num);
        chunk.tag = "Chunk";
        chunk.transform.position = new Vector3(posX, 0, posZ);
        this.noiseMap = Noise.GenerateNoiseMap(chunkWidth, chunkLength, chunkDepth, chunk.transform, WG.frequency, WG.octaves, WG.lacunarity, WG.persistence);
        chunk.transform.position = new Vector3(posX * chunkLength, 0, posZ * chunkWidth);
        GenerateChunk();
        RenderChunk();
    }

    void GenerateChunk(){
        for (int z = 0; z < chunkWidth; z++){
            for (int x = 0; x < chunkLength; x++){
                for (int y = 0; y < chunkDepth; y++){
                    PlaceBlock(noiseMap[z, x], x, y, z);
                }
            }
        }
    }

    void RenderChunk() { 
        for (int z = 0; z < chunkWidth; z++) {
            for (int x = 0; x < chunkLength; x++) {
                for (int y = 0; y < chunkDepth; y++) {
                    if (!blocks[x, y, z].isTransparent()) {
                        int[] faces = CheckNeighbors(x, y, z);
                        if (faces.Length > 0) { 
                            Mesh block = blocks[x, y, z].Draw(faces);
                            Vector3[] blockVertices = block.vertices;
                            for (int i = 0; i < blockVertices.Length; i++) {
                                blockVertices[i] += new Vector3(x + (int)chunk.transform.position.x, y, z + (int)chunk.transform.position.z);
                            }
                            block.vertices = blockVertices;
                            meshesPerBlockType[blocks[x, y, z].getBlockType()].Add(block);
                        }
                    }
                }
            }
        }
        Debug.Log(blockCount + " in " + chunk.name);
        CombineMeshesByType();
    }

    private void PlaceBlock(float noiseSample, int x, int y, int z){

        int dirtLayer = Mathf.FloorToInt(noiseSample);
        int dirtThickness = dirtLayer / 32 - 6;
        int stoneLayer = dirtLayer + dirtThickness;

        Block block = new Block("Air");
        blocks[x, y, z] = block;

        if (y == 0) {
            block = new Block("Bedrock");
            blocks[x, y, z] = block;
            blockCount++;
        } else if (y <= stoneLayer) {
            block = new Block("Stone");
            blocks[x, y, z] = block;
            blockCount++;
        } else if (y <= dirtLayer) {
            block = new Block("Land");
            blocks[x, y, z] = block;
            blockCount++;
        }
        if (!meshesPerBlockType.ContainsKey(block.getBlockType())) {
            meshesPerBlockType.Add(block.getBlockType(), new List<Mesh>());
        }
    }

    private int [] CheckNeighbors(int x, int y, int z) {
        List<int> faces = new List<int>();

        if (x == chunkLength - 1)
            faces.Add((int)Block.Faces.right);
        if (y == chunkDepth - 1)
            faces.Add((int)Block.Faces.top);
        if (z == chunkWidth - 1)
            faces.Add((int)Block.Faces.front);

        if (x == 0)
            faces.Add((int)Block.Faces.left);
        if (y == 0)
            faces.Add((int)Block.Faces.bottom);
        if (z == 0)
            faces.Add((int)Block.Faces.back);

        if (y + 1 < chunkDepth && blocks[x, y + 1, z].isTransparent())
            faces.Add((int)Block.Faces.top);
        if (y - 1 >= 0 && blocks[x, y - 1, z].isTransparent())
            faces.Add((int)Block.Faces.bottom);

        if (x + 1 < chunkLength && blocks[x + 1, y, z].isTransparent())
            faces.Add((int)Block.Faces.right);
        if (x - 1 >= 0 && blocks[x - 1, y, z].isTransparent())
            faces.Add((int)Block.Faces.left);

        if (z + 1 < chunkWidth && blocks[x, y, z + 1].isTransparent())
            faces.Add((int)Block.Faces.front);
        if (z - 1 >= 0 && blocks[x, y, z - 1].isTransparent())
            faces.Add((int)Block.Faces.back);

        return faces.ToArray();
    }

    public void CombineMeshesByType() {
        foreach(string blockType in meshesPerBlockType.Keys) {
            if (blockType.Equals("Air"))
                continue;

            GameObject meshType = new GameObject(blockType + " blocks");

            List<Mesh> meshes = meshesPerBlockType[blockType];
            List<Vector3> finalMeshVercies = new List<Vector3>();
            List<int> finalMeshTriangles = new List<int>();

            //Debug.Log(meshes[0].vertices.Length);

            finalMeshVercies.AddRange(meshes[0].vertices);
            finalMeshTriangles.AddRange(meshes[0].triangles);
            int offset = meshes[0].vertices.Length;

            for (int i = 1; i < meshes.Count; i++) {
                finalMeshVercies.AddRange(meshes[i].vertices);
                int[] meshTriangles = meshes[i].triangles;
                for (int t = 0; t < meshTriangles.Length; t++) {
                    meshTriangles[t] += offset;
                }
                finalMeshTriangles.AddRange(meshTriangles);
                offset += meshes[i].vertices.Length;
            }

            Mesh finalMesh = new Mesh {
                vertices = finalMeshVercies.ToArray(),
                triangles = finalMeshTriangles.ToArray()
            };
            finalMesh.RecalculateNormals();
            meshType.AddComponent<MeshFilter>().sharedMesh = finalMesh;
            meshType.AddComponent<MeshRenderer>().material = WG.materialDictionary[blockType];
            meshType.transform.parent = chunk.transform;
        }
    }
}
