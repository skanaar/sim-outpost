using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.ShadowCastingMode;
using static Util;

public class WaterCtrl : MonoBehaviour {

    TerrainGrid Terrain => Game.Instance.Terrain;
    Game Game => Game.Instance;

    void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        var r = gameObject.GetComponent<Renderer>();
        r.material = Materials.Water;
        r.receiveShadows = false;
        r.shadowCastingMode = Off;
    }

    Mesh BuildMesh(int res) {
        var vertices = new Vector3[res * res];
        var uv = new Vector2[res * res];
        var colors = new Color[res * res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var w = Terrain.Water[i, j];
                var h = Terrain.Height[i, j];
                vertices[i + res * j] = new Vector3(i, w+h, j);
                uv[i + res * j] = new Vector2(i/(float)res, j/(float)res);
                colors[i + res * j] = new Color(1, 1, 1, Mathf.Min(1, 1.25f*w));
            }
        }
        for (int i = 1; i < res-1; i++) {
            for (int j = 1; j < res-1; j++) {
                var w = Terrain.Water[i, j];
                var h = Terrain.Height[i, j];
                if (Terrain.Water[i, j] < Game.WaterLowThreshold) {
                    var a = Terrain.Height[i+1, j]+Terrain.Water[i+1, j];
                    var b = Terrain.Height[i-1,j]+Terrain.Water[i-1, j];
                    var c = Terrain.Height[i, j+1]+Terrain.Water[i, j+1];
                    var d = Terrain.Height[i, j-1]+Terrain.Water[i, j-1];
                    vertices[i + res * j] = new Vector3(i, min(a,b,c,d), j);
                }
            }
        }
        var triangles = new List<int>();
        for (int i = 0; i < res - 1; i++) {
            for (int j = 0; j < res - 1; j++) {
                if (Game.NeighbourDist[i, j] > Game.VisionRange) { continue; }
                if ((i + j) % 2 == 0) {
                    triangles.Add((i + 1) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 1));
                    triangles.Add((i + 1) + res * (j + 1));
                    triangles.Add((i + 1) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 1));
                } else {
                    triangles.Add((i + 1) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 0));
                    triangles.Add((i + 1) + res * (j + 1));
                    triangles.Add((i + 1) + res * (j + 1));
                    triangles.Add((i + 0) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 1));
                }
            }
        }
        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.colors = colors;
        mesh.triangles = triangles.ToArray();
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
