using System.Collections.Generic;
using UnityEngine;

public class TreeCtrl : MonoBehaviour {
    public FractalTree Tree { get; set; }
    bool isInitialized = false;

    void Update() {
        if (!isInitialized && Tree != null) {
            GetComponent<MeshFilter>().mesh = BuildMesh(Tree);
            isInitialized = true;
        }
    }

    void AddToMesh(FractalTree.Segment segment, List<Vector3> vertices, List<int> triangles) {
        int i = vertices.Count;
        vertices.Add(segment.start + new Vector3(0.1f, 0, 0));
        vertices.Add(segment.start + new Vector3(-0.05f, 0, -0.07f));
        vertices.Add(segment.start + new Vector3(-0.05f, 0, 0.07f));
        vertices.Add(segment.start + segment.dir + new Vector3(0.1f, 0, 0));
        vertices.Add(segment.start + segment.dir + new Vector3(-0.05f, 0, -0.07f));
        vertices.Add(segment.start + segment.dir + new Vector3(-0.05f, 0, 0.07f));
        triangles.AddRange(new int[] { i+1, i+0, i+3 });
        triangles.AddRange(new int[] { i+2, i+1, i+4 });
        triangles.AddRange(new int[] { i+0, i+2, i+5 });
        triangles.AddRange(new int[] { i+1, i+4, i+3 });
        triangles.AddRange(new int[] { i+2, i+5, i+4 });
        triangles.AddRange(new int[] { i+3, i+0, i+5 });
        foreach (var e in segment.children) {
            AddToMesh(e, vertices, triangles);
        }
    }

    Mesh BuildMesh(FractalTree tree) {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        AddToMesh(tree.trunk, vertices, triangles);
        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
}
