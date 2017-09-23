using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public enum Faces { top, bottom, right, left, front, back};

    private Material material;
    private bool transparent;

    public Block(string type) {
        material = WorldGenerator.getInstance().materialDictionary[type];
        transparent = type.Equals("Air");
        //Debug.Log("Block of type " + type + " is transparent? " + transparent);
    }

    public bool isTransparent() {
        return transparent;
    }

    public GameObject Draw(int[] faces) {
        GameObject block = new GameObject(material.name + " Block");
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

        block.AddComponent<MeshFilter>();
        block.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh {
            vertices = blockVertices.ToArray(),
            triangles = blockTriangles.ToArray()
        };
        mesh.RecalculateNormals();
        block.GetComponent<MeshFilter>().sharedMesh = mesh;
        block.GetComponent<MeshRenderer>().material = material;

        //for (int i = 0; i < faces.Length; i++) {
        //    GameObject face = new GameObject();
        //    face.AddComponent<MeshFilter>();
        //    face.AddComponent<MeshRenderer>();

        //    Mesh mesh = new Mesh {
        //        vertices = MeshData.faces[faces[i]].GetVertices().ToArray(),
        //        triangles = MeshData.faces[faces[i]].GetTriangles().ToArray()
        //    };
        //    face.GetComponent<MeshFilter>().sharedMesh = mesh;
        //    mesh.RecalculateNormals();
        //    face.GetComponent<MeshRenderer>().material = material;
        //    face.transform.parent = block.transform;
        //}

        return block;
    }
}
