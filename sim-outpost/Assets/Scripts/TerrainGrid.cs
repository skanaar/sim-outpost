using UnityEngine;
using static Util;

public struct Cell {
    public int i;
    public int j;
    public Vector3 ToVector => new Vector3(i, 0, j);
    public Vector3 Center => new Vector3(i+0.5f, 0, j+ 0.5f);
}

public static class ScalarFieldExtensions {
    public static float GetCell(this float[,] self, Cell cell) {
        return self[cell.i, cell.j];
    }
    public static float GetInterpolated(this float[,] self, Vector3 p) {
        var cell = new Cell{ i = (int)p.x, j = (int)p.y};
        if (self.ContainsCell(cell)) return 0;
        var u = p.x - cell.i;
        var v = p.z - cell.j;
        var a = (1 - u) * self[cell.i, cell.j] + u * self[cell.i + 1, cell.j];
        var b = (1 - u) * self[cell.i, cell.j + 1] + u * self[cell.i + 1, cell.j + 1];
        return (1 - v) * a + v * b;
    }
    public static bool ContainsCell(this float[,] self, Cell cell) {
        return 
            cell.i >= 0 &&
            cell.i < self.GetLength(0) - 1 &&
            cell.j >= 0 &&
            cell.j < self.GetLength(0) - 1;
    }
}

public class TerrainGrid {
    Noise noise = new Noise{ scale = 10f, octaves = 4 };
    float[] curve = { 0.2f, 0.15f, 0.2f, 0.6f, 1 };
    public int Res { get; }
    public float MaxHeight { get; } = 10;
    public Color[] Spectrum { get; } = { rgb(0x88F), rgb(0x8F8), rgb(0x444), rgb(0xAAA) };
    public float[,] Height;
    public float[,] Viability;

    public TerrainGrid(int res) {
        Res = res;
        Height = new float[Res, Res];
        Viability = new float[Res, Res];
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                Height[x, y] = MaxHeight * lerp(curve, noise[x, y]);
                Viability[x, y] = 1 - Height[x, y] / MaxHeight;
            }
        }
    }

    public void Update(float dt) {
    }

    public Vector3 GetCellFloor(Cell cell) => GetCellFloor(cell.i, cell.j);
    public Vector3 GetCorner(int i, int j) => new Vector3(i, Height[i, j], j);

    public Vector3 GetCellFloor(int i, int j) {
        if (i < 0 || i > Res-2 || j < 0 || j > Res-2) {
            return new Vector3(0, 0, 0);
        }
        float h1 = Height[(i + 0), (j + 0)];
        float h2 = Height[(i + 1), (j + 0)];
        float h3 = Height[(i + 0), (j + 1)];
        float h4 = Height[(i + 1), (j + 1)];
        float h = min(min(h1, h2), min(h3, h4));
        return new Vector3(i + 0.5f, h, j + 0.5f);
    }

    internal float Slope(Cell cell) {
        var i = cell.i;
        var j = cell.j;
        if (i < 0 || i > Res - 2 || j < 0 || j > Res - 2) {
            return 0;
        }
        float h1 = Height[(i + 0), (j + 0)];
        float h2 = Height[(i + 1), (j + 0)];
        float h3 = Height[(i + 0), (j + 1)];
        float h4 = Height[(i + 1), (j + 1)];
        float low = min(min(h1, h2), min(h3, h4));
        float high = max(max(h1, h2), max(h3, h4));
        return high - low;
    }

    internal Vector3 RandomPos() {
        var p = new Vector2(Random.value * Res, Random.value * Res);
        return new Vector3(p.x, Height[(int)p.x,(int)p.y], p.y);
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