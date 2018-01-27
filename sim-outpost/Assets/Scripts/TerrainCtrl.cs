using UnityEngine;
using static Util;

public class TerrainCtrl : MonoBehaviour {

    TerrainGrid Terrain => Manager.Instance.Terrain;
    
    void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
    }

    Mesh BuildMesh(int res) {
        var vertices = new Vector3[res * res];
        var colors = new Color[res * res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var h = Terrain.height[i, j];
                vertices[i + res * j] = new Vector3(i * Terrain.MeshScale, h, j * Terrain.MeshScale);
                colors[i + res * j] = lerp(Terrain.Spectrum, h / Terrain.MaxHeight);
            }
        }
        var triangles = new int[2 * 3 * sq(res - 1)];
        int index = 0;
        for (int i = 0; i < res - 1; i++) {
            for (int j = 0; j < res - 1; j++) {
                if (Random.value < 0.5) {
                    triangles[index + 0] = (i + 1) + res * (j + 0);
                    triangles[index + 1] = (i + 0) + res * (j + 0);
                    triangles[index + 2] = (i + 0) + res * (j + 1);
                    triangles[index + 3] = (i + 1) + res * (j + 1);
                    triangles[index + 4] = (i + 1) + res * (j + 0);
                    triangles[index + 5] = (i + 0) + res * (j + 1);
                } else {
                    triangles[index + 0] = (i + 1) + res * (j + 0);
                    triangles[index + 1] = (i + 0) + res * (j + 0);
                    triangles[index + 2] = (i + 1) + res * (j + 1);
                    triangles[index + 3] = (i + 1) + res * (j + 1);
                    triangles[index + 4] = (i + 0) + res * (j + 0);
                    triangles[index + 5] = (i + 0) + res * (j + 1);
                }
                index += 6;
            }
        }
        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }

    void Update() {
	}
}
