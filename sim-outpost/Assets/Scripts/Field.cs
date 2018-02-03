using UnityEngine;
using static Util;

public struct Cell {
    public readonly int i;
    public readonly int j;
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

public class Field {
    public readonly float[,] field;
    public int Res => field.GetLength(0);
    public Field(int res, float value = 0) {
        field = new float[res, res];
        for (int i = 0; i < res; i++) {
            for (int j = 0; j < res; j++) {
                field[i, j] = value;
            }
        }
    }
    public float this[int i, int j] => field[i, j];
    public float this[Cell cell] => field[cell.i, cell.j];
    public float this[Vector3 p] {
        get {
            var cell = new Cell(p);
            var u = p.x - cell.i;
            var v = p.z - cell.j;
            var a = (1 - u)*this[cell.Add(0, 0)] + u*this[cell.Add(1, 0)];
            var b = (1 - u)*this[cell.Add(0, 1)] + u*this[cell.Add(1, 1)];
            return (1 - v) * a + v* b;
        }
    }
    public Cell CellWithin(Cell cell) {
        return new Cell(clamp(1, Res-2, cell.i), clamp(1, Res-2, cell.j));
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
