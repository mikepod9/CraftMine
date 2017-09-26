using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk{

    private WorldGenerator WG = WorldGenerator.getInstance();

    private int chunkWidth;
    private int chunkLength;
    private int chunkDepth;
    public Vector3 chunkGridPosition;
    public Vector3 chunkWorldPosition;

    public Block[,,] blocks;
    private float[,] noiseMap;

    public GameObject chunk;
    public int chunkID;
    private int blockCount = 0;

    private Dictionary<string, List<MeshData>> meshesPerBlockType = new Dictionary<string, List<MeshData>>();
    public List<MeshMaterial> finalMeshes = new List<MeshMaterial>();

    public Chunk(int x, int z, int i) {
        chunkWidth = WG.length;
        chunkLength = WG.length;
        chunkDepth = WG.depth;
        blocks = new Block[chunkWidth, chunkDepth, chunkLength];
        chunkID = i;
        chunkGridPosition = new Vector3(x, 0, z);
        chunkWorldPosition = new Vector3(x * chunkLength, 0, z * chunkWidth);
        noiseMap = Noise.GenerateNoiseMap(chunkWidth, chunkLength, chunkDepth, chunkGridPosition, WG.frequency, WG.octaves, WG.lacunarity, WG.persistence);
        GenerateChunk();
        lock (WG.chunksToAddtoGridQueue) { 
            WG.chunksToAddtoGridQueue.Enqueue(this);
        }
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

    public void RenderChunk() { 
        for (int z = 0; z < chunkWidth; z++) {
            for (int x = 0; x < chunkLength; x++) {
                for (int y = 0; y < chunkDepth; y++) {
                    if (!blocks[x, y, z].isTransparent()) {
                        int[] faces = CheckNeighbors(x, y, z);
                        if (faces.Length > 0) { 
                            MeshData block = blocks[x, y, z].Draw(faces);
                            Vector3[] blockVertices = block.GetVertices().ToArray();
                            for (int i = 0; i < blockVertices.Length; i++) {
                                blockVertices[i] += new Vector3(x + (int)chunkWorldPosition.x, y, z + (int)chunkWorldPosition.z);
                            }
                            block.SetVertices(new List<Vector3>( blockVertices));
                            meshesPerBlockType[blocks[x, y, z].getBlockType()].Add(block);
                        }
                    }
                }
            }
        }
        CombineMeshesByType();
        lock (WG.chunksToRenderQueue) {
            WG.chunksToRenderQueue.Enqueue(this);
        }
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
            meshesPerBlockType.Add(block.getBlockType(), new List<MeshData>());
        }
    }

    private int [] CheckNeighbors(int x, int y, int z) {
        List<int> faces = new List<int>();

        if (x == chunkLength - 1)
            if (getNeighbourChunkBlocks(1, 0) != null) {
                if (getNeighbourChunkBlocks(1, 0)[0, y, z].isTransparent())
                    faces.Add((int)Block.Faces.right);
            } else {
                faces.Add((int)Block.Faces.right);
            }
        if (y == chunkDepth - 1)
            faces.Add((int)Block.Faces.top);
        if (z == chunkWidth - 1)
            if (getNeighbourChunkBlocks(0, 1) != null) {
                if (getNeighbourChunkBlocks(0, 1)[x, y, 0].isTransparent())
                    faces.Add((int)Block.Faces.front);
            } else {
                faces.Add((int)Block.Faces.front);
            }

        if (x == 0)
            if (getNeighbourChunkBlocks(-1, 0) != null) {
                if (getNeighbourChunkBlocks(-1, 0)[WG.length - 1, y, z].isTransparent())
                    faces.Add((int)Block.Faces.left);
            } else {
                faces.Add((int)Block.Faces.left);
            }
        if (y == 0)
            faces.Add((int)Block.Faces.bottom);
        if (z == 0)
            if (getNeighbourChunkBlocks(0, -1) != null) {
                if (getNeighbourChunkBlocks(0, -1)[x, y, WG.length - 1].isTransparent())
                    faces.Add((int)Block.Faces.back);
            } else {
                faces.Add((int)Block.Faces.back);
            }

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

    private Block[,,] getNeighbourChunkBlocks(int x, int z) {
        int chunkGridCoordX = (int)chunkGridPosition.x;
        int chunkGridCoordZ = (int)chunkGridPosition.z;

        chunkGridCoordX = chunkGridCoordX + WG.numChunks + x;
        chunkGridCoordZ = chunkGridCoordZ + WG.numChunks + z;

        try {
            return WG.chunks[chunkGridCoordX, chunkGridCoordZ].blocks;
        }catch(System.Exception e) {
            return null;
        }
    }

    public void CombineMeshesByType() {
        foreach(string blockType in meshesPerBlockType.Keys) {
            if (blockType.Equals("Air"))
                continue;

            List<MeshData> meshes = meshesPerBlockType[blockType];
            List<Vector3> finalMeshVercies = new List<Vector3>();
            List<int> finalMeshTriangles = new List<int>();
            
            if (meshes.Count == 0)
                continue;
            finalMeshVercies.AddRange(meshes[0].GetVertices());
            finalMeshTriangles.AddRange(meshes[0].GetTriangles());
            int offset = meshes[0].GetVertices().Count;

            for (int i = 1; i < meshes.Count; i++) {
                finalMeshVercies.AddRange(meshes[i].GetVertices());
                int[] meshTriangles = meshes[i].GetTriangles().ToArray();
                for (int t = 0; t < meshTriangles.Length; t++) {
                    meshTriangles[t] += offset;
                }
                finalMeshTriangles.AddRange(meshTriangles);
                offset += meshes[i].GetVertices().Count;
            }

            MeshData finalMesh = new MeshData(finalMeshVercies, finalMeshTriangles);
            finalMeshes.Add(new MeshMaterial(finalMesh, WG.materialDictionary[blockType]));
        }
    }

    public struct MeshMaterial {
        public readonly MeshData mesh;
        public readonly Material material;

        public MeshMaterial(MeshData mesh, Material material) {
            this.mesh = mesh;
            this.material = material;
        }
    }
}
