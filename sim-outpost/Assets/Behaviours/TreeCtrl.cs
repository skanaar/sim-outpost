using System.Collections.Generic;
using UnityEngine;

public class TreeCtrl : MonoBehaviour {
    public FractalTree Tree { get; set; }
    bool isInitialized = false;

    void Update() {
        if (!isInitialized && Tree != null) {
            GetComponent<MeshFilter>().mesh = BuildMesh(Tree);
            GetComponent<Renderer>().material = Resources.Load<Material>("tree-material");
            isInitialized = true;
        }
    }

    void AddToMesh(FractalTree.Segment segment, List<Vector3> vertices, List<int> triangles) {
        int i = vertices.Count;
        float a = 0.05f;
        float b = a*0.7f;
        float c = a*0.5f;
        vertices.Add(segment.start + new Vector3(a, 0, 0));
        vertices.Add(segment.start + new Vector3(-c, 0, -b));
        vertices.Add(segment.start + new Vector3(-c, 0, b));
        vertices.Add(segment.start + segment.dir + new Vector3(0.1f, 0, 0));
        vertices.Add(segment.start + segment.dir + new Vector3(-c, 0, -b));
        vertices.Add(segment.start + segment.dir + new Vector3(-c, 0, b));
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
