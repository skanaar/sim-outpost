using UnityEngine;
using static UnityEngine.Rendering.ShadowCastingMode;
using static Util;

public class WaterCtrl : MonoBehaviour {

    TerrainGrid Terrain => Game.Instance.Terrain;

    void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        var r = gameObject.GetComponent<Renderer>();
        r.material = Materials.Water;
        r.receiveShadows = false;
        r.shadowCastingMode = Off;
    }

    Mesh BuildMesh(int res) {
        var minima = Game.Instance.WaterLowThreshold;
        var vertices = new Vector3[res * res];
        var uv = new Vector2[res * res];
        var colors = new Color[res * res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var w = Terrain.Water[i, j];
                var h = Terrain.Height[i, j];
                vertices[i + res * j] = new Vector3(i, w+h-minima, j);
                uv[i + res * j] = new Vector2(i/(float)res, j/(float)res);
                colors[i + res * j] = new Color(1,1,1,Mathf.Min(1, 1.25f*w));
            }
        }
        var triangles = new int[2 * 3 * sq(res - 1)];
        int index = 0;
        for (int i = 0; i < res - 1; i++) {
            for (int j = 0; j < res - 1; j++) {
                if ((i + j) % 2 == 0) {
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
        mesh.uv = uv;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }

    void Update() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        Destroy(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }
}
