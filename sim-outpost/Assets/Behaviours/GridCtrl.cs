using UnityEngine;
using static Util;

public class GridCtrl : MonoBehaviour {
    
    TerrainGrid Terrain => Game.Instance.Terrain;
    Game Game => Game.Instance;

    void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        Material material = new Material(Shader.Find("Low Poly"));
        material.color = rgba(0x0004);
        GetComponent<Renderer>().material = material;
    }

    public void OnTerrainChange() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        Destroy(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }

    Mesh BuildMesh(int res) {
        var vertices = new Vector3[res * res * 2];
        var r2 = res * res;
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var isBuildable = Game.NeighbourDist[i, j] < Game.BuildRange;
                var h = Terrain.Height[i, j] + (isBuildable ? 0f : -0.5f);
                vertices[i + res * j] = new Vector3(i, h, j);
                vertices[i + res * j + r2] = new Vector3(i, h+0.2f, j);
            }
        }
        var triangles = new int[2 * 3 * 2 * sq(res - 1)];
        int index = 0;
        for (int i = 0; i < res - 1; i++) {
            for (int j = 0; j < res - 1; j++) {
                triangles[index + 0] = (i + 0) + res * (j + 0);
                triangles[index + 1] = (i + 0) + res * (j + 0) + r2;
                triangles[index + 2] = (i + 1) + res * (j + 0);
                triangles[index + 3] = (i + 0) + res * (j + 0) + r2;
                triangles[index + 4] = (i + 1) + res * (j + 0) + r2;
                triangles[index + 5] = (i + 1) + res * (j + 0);

                triangles[index + 6] = (i + 0) + res * (j + 0) + r2;
                triangles[index + 7] = (i + 0) + res * (j + 0);
                triangles[index + 8] = (i + 0) + res * (j + 1) + r2;
                triangles[index + 9] = (i + 0) + res * (j + 0);
                triangles[index +10] = (i + 0) + res * (j + 1);
                triangles[index +11] = (i + 0) + res * (j + 1) + r2;
                index += 12;
            }
        }
        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
}
