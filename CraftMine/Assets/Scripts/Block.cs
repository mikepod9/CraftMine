using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public enum Faces { top, bottom, right, left, front, back};

    private Material material;
    private bool transparent;

    private GameObject block;

    public Block(string type) {
        material = WorldGenerator.getInstance().materialDictionary[type];
        transparent = type.Equals("Air");
        //Debug.Log("Block of type " + type + " is transparent? " + transparent);
    }

    public bool isTransparent() {
        return transparent;
    }

    public GameObject Draw(int[] faces) {
        block = new GameObject();
        for (int i = 0; i < faces.Length; i++) {
            GameObject face = new GameObject();
            face.AddComponent<MeshFilter>();
            face.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh {
                vertices = MeshData.faces[faces[i]].GetVertices().ToArray(),
                triangles = MeshData.faces[faces[i]].GetTriangles()
            };
            face.GetComponent<MeshFilter>().sharedMesh = mesh;
            mesh.RecalculateNormals();
            face.GetComponent<MeshRenderer>().material = material;
            face.transform.parent = block.transform;
        }
        return block;
    }
}
