using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager {

    public TerrainGrid Terrain { get; } = new TerrainGrid(21);
    public List<Building> Buildings { get; } = new List<Building>{
        new Building{ type = Definitions.reactor, i = 1, j = 3 },
        new Building{ type = Definitions.greenhouse, i = 6, j = 6 },
        new Building{ type = Definitions.greenhouse, i = 5, j = 4 },
        new Building{ type = Definitions.habitat, i = 8, j = 8 },
    };

    public static Manager Instance { get; private set; } = new Manager();

    public Action<Building> OnAddBuilding { get; set; }
    public Attr Commodities { get; set; } = Definitions.StartingCommodities;
    public Vector3 HoverPoint { get; set; } = new Vector3(0, 0, 0);
    public Cell SelectedCell { get; set; } = new Cell{ i = -1, j = -1};

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
        var building = new Building { type = type, i = cell.i, j = cell.j };
        Buildings.Add(building);
        OnAddBuilding(building);
    }

    public void Update(float deltaTime) {
        Terrain.Update(deltaTime);
        foreach (var building in Buildings) {
            if (building.buildProgress >= 1) {
                building.enabled = (Attr.Zero <= building.type.turnover + Commodities);
                if (building.enabled) {
                    Commodities = Commodities + deltaTime * building.type.turnover;
                }
            }
            else {
                var deltaCost = (-deltaTime / building.type.buildTime) * building.type.cost;
                building.enabled = (Attr.Zero <= deltaCost + Commodities);
                if (building.enabled) {
                    Commodities = Commodities + deltaCost;
                    building.buildProgress += deltaTime / building.type.buildTime;
                }
            }
        }
    }
}
