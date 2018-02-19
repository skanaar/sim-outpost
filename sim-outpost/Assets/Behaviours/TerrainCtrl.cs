using UnityEngine;
using static Util;

public class TerrainCtrl : MonoBehaviour {

    public static int TerrainLayer = 10;

    Game Game => Game.Instance;
    TerrainGrid Terrain => Game.Instance.Terrain;
    int lastDataOverlay = 0;

    void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        GetComponent<Renderer>().material = Materials.Ground;
        gameObject.AddComponent<MeshCollider>();
        gameObject.layer = TerrainLayer;
    }

    public void OnTerrainChange() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        Destroy(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }

    public void OnWaterChange() {
        UpdateMeshColor(Terrain.Res);
    }

    Color groundColor(int i, int j) {
        float pollution = clamp(0, 1, Game.Pollution[i, j]*Game.PollutionOverlayScale);
        float beauty = clamp(0, 1, 0.5f + Game.Beauty[i, j]*Game.BeautyOverlayScale);
        float zone = getZone(i, j);
        return new Color(pollution, beauty, Game.Terrain.XFlow[i,j], zone);
    }

    float getZone(int i, int j) {
        var waterMin = Game.Instance.WaterLowThreshold;
        if (Terrain.Water[i, j] > waterMin) return 0;
        if (Terrain.PeakProminence[new Cell(i, j)] > 0.5) return 1;
        if (Terrain.Slope[i, j] <= Game.Instance.MaxBuildingSlope) return 0.3f;
        return 0.6f;
    }

    void UpdateMeshColor(int res) {
        var colors = new Color[res * res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                colors[i + res * j] = groundColor(i, j);
            }
        }
        GetComponent<MeshFilter>().mesh.colors = colors;
    }

    Mesh BuildMesh(int res) {
        var vertices = new Vector3[res * res];
        var uv = new Vector2[res * res];
        var colors = new Color[res * res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var h = Terrain.Height[i, j];
                vertices[i + res * j] = new Vector3(i, h, j);
                uv[i + res * j] = new Vector2(i/(float)res, j/(float)res);
                colors[i + res * j] = groundColor(i, j);
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
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }

    void Update() {
        if (lastDataOverlay != Game.Instance.DataOverlay) {
            lastDataOverlay = Game.Instance.DataOverlay;
            var rendr = GetComponent<Renderer>(); 
            switch (Game.Instance.DataOverlay) {
                case 0: rendr.material = Materials.Ground; break;
                case 1: rendr.material = Materials.GroundBeauty; break;
                case 2: rendr.material = Materials.GroundPollution; break;
                case 3: rendr.material = Materials.GroundBeauty; break;
                default: rendr.material = Materials.Ground; break;
            }
        }
	}
}
