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

    public Chunk(int posX, int posZ)
    {
        chunkWidth = WG.length;
        chunkLength = WG.length;
        chunkDepth = WG.depth;
        blocks = new Block[chunkWidth, chunkDepth, chunkLength];
        chunk = new GameObject("Chunk");
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
                            GameObject block = blocks[x, y, z].Draw(faces);
                            block.transform.position = new Vector3(x + (int)chunk.transform.position.x, y, z + (int)chunk.transform.position.z);
                            block.transform.parent = chunk.transform;
                        }
                    }
                }
            }
        }
    }

    private void PlaceBlock(float noiseSample, int x, int y, int z){

        int dirtLayer = Mathf.FloorToInt(noiseSample);
        int dirtThickness = dirtLayer / 32 - 6;
        int stoneLayer = dirtLayer + dirtThickness;

        if (y == 0) {
            Block block = new Block("Bedrock");
            blocks[x, y, z] = block;
        } else if (y <= stoneLayer) {
            Block block = new Block("Stone");
            blocks[x, y, z] = block;
        } else if (y <= dirtLayer) {
            Block block = new Block("Land");
            blocks[x, y, z] = block;
        } else {
            Block block = new Block("Air");
            blocks[x, y, z] = block;
        };        
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
}
