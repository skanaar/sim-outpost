using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Util;

public class Game {
    public static Game Instance { get; private set; } = new Game();

    internal TerrainCtrl TerrainController;

    // constants
    public int Res = 50;
    public float BuildRange = 5;
    public float MaxBuildingSlope = 0.25f;
    public float ShoreBeauty = 2f;
    public float BeautyDecay = 0.4f;
    public float WaterLowThreshold = 0.05f;
    public float PollutionDecay = 0.002f;
    public float PollutionDispersal = 0.001f;
    public float PollutionPersistTreshold = 0.01f;
    public float PollutionOverlayScale = 5f;
    public float BeautyOverlayScale = 0.2f;
    public float BeautyMax = 1f;

    // game state
    public float Time => UnityEngine.Time.time;
    public TerrainGrid Terrain;
    public List<Building> Buildings { get; } = new List<Building>();
    public List<Entity> Entities { get; } = new List<Entity>();
    public Field NeighbourDist;
    public Field Beauty;
    public Field Pollution;
    public Attr Store { get; set; } = Definitions.StartingCommodities;
    public int Beds { get; set; } = 0;
    public int Population { get; set; } = 0;

    // ui state
    public float Zoom = 1.0f;
    public Vector3 Pan = Vector3.zero;
    public Vector3 HoverPoint { get; set; } = new Vector3(0, 0, 0);
    public Cell SelectedCell { get; set; } = new Cell(0, 0);
    public Building SelectedBuilding { get; set; } = null;
    public int DataOverlay { get; set; } = 0;

    public Game() {
        Terrain = new TerrainGrid(Res);
        NeighbourDist = new Field(Res);
        Beauty = new Field(Res);
        Pollution = new Field(Res);
        Entities.Add(new Entity {
            Pos = Vector3.zero, Type=Definitions.treeCollector
        });
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
        TerrainController.OnWaterChange();
        foreach (var building in Buildings) {
            building.Update(dt, this);
        }
        foreach (var mob in Entities) {
            mob.Update(dt, this);
        }
        GrowTrees(dt);
        EnvironmentalBeauty(dt);
        PollutionNaturalDecay(dt);
        Beauty.Smooth(dt);
        Pollution.Smooth(dt*PollutionDispersal);
        Beds = (int)Buildings.Sum(e => e.type.beds * compress(Beauty[e.Cell], halfAt: 5));
    }

    public void EnvironmentalBeauty(float dt) {
        for (int x = 1; x < Terrain.Res-1; x++) {
            for (int y = 1; y < Terrain.Res-1; y++) {
                var w0 = Terrain.Water[x, y] > WaterLowThreshold;
                var wx = Terrain.Water[x-1, y] > WaterLowThreshold;
                var wy = Terrain.Water[x, y-1] > WaterLowThreshold;
                var wX = Terrain.Water[x+1, y] > WaterLowThreshold;
                var wY = Terrain.Water[x, y+1] > WaterLowThreshold;
                if (w0 != wx || w0 != wy || w0 != wX || w0 != wY) { // cell is shoreline
                    Beauty[x, y] += dt*ShoreBeauty;
                }
                var b = Beauty[x, y] - Mathf.Sign(Beauty[x, y])*dt*BeautyDecay;
                Beauty[x, y] = clamp(-BeautyMax, BeautyMax, b);
            }
        }
    }

    public void PollutionNaturalDecay(float dt) {
        for (int x = 1; x < Terrain.Res-1; x++) {
            for (int y = 1; y < Terrain.Res-1; y++) {
                var decay = Pollution[x,y]>PollutionPersistTreshold ? 0 : PollutionDecay;
                Pollution[x, y] = max(0, Pollution[x, y] - dt*decay);
            }
        }
    }

    public void GrowTrees(float dt) {
        if (Entities.Count < 10 && UnityEngine.Random.value < dt) {
            Entities.Add(new Entity{
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
