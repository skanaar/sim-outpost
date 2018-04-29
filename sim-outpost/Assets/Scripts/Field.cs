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

public static class FieldOperations {
    public static float InterpolatedAt(this Field me, Vector3 p) {
        var cell = me.CellWithin(new Cell(p));
        var u = (p.x - cell.i) % 1;
        var v = (p.z - cell.j) % 1;
        var a = (1 - u)*me[cell.Add(0, 0)] + u*me[cell.Add(1, 0)];
        var b = (1 - u)*me[cell.Add(0, 1)] + u*me[cell.Add(1, 1)];
        return (1 - v) * a + v* b;
    }
    public static float Average(this Field me, int i, int j, int w, int h) {
        int xMax = min(me.Res, i+w+1);
        int yMax = min(me.Res, j+h+1);
        float sum = 0;
        for (int x = max(0, i); x < xMax; x++) {
            for (int y = max(0, j); y < yMax; y++) {
                sum += me[x, y];
            }
        }
        return sum/((w+1)*(h+1));
    }
    public static float Minimum(this Field me, int i, int j, int w, int h) {
        int xMax = min(me.Res, i+w+1);
        int yMax = min(me.Res, j+h+1);
        float value = Mathf.Infinity;
        for (int x = max(0, i); x < xMax; x++) {
            for (int y = max(0, j); y < yMax; y++) {
                value = min(value, me[x, y]);
            }
        }
        return value;
    }
    public static float Maximum(this Field me, int i, int j, int w, int h) {
        int xMax = min(me.Res, i+w+1);
        int yMax = min(me.Res, j+h+1);
        float value = Mathf.NegativeInfinity;
        for (int x = max(0, i); x < xMax; x++) {
            for (int y = max(0, j); y < yMax; y++) {
                value = max(value, me[x, y]);
            }
        }
        return value;
    }
    public static Range Range(this Field me, int i, int j, int w, int h) {
        int xMax = min(me.Res, i+w+1);
        int yMax = min(me.Res, j+h+1);
        float low = Mathf.Infinity;
        float high = Mathf.NegativeInfinity;
        for (int x = max(0, i); x < xMax; x++) {
            for (int y = max(0, j); y < yMax; y++) {
                float val = me[x, y];
                low = min(low, val);
                high = max(high, val);
            }
        }
        return new Range(low, high);
    }
    public static void Smooth(this Field me, float dt) {
        float step = max(0.5f, dt);
        for (int x = 0; x < me.Res; x++) {
            var x_ = max(0, x-1);
            var x1 = min(me.Res-1, x+1);
            for (int y = 0; y < me.Res; y++) {
                var y_ = max(0, y-1);
                var y1 = min(me.Res-1, y+1);
                var h = me[x, y];
                var d00 = me[x_, y];
                var d10 = me[x1, y];
                var d01 = me[x, y_];
                var d11 = me[x, y1];
                me[x, y] += step * (d00 + d10 + d01 + d11) / 4 - h;
            }
        }
    }
    public static void Multiply(this Field me, float k) {
        for (int x = 0; x < me.Res; x++) {
            for (int y = 0; y < me.Res; y++) {
                me[x, y] *= k;
            }
        }
    }
    public static Cell CellWithin(this Field me, Cell cell) {
        return new Cell(clamp(1, me.Res-2, cell.i), clamp(1, me.Res-2, cell.j));
    }
}

[Serializable]
public class Field {
    public float[] field;
    public float[] buffer;
    public float[] RawData => field;
    bool isBuffered;
    public int Res { get; private set; }
    public Field(float[] field, bool buffered = true) {
        Res = (int)Math.Sqrt(field.Length);
        isBuffered = buffered;
        this.field = field;
        this.buffer = buffered ? new float[Res*Res] : field;
        for (int i = 0; i < field.Length; i++) {
            buffer[i] = field[i];
        }
    }
    public Field(int res, float value = 0, bool buffered = true) {
        Res = res;
        isBuffered = buffered;
        field = new float[res*res];
        buffer = buffered ? new float[res*res] : field;
        for (int i = 0; i < field.Length; i++) {
            field[i] = value;
            buffer[i] = value;
        }
    }
    public void ApplyChanges() {
        if (!isBuffered) return;
        for (int i = 0; i < field.Length; i++) {
            field[i] = buffer[i];
        }
    }
    public float this[int i, int j] {
        get { return field[i + Res*j]; }
        set { buffer[i + Res*j] = value; }
    }
    public float this[Cell cell] {
        get { return field[cell.i + Res*cell.j]; }
        set { buffer[cell.i + Res*cell.j] = value; }
    }
    public float this[Vector3 p] {
        get { return this.InterpolatedAt(p); }
        set { this[this.CellWithin(new Cell(p))] = value; }
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
