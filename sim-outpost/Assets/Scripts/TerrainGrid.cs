using UnityEngine;
using static Util;

public class TerrainGrid {
    public int Res { get; }
    public float MaxHeight = 10;
    public float Erosion = 0.01f;
    public Color[] Spectrum = {rgb(0x888),rgb(0x6F2),rgb(0x444),rgb(0xAAA),rgb(0xAAA)};
    public Field Height;
    public Field Water;
    public Field Change;
    public Field XFlow;
    public Field YFlow;
    public Well[] Wells;
    public Slope Slope;
    public PeakProminence PeakProminence;

    public TerrainGrid(Field height, Field water, Well[] wells) {
        Res = height.Res;
        Height = height;
        Water = water;
        Wells = wells;
        Change = new Field(Res, buffered: false);
        XFlow = new Field(Res, buffered: false);
        YFlow = new Field(Res, buffered: false);
        Slope = new Slope(height);
        PeakProminence = new PeakProminence(Height);
    }

    public void Update(float dt) {
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                Change[x, y] = 0;
                XFlow[x, y] = 0;
                YFlow[x, y] = 0;
            }
        }
        var step = max(1, 10*dt) * 0.2f;
        for (int x = 0; x < Res; x++) {
            var x_ = max(0, x-1);
            var x1 = min(Res-1, x+1);
            for (int y = 0; y < Res; y++) {
                var y_ = max(0, y-1);
                var y1 = min(Res-1, y+1);
                var h = Height[x, y] + Water[x, y];
                var w = Water[x, y];
                var d_0 = clamp(0, w/4, h - (Height[x_, y]+Water[x_, y]));
                var d10 = clamp(0, w/4, h - (Height[x1, y]+Water[x1, y]));
                var d0_ = clamp(0, w/4, h - (Height[x, y_]+Water[x, y_]));
                var d01 = clamp(0, w/4, h - (Height[x, y1]+Water[x, y1]));
                XFlow[x, y] -= d_0;
                XFlow[x1,y] += d10;
                YFlow[x, y] -= d0_;
                YFlow[x,y1] += d01;
                Change[x_, y] += d_0;
                Change[x1, y] += d10;
                Change[x, y_] += d0_;
                Change[x, y1] += d01;
                Change[x, y] -= d_0 + d10 + d0_ + d01;
            }
        }
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                Water[x, y] += step * Change[x, y] + step * 0.0001f;
            }
        }
        foreach (var well in Wells) {
            Water[well.Cell] = well.Level;
        }
        Water.ApplyChanges();
    }

    public void UpdateErosion(float dt) {
        var step = max(1, 10*dt) * 0.2f;
        for (int x = 0; x < Res-1; x++) {
            for (int y = 0; y < Res-1; y++) {
                Height[x, y] -= Erosion * step * XFlow[x, y];
                Height[x+1, y] += Erosion * step * XFlow[x, y];
                Height[x, y] -= Erosion * step * YFlow[x, y];
                Height[x, y+1] += Erosion * step * YFlow[x, y];
            }
        }
        Height.ApplyChanges();
    }
    
    public Vector3 GetCorner(int i, int j) => new Vector3(i, Height[i, j], j);
    public Vector3 GetCellFloor(Cell cell) => GetCellFloor(cell.i, cell.j);
    public Vector3 GetCellFloor(int i, int j) => new Vector3(i,Height.Minimum(i,j,1,1),j);
    public float AverageHeight(int i, int j, int w, int h) => Height.Average(i, j, w, h);

    internal Vector3 RandomPos() {
        var p = new Vector3(Random.Range(1, Res-1), 0, Random.Range(1, Res-1));
        return new Vector3(p.x, Height[p], p.z);
    }
}

public class Well {
    public Cell Cell;
    public float Level;
    public Well(int i, int j, float level) {
        Cell = new Cell(i, j);
        Level = level;
    }
}
