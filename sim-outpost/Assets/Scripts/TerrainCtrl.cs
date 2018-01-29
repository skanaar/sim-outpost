using UnityEngine;
using static Util;

public class TerrainCtrl : MonoBehaviour {

    public static int TerrainLayer = 10;

    TerrainGrid Terrain => Manager.Instance.Terrain;
    
    void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        GetComponent<Renderer>().material = new Material(Shader.Find("Low Poly"));
        GetComponent<Renderer>().material.color = rgb(0x4A4);
        gameObject.AddComponent<MeshCollider>();
        gameObject.layer = TerrainLayer;
    }

    Mesh BuildMesh(int res) {
        var vertices = new Vector3[res * res];
        var colors = new Color[res * res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var h = Terrain.height[i, j];
                vertices[i + res * j] = new Vector3(i, h, j);
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
                }
                else {
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

    Mesh BuildFlatMesh(int res) {
        var vertices = new Vector3[res * res * 4];
        var colors = new Color[res * res * 4];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var h = Terrain.height[i, j];
                vertices[(i + res * j) * 4 + 0] = new Vector3(i + 0, h, j + 0);
                vertices[(i + res * j) * 4 + 1] = new Vector3(i + 1, h, j + 0);
                vertices[(i + res * j) * 4 + 2] = new Vector3(i + 0, h, j + 1);
                vertices[(i + res * j) * 4 + 3] = new Vector3(i + 1, h, j + 1);
                colors[(i + res * j) * 4 + 0] = lerp(Terrain.Spectrum, h / Terrain.MaxHeight);
                colors[(i + res * j) * 4 + 1] = lerp(Terrain.Spectrum, h / Terrain.MaxHeight);
                colors[(i + res * j) * 4 + 2] = lerp(Terrain.Spectrum, h / Terrain.MaxHeight);
                colors[(i + res * j) * 4 + 3] = lerp(Terrain.Spectrum, h / Terrain.MaxHeight);
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
                }
                else {
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
