using UnityEngine;
using static Util;

public class TerrainCtrl : MonoBehaviour {

    public static int TerrainLayer = 10;

    public int Hash = 0;
    Game Game => Game.Instance;
    TerrainGrid Terrain => Game.Instance.Terrain;
    int lastDataOverlay = 0;

    void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        var material = Materials.Ground;
        material = Materials.Ground;
        var spectrum = new Color[] {
            rgb(0x421), rgb(0x176), rgb(0x743), rgb(0xc65),
            rgb(0xF86), rgb(0xC65), rgb(0x743), rgb(0xC65),
            rgb(0x421), rgb(0xCA5), rgb(0x9C2), rgb(0xCA5),
            rgb(0x421), rgb(0xD98), rgb(0xF86), rgb(0x932),
            rgb(0xD98), rgb(0xD98), rgb(0xD98), rgb(0x537)
        };
        var tex = GenerateTexture(512, Game.Terrain.Height, spectrum);
        material.SetTexture("_MainTex", tex);
        material.SetTextureScale("_MainTex", new Vector2(1, 1));
        material.SetTextureOffset("_MainTex", new Vector2(0, 0));
        material.SetTextureScale("_GridTex", Game.Terrain.Res * new Vector2(1, 1));
        GetComponent<Renderer>().material = material;
        gameObject.AddComponent<MeshCollider>();
        gameObject.layer = TerrainLayer;
    }

    public void OnTerrainChange() {
        Hash++;
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
        if (Terrain.Water[i, j] > Game.WaterLowThreshold) return 0;
        if (Terrain.PeakProminence[new Cell(i, j)] > 0.5) return 1;
        var inRange = Game.NeighbourDist[i, j]<Game.BuildRange;
        if (Terrain.Slope[i, j] <= Game.MaxBuildingSlope && inRange) return 0.3f;
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
        var msh = BuildTerrainMesh(Terrain.Height, Cell.Zero, res);
        var colors = new Color[res * res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                colors[i + res * j] = groundColor(i, j);
            }
        }
        msh.colors = colors;
        return msh;
    }

    public static Mesh BuildTerrainMesh(Field height, Cell offset, int res) {
        var vertices = new Vector3[res * res];
        var uv = new Vector2[res * res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var x = offset.i + i;
                var y = offset.j + j;
                var h = height[x, y];
                vertices[i + res * j] = new Vector3(x, h, y);
                uv[i+res*j] = new Vector2(x/(float)res, y/(float)res);
            }
        }
        var triangles = new int[2 * 3 * sq(res - 1)];
        int index = 0;
        for (int i = 0; i < res - 1; i++) {
            for (int j = 0; j < res - 1; j++) {
                if ((offset.i + i + offset.j + j) % 2 == 0) {
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

    Texture2D GenerateTexture(int res, Field height, Color[] spectrum) {
        var texture = new Texture2D(res, res, TextureFormat.ARGB32, false);
        var range = height.Range(0, 0, height.Res, height.Res);
        var span = range.High - range.Low;
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var p = (height.Res / (float)res) * new Vector3(i, 0, j);
                texture.SetPixel(i, j, lerp(spectrum, (height[p] - range.Low)/span));
            }
        }
        texture.Apply();
        return texture;
    }
}
