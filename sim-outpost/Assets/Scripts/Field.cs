using System;
using UnityEngine;
using static Util;

[Serializable]
public struct Cell {
    public static Cell Zero = new Cell(0, 0);
    public int i;
    public int j;
    public Cell(Vector3 vec): this((int)(vec.x), (int)(vec.z)) {}
    public Cell(int i, int j) {
        this.i = i;
        this.j = j;
    }
    public Vector3 ToVector => new Vector3(i, 0, j);
    public Vector3 Center => new Vector3(i+0.5f, 0, j+ 0.5f);
    public Cell Add(int u, int v) => new Cell(i+u, j+v);
    public static bool operator ==(Cell a, Cell b) => a.i == b.i && a.j == b.j;
    public static bool operator !=(Cell a, Cell b) => !(a == b);
    public override bool Equals(object obj) {
        return !(obj == null || GetType() != obj.GetType()) && this == (Cell)obj;
    }
    public override int GetHashCode() => i + 31*j;
}

[Serializable]
public class Field {
    public float[] field;
    public int Res;
    public Field(int res, float value = 0) {
        Res = res;
        field = new float[res*res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                field[i + res*j] = value;
            }
        }
    }
    public float this[int i, int j] {
        get { return field[i + Res*j]; }
        set { field[i + Res*j] = value; }
    }
    public float this[Cell cell] {
        get { return field[cell.i + Res*cell.j]; }
        set { field[cell.i + Res*cell.j] = value; }
    }
    public float this[Vector3 p] {
        get {
            var cell = CellWithin(new Cell(p));
            var u = (p.x - cell.i) % 1;
            var v = (p.z - cell.j) % 1;
            var a = (1 - u)*this[cell.Add(0, 0)] + u*this[cell.Add(1, 0)];
            var b = (1 - u)*this[cell.Add(0, 1)] + u*this[cell.Add(1, 1)];
            return (1 - v) * a + v* b;
        }
        set { this[CellWithin(new Cell(p))] = value; }
    }
    public Cell CellWithin(Cell cell) {
        return new Cell(clamp(1, Res-2, cell.i), clamp(1, Res-2, cell.j));
    }
    public void Smooth(float dt) {
        float step = max(0.5f, dt);
        var diff = new float[Res, Res];
        for (int x = 0; x < Res; x++) {
            var x_ = max(0, x-1);
            var x1 = min(Res-1, x+1);
            for (int y = 0; y < Res; y++) {
                var y_ = max(0, y-1);
                var y1 = min(Res-1, y+1);
                var h = this[x, y];
                var d00 = this[x_, y];
                var d10 = this[x1, y];
                var d01 = this[x, y_];
                var d11 = this[x, y1];
                diff[x, y] = (d00 + d10 + d01 + d11) / 4 - h;
            }
        }
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                this[x, y] += step * diff[x, y];
            }
        }
    }
    public void Multiply(float k) {
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                this[x, y] *= k;
            }
        }
    }
    public float Average(int i, int j, int w, int h) {
        int xMax = min(Res, i+w+1);
        int yMax = min(Res, j+h+1);
        float sum = 0;
        for (int x = max(0, i); x < xMax; x++) {
            for (int y = max(0, j); y < yMax; y++) {
                sum += field[i + Res*y];
            }
        }
        return sum/((w+1)*(h+1));
    }
    public float Minimum(int i, int j, int w, int h) {
        int xMax = min(Res, i+w+1);
        int yMax = min(Res, j+h+1);
        float value = Mathf.Infinity;
        for (int x = max(0, i); x < xMax; x++) {
            for (int y = max(0, j); y < yMax; y++) {
                value = min(value, field[i + Res*y]);
            }
        }
        return value;
    }
    public float Maximum(int i, int j, int w, int h) {
        int xMax = min(Res, i+w+1);
        int yMax = min(Res, j+h+1);
        float value = Mathf.NegativeInfinity;
        for (int x = max(0, i); x < xMax; x++) {
            for (int y = max(0, j); y < yMax; y++) {
                value = max(value, field[x + Res*y]);
            }
        }
        return value;
    }
    public Range Range(int i, int j, int w, int h) {
        int xMax = min(Res, i+w+1);
        int yMax = min(Res, j+h+1);
        float low = Mathf.Infinity;
        float high = Mathf.NegativeInfinity;
        for (int x = max(0, i); x < xMax; x++) {
            for (int y = max(0, j); y < yMax; y++) {
                float val = field[x + Res*y];
                low = min(low, val);
                high = max(high, val);
            }
        }
        return new Range(low, high);
    }
}

public class Slope {
    public readonly Field field;
    public Slope(Field f) { this.field = f; }
    public float this[Cell cell] => this[cell.i, cell.j];
    public float this[int i, int j] {
        get {
            if (i < 0 || i > field.Res - 2 || j < 0 || j > field.Res - 2) {
                return 0;
            }
            float h1 = field[(i + 0), (j + 0)];
            float h2 = field[(i + 1), (j + 0)];
            float h3 = field[(i + 0), (j + 1)];
            float h4 = field[(i + 1), (j + 1)];
            float low = min(min(h1, h2), min(h3, h4));
            float high = max(max(h1, h2), max(h3, h4));
            return high - low;
        }
    }
}

public class FieldSum {
    public readonly Field A, B;
    public FieldSum(Field a, Field b) {
        A = a;
        B = b;
    }
    public float this[Cell cell] => A[cell] + B[cell];
    public float this[int i, int j] => A[i, j] + B[i, j];
}

class Gradient {
    public Field field;
    public Gradient(Field field) { this.field = field; }
    public float this[Cell cell] {
        get {
            var c = field.CellWithin(cell);
            float h1 = field[c.Add(0, 0)];
            float h2 = field[c.Add(1, 0)];
            float h3 = field[c.Add(0, 1)];
            float h4 = field[c.Add(1, 1)];
            float low = min(min(h1, h2), min(h3, h4));
            float high = max(max(h1, h2), max(h3, h4));
            return high - low;
        }
    }
}

public class FieldAverage {
    public int Radius = 4;
    public Field field;
    public FieldAverage(Field field, int radius) {
        this.field = field;
        Radius = radius;
    }
    public float this[Cell cell] {
        get {
            var iLow = max(0, cell.i-Radius);
            var jLow = max(0, cell.j-Radius);
            var iHig = min(field.Res, cell.i+Radius);
            var jHig = min(field.Res, cell.j+Radius);
            float sum = 0;
            for (var i = iLow; i<iHig; i++) {
                for (var j = jLow; j<jHig; j++) {
                    sum += field[i, j];
                }
            }
            return sum/(4*Radius*Radius);
        }
    }
}

public class PeakProminence {
    public int Radius = 4;
    public Field field;
    public PeakProminence(Field field) {
        this.field = field;
    }
    public float this[Cell cell] =>
        field[cell] - (new FieldAverage(field, Radius))[cell];
}


public static class ScalarFieldExtensions {
    public static float GetInterpolated(this float[,] self, Vector3 p) {
        var cell = new Cell(p);
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
