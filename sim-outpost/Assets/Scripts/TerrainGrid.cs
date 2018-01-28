using UnityEngine;
using static Util;

public struct Cell {
    public int i;
    public int j;
}

public class TerrainGrid {
    Noise noise = new Noise{ scale = 10f, octaves = 4 };
    float[] curve = { 0, 0.2f, 1 };
    public int Res { get; }
    public int GridRes { get; }
    public float MaxHeight { get; } = 10;
    public Color[] Spectrum { get; } = { rgb(0x88F), rgb(0x8F8), rgb(0x444), rgb(0xAAA) };
    public float[,] height;

    public TerrainGrid(int res) {
        Res = res * 1;
        GridRes = res;
        height = new float[Res, Res];
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                height[x, y] = MaxHeight * lerp(curve, noise[x * MeshScale, y * MeshScale]);
            }
        }
    }

    public void Update(float dt) {
    }

    public int Supersampling => Res / GridRes;

    public float MeshScale => GridRes / (float)Res;

    public Vector3 GetCellFloor(Cell cell) => GetCellFloor(cell.i, cell.j);

    public Vector3 GetCellFloor(int i, int j) {
        if (i < 0 || i >= Res || j < 0 || j >= Res) {
            return new Vector3(0, 0, 0);
        }
        float h1 = height[(i + 0) / Supersampling, (j + 0) / Supersampling];
        float h2 = height[(i + 1) / Supersampling, (j + 0) / Supersampling];
        float h3 = height[(i + 0) / Supersampling, (j + 1) / Supersampling];
        float h4 = height[(i + 1) / Supersampling, (j + 1) / Supersampling];
        float h = min(min(h1, h2), min(h3, h4));
        return new Vector3(i * MeshScale + 0.5f/ MeshScale, h, j * MeshScale + 0.5f / MeshScale);
    }
}

public class Noise {
    public float scale = 5f;
    public int octaves = 3;
    public float falloff = 0.5f;
    public float this[float x, float y] {
        get {
            float sum = 0;
            float layer = 1f;
            float opacity = 0.5f;
            for (int i = 0; i < octaves; i++) {
                sum += opacity * Mathf.PerlinNoise(x * layer / scale, y * layer / scale);
                layer *= 2f;
                opacity *= falloff;
            }
            return sum;
        }
    }
}