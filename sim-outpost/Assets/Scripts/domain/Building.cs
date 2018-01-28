using UnityEngine;

public class Building {
    public BuildingType type;
    public int i;
    public int j;
    public float power;
    public bool enabled;
    public GameObject gameObject;
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

