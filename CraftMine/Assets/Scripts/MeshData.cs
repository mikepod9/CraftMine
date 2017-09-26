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
        new List<int>(new int[] {
            0, 2, 1, 0, 3, 2
        }));

    public static MeshData bottom = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1)
        }),
        new List<int>(new int[] {
            0, 1, 2, 0, 2, 3
        }));

    public static MeshData right = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0)
        }),
        new List<int>(new int[] {
            0, 2, 1, 0, 3, 2
        }));

    public static MeshData left = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(0, 1, 0)
        }),
        new List<int>(new int[] {
            0, 1, 2, 0, 2, 3
        }));

    public static MeshData front = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 1)
        }),
        new List<int>(new int[] {
            0, 1, 2, 0, 2, 3
        }));

    public static MeshData back = new MeshData(

        new List<Vector3>(new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0)
        }),
        new List<int>(new int[] {
            0, 2, 1, 0, 3, 2
        }));


    public readonly static MeshData[] faces = new MeshData[] { top , bottom, right, left, front, back };

    private List<Vector3> vertices;
    private List<int> triangles;

    public MeshData(List<Vector3> vertices, List<int> triangles) {
        this.vertices = new List<Vector3>( vertices);
        this.triangles = new List<int>(triangles);
    }

    public List<Vector3> GetVertices() {
        return new List<Vector3>(vertices);
    }

    public void SetVertices(List<Vector3> vertices) {
        this.vertices = vertices;
    }

    public List<int> GetTriangles() {
        return new List<int>(triangles);
    }

    public void SetTriangles(List<int> triangles) {
        this.triangles = triangles;
    }
}
