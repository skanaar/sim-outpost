using UnityEngine;
using static Util;

public class TerrainGrid {
    Noise noise = new Noise{ scale = 12f, octaves = 4 };
    float[] curve = { 0f, 0.25f, 0.3f, 0.8f, 1 };
    public int Res { get; }
    public float MaxHeight = 10;
    public Color[] Spectrum = { rgb(0x888), rgb(0x6F2), rgb(0x444), rgb(0xAAA), rgb(0xAAA) };
    public Field Height;
    public Field Water;
    public Slope Slope;
    public PeakProminence PeakProminence;

    public TerrainGrid(int res) {
        Res = res;
        Height = new Field(res);
        Water = new Field(res);
        Slope = new Slope(Height);
        PeakProminence = new PeakProminence(Height);
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                float slope = 20 * (1 - Mathf.Cos(sqrt(sq(x-Res/2) + sq(y-Res/2))/Res));
                Height[x, y] = MaxHeight * lerp(curve, noise[x, y]) - slope;
                Water[x, y] = 0.1f;
            }
        }
    }

    public TerrainGrid(Field height, Field water) {
        Res = height.Res;
        Height = height;
        Water = water;
        Slope = new Slope(height);
        PeakProminence = new PeakProminence(Height);
    }

    public void Update(float dt) {
        var step = max(1, 10*dt) * 0.2f;
        var diff = new float[Res, Res];
        for (int x = 0; x < Res; x++) {
            var x_ = max(0, x-1);
            var x1 = min(Res-1, x+1);
            for (int y = 0; y < Res; y++) {
                var y_ = max(0, y-1);
                var y1 = min(Res-1, y+1);
                var h = Height[x, y] + Water[x, y];
                var w = Water[x, y];
                var d00 = clamp(0, w/4, h - (Height[x_, y]+Water[x_, y]));
                var d10 = clamp(0, w/4, h - (Height[x1, y]+Water[x1, y]));
                var d01 = clamp(0, w/4, h - (Height[x, y_]+Water[x, y_]));
                var d11 = clamp(0, w/4, h - (Height[x, y1]+Water[x, y1]));
                diff[x_, y] += d00;
                diff[x1, y] += d10;
                diff[x, y_] += d01;
                diff[x, y1] += d11;
                diff[x, y] -= d00 + d10 + d01 + d11;
            }
        }
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                Water[x, y] += step * diff[x, y] + step * 0.0001f;
            }
        }
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
        return new Vector3(i, h, j);
    }

    public float AverageHeight(int i, int j, int w, int h) {
        if (i < 0 || i > Res-1-w || j < 0 || j > Res-1-h) {
            return 0;
        }
        float sum = 0;
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                sum += Height[i+x, j+y];
            }
        }
        return sum/(w*h);
    }

    internal Vector3 RandomPos() {
        var p = new Vector3(Random.Range(1, Res-1), 0, Random.Range(1, Res-1));
        return new Vector3(p.x, Height[p], p.z);
    }
}