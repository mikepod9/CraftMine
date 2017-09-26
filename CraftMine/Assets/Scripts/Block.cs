using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public enum Faces { top, bottom, right, left, front, back};
    
    private string blockType;
    private bool transparent;

    public Block(string type) {
        transparent = type.Equals("Air");
        blockType = type;
    }

    public bool isTransparent() {
        return transparent;
    }

    public string getBlockType() {
        return blockType;
    }

    public MeshData Draw(int[] faces) {
        List<Vector3> blockVertices = MeshData.faces[faces[0]].GetVertices();
        List<int>  blockTriangles = MeshData.faces[faces[0]].GetTriangles();

        for (int i = 1; i < faces.Length; i++) {
            List<Vector3> sideVertices = MeshData.faces[faces[i]].GetVertices();
            blockVertices.AddRange(sideVertices);
            List<int> sideTriangles = MeshData.faces[faces[i]].GetTriangles();
            for (int t = 0; t < sideTriangles.Count; t++) {
                sideTriangles[t] = sideTriangles[t] + (4 * i);
            }
            blockTriangles.AddRange(sideTriangles);
        }

        MeshData mesh = new MeshData(blockVertices, blockTriangles);

        return mesh;
    }
}
