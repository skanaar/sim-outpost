﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager {
    internal TerrainCtrl TerrainController;

    public TerrainGrid Terrain { get; } = new TerrainGrid(30);
    public List<Building> Buildings { get; } = new List<Building>();
    public List<Item> Items { get; } = new List<Item>();

    public static Manager Instance { get; private set; } = new Manager();

    public Attr Commodities { get; set; } = Definitions.StartingCommodities;
    public Vector3 HoverPoint { get; set; } = new Vector3(0, 0, 0);
    public Cell SelectedCell { get; set; } = new Cell { i = -1, j = -1 };
    public bool SelectedCellIsBuildable { get; set; } = false;
    public Building SelectedBuilding { get; set; } = null;

    public Cell GridAtPoint(Vector3 p) {
        return new Cell{ i = Mathf.RoundToInt(p.x-0.5f), j = Mathf.RoundToInt(p.z-0.5f) };
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
        SelectCell();
    }

    public void LevelTerrain(Cell cell) {
        if (Terrain.Height.ContainsCell(cell) && Commodities.energy > 10) {
            Commodities = Commodities + new Attr { energy = -10 };
            var grid = Terrain.Height;
            var h = (
                grid[cell.i + 0, cell.j + 0] +
                grid[cell.i + 1, cell.j + 0] +
                grid[cell.i + 0, cell.j + 1] +
                grid[cell.i + 1, cell.j + 1]
            ) / 4;
            grid[cell.i+0, cell.j+0] = (h + grid[cell.i+0, cell.j+0]) / 2;
            grid[cell.i+1, cell.j+0] = (h + grid[cell.i+1, cell.j+0]) / 2;
            grid[cell.i+0, cell.j+1] = (h + grid[cell.i+0, cell.j+1]) / 2;
            grid[cell.i+1, cell.j+1] = (h + grid[cell.i+1, cell.j+1]) / 2;
            TerrainController?.UpdateMesh();
        }
    }

    public void AdjustTerrain(Cell cell, float delta) {
        if (Terrain.Height.ContainsCell(cell) && Commodities.energy > 10) {
            Commodities = Commodities + new Attr { energy = -10 };
            Terrain.Height[cell.i + 0, cell.j + 0] += delta;
            Terrain.Height[cell.i + 1, cell.j + 0] += delta;
            Terrain.Height[cell.i + 0, cell.j + 1] += delta;
            Terrain.Height[cell.i + 1, cell.j + 1] += delta;
            TerrainController?.UpdateMesh();
        }
    }

    public void Update(float dt) {
        Terrain.Update(dt);
        foreach (var building in Buildings) {
            if (building.BuildProgress >= 1) {
                building.IsSupplied = (Attr.Zero <= building.type.turnover + Commodities);
                if (building.IsSupplied && building.IsEnabled) {
                    Commodities += dt * building.type.turnover;
                    foreach (var aspect in building.type.Aspects) {
                        aspect.Update(dt, Time.time, building, this);
                    }
                }
            }
            else {
                var deltaCost = (-dt / building.type.buildTime) * building.type.cost;
                building.IsSupplied = (Attr.Zero <= deltaCost + Commodities);
                if (building.IsSupplied && building.IsEnabled) {
                    Commodities += deltaCost;
                    building.BuildProgress += dt / building.type.buildTime;
                }
            }
        }
        foreach (var item in Items) {
            var viability = Terrain.Viability[(int)item.Pos.x, (int)item.Pos.z];
            if (viability < 0.25f) {
                item.IsDead = true;
            }
            item.Age = Math.Min(item.Age + viability * dt, item.Type.MaxAge);
        }
        if (Items.Count < 80 && UnityEngine.Random.value < dt) {
            Items.Add(new Item{
                Type = Definitions.tree,
                Pos = Terrain.RandomPos()
            });
        }
    }

    public void SelectCell() {
        SelectedCell = GridAtPoint(HoverPoint);
        SelectedBuilding = Buildings.FirstOrDefault(e => e.IsOccupying(SelectedCell));
        SelectedCellIsBuildable =
            Terrain.Slope(SelectedCell) < 0.25f && SelectedBuilding == null;
    }
}
