using System;
using UnityEngine;

public class Building {
    public BuildingType type;
    public Cell Cell;
    public bool IsEnabled = true;
    public bool IsSupplied;
    public float BuildProgress;
    public GameObject GameObject;

    public bool IsOccupying(Cell cell) {
        return cell.i >= Cell.i && cell.i < Cell.i + type.w && cell.j >= Cell.j && cell.j < Cell.j + type.h;
    }
}

public class BuildingType {
    public string name;
    public float height;
    public int w;
    public int h;
    public Attr turnover;
    public Attr cost;
    public float buildTime;
    public string model;
}
