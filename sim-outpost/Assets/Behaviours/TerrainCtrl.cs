using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Util;

public class TerrainCtrl : MonoBehaviour {

    public static int TerrainLayer = 10;

    public int Hash = 0;
    Game Game => Game.Instance;
    TerrainGrid Terrain => Game.Instance.Terrain;
    int lastDataOverlay = 0;
    Color[] spectrum = new Color[] {
        rgb(0x421), rgb(0x176), rgb(0x743), rgb(0xc65),
        rgb(0xF86), rgb(0xC65), rgb(0x743), rgb(0xC65),
        rgb(0x421), rgb(0xCA5), rgb(0x9C2), rgb(0xCA5),
        rgb(0x421), rgb(0xD98), rgb(0xF86), rgb(0x932),
        rgb(0xD98), rgb(0xD98), rgb(0xD98), rgb(0x537)
    };

    void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh(Terrain.Res);
        var material = Materials.Ground;
        material = Materials.Ground;
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
        var tex = GenerateTexture(512, Game.Terrain.Height, spectrum);
        GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
    }

    public void OnWaterChange() {
        UpdateMeshColor();
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

    void UpdateMeshColor() {
        GetComponent<MeshFilter>().mesh.colors = GetComponent<MeshFilter>().mesh
                                               .vertices
                                               .Select(p => groundColor((int)p.x, (int)p.z))
                                               .ToArray();
    }

    Mesh BuildMesh(int res) {
        var msh = BuildTerrainMeshFogOfWar(Terrain.Height, Cell.Zero, res);
        msh.colors = msh.vertices.Select(p => groundColor((int)p.x, (int)p.z)).ToArray();
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
        var triangles = new List<int>();
        for (int i = 0; i < res - 1; i++) {
            for (int j = 0; j < res - 1; j++) {
                if ((offset.i + i + offset.j + j) % 2 == 0) {
                    triangles.Add((i + 1) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 1));
                    triangles.Add((i + 1) + res * (j + 1));
                    triangles.Add((i + 1) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 1));
                }
                else {
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
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }

    public Mesh BuildTerrainMeshFogOfWar(Field height, Cell offset, int res) {
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
        var triangles = new List<int>();
        for (int i = 0; i < res - 1; i++) {
            for (int j = 0; j < res - 1; j++) {
                if (Game.NeighbourDist[i, j] > Game.VisionRange) { continue; }
                if ((offset.i + i + offset.j + j) % 2 == 0) {
                    triangles.Add((i + 1) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 1));
                    triangles.Add((i + 1) + res * (j + 1));
                    triangles.Add((i + 1) + res * (j + 0));
                    triangles.Add((i + 0) + res * (j + 1));
                }
                else {
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
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
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
        var span = max(0.01f, range.High - range.Low);
        var factor = height.Res / (float)res;
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                var p = new Vector3(i * factor, 0, j * factor);
                var fog = clamp(0, 1, 0.5f * (Game.VisionRange - Game.NeighbourDist[p]));
                texture.SetPixel(i, j, fog*lerp(spectrum, (height[p] - range.Low)/span));
            }
        }
        texture.Apply();
        return texture;
    }
}
