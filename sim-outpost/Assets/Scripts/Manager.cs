using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager {
    public static Manager Instance { get; private set; } = new Manager();

    internal TerrainCtrl TerrainController;

    public TerrainGrid Terrain;
    public List<Building> Buildings { get; } = new List<Building>();
    public List<Item> Items { get; } = new List<Item>();
    public Field NeighbourDist;

    public Attr Store { get; set; } = Definitions.StartingCommodities;
    public Vector3 HoverPoint { get; set; } = new Vector3(0, 0, 0);
    public Cell SelectedCell { get; set; } = new Cell(0, 0);
    public bool IsBuildable(Cell cell) => Terrain.Slope(cell) < 0.25f && NeighbourDist[cell]<5;
    public Building SelectedBuilding { get; set; } = null;
    public float Time => UnityEngine.Time.time;
    public float BuildRange = 5;

    public Manager() {
        Terrain = new TerrainGrid(30);
        NeighbourDist = new Field(Terrain.Res);
    }

    public void AddBuilding(BuildingType type, Cell cell) {
        foreach (var e in Buildings.Where(e => e.IsOccupying(cell))) {
            e.IsDead = true;
        }
        var building = new Building { type = type, Cell = cell };
        Buildings.Add(building);
        SelectedBuilding = building;
        for (int i = 0; i < Terrain.Res; i++) {
            for (int j = 0; j < Terrain.Res; j++) {
                NeighbourDist.field[i, j] = 1000;
                foreach (var e in Buildings) {
                    var x = NeighbourDist.field[i, j];
                    var dist = (e.Cell.ToVector - new Vector3(i, 0, j)).magnitude;
                    NeighbourDist.field[i, j] = Math.Min(x, dist);
                }
            }
        }
    }

    public void Update(float dt) {
        Terrain.Update(dt);
        foreach (var building in Buildings) {
            building.Update(dt, this);
        }
        foreach (var item in Items) {
            var viability = Terrain.Viability[(int)item.Pos.x, (int)item.Pos.z];
            if (viability < 0.25f) {
                item.IsDead = true;
            }
            item.Age = Math.Min(item.Age + viability * dt, item.Type.MaxAge);
        }
        if (Items.Count < 10 && UnityEngine.Random.value < dt) {
            Items.Add(new Item{
                Type = Definitions.tree,
                Pos = Terrain.RandomPos()
            });
        }
    }

    public void SelectCell() {
        SelectedCell = new Cell(HoverPoint);
        SelectedBuilding = Buildings.FirstOrDefault(e => e.IsOccupying(SelectedCell));
    }
}
