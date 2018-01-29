using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager {

    public TerrainGrid Terrain { get; } = new TerrainGrid(21);
    public List<Building> Buildings { get; } = new List<Building>();

    public static Manager Instance { get; private set; } = new Manager();

    public Attr Commodities { get; set; } = Definitions.StartingCommodities;
    public Vector3 HoverPoint { get; set; } = new Vector3(0, 0, 0);
    public Cell SelectedCell { get; set; } = new Cell{ i = -1, j = -1};
    public Building SelectedBuilding { get; set; } = null;

    public Cell GridAtPoint(Vector3 p) {
        return new Cell { i = Mathf.RoundToInt(p.x - 0.5f), j = Mathf.RoundToInt(p.z - 0.5f) };
    }

    public void StartBuild(BuildingType type) {
        AddBuilding(type, SelectedCell);
    }

    public void AddBuilding(BuildingType type, Cell cell) {
        if (Buildings.Any(e => e.IsOccupying(cell))) {
            return;
        }
        var building = new Building { type = type, Cell = cell };
        Buildings.Add(building);
        SelectedBuilding = building;
    }

    public void Update(float deltaTime) {
        Terrain.Update(deltaTime);
        foreach (var building in Buildings) {
            if (building.BuildProgress >= 1) {
                building.IsSupplied = (Attr.Zero <= building.type.turnover + Commodities);
                if (building.IsSupplied && building.IsEnabled) {
                    Commodities = Commodities + deltaTime * building.type.turnover;
                }
            }
            else {
                var deltaCost = (-deltaTime / building.type.buildTime) * building.type.cost;
                building.IsSupplied = (Attr.Zero <= deltaCost + Commodities);
                if (building.IsSupplied && building.IsEnabled) {
                    Commodities = Commodities + deltaCost;
                    building.BuildProgress += deltaTime / building.type.buildTime;
                }
            }
        }
    }

    internal void SelectCell() {
        SelectedCell = GridAtPoint(HoverPoint);
        SelectedBuilding = Buildings.FirstOrDefault(e => e.IsOccupying(SelectedCell));
    }
}
