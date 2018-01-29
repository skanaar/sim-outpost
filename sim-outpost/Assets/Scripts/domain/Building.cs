using System;
using UnityEngine;

public class Building {
    public BuildingType type;
    public int i;
    public int j;
    public bool enabled;
    public float buildProgress;
    public GameObject gameObject;

    public bool IsOccupying(Cell cell) {
        return cell.i >= i && cell.i < i + type.w && cell.j >= j && cell.j < j + type.h;
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
