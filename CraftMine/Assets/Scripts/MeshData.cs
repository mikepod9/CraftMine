using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {

    public static MeshData top = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 1)
        }),
        new int[] {
            0, 2, 1, 0, 3, 2
        });

    public static MeshData bottom = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1)
        }),
        new int[] {
            0, 1, 2, 0, 2, 3
        });

    public static MeshData right = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0)
        }),
        new int[] {
            0, 2, 1, 0, 3, 2
        });

    public static MeshData left = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(0, 1, 0)
        }),
        new int[] {
            0, 1, 2, 0, 2, 3
        });

    public static MeshData front = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 1)
        }),
        new int[] {
            0, 1, 2, 0, 2, 3
        });

    public static MeshData back = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0)
        }),
        new int[] {
            0, 2, 1, 0, 3, 2
        });


    public static MeshData[] faces = new MeshData[] { top , bottom, right, left, front, back };

    List<Vector3> vertices;
    int[] triangles;

    public MeshData(List<Vector3> vertices, int[] triangles) {
        this.vertices = vertices;
        this.triangles = triangles;
    }

    public List<Vector3> GetVertices() {
        return vertices;
    }

    public int[] GetTriangles() {
        return triangles;
    }
}
