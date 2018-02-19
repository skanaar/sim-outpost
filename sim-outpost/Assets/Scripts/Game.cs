using System;
using System.Collections.Generic;
using System.IO;
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
    public float PollutionDecay = 0.01f;
    public float PollutionDispersal = 0.001f;
    public float PollutionPersistTreshold = 0.1f;
    public float PollutionOverlayScale = 5f;
    public float BeautyOverlayScale = 0.2f;
    public float BeautyMax = 1f;

    // engine settings
    public float UpdatePeriod = 0.1f;

    // game state
    public float Time = 0;
    public TerrainGrid Terrain;
    public List<Building> Buildings { get; private set; } = new List<Building>();
    public List<Entity> Entities { get; } = new List<Entity>();
    public List<Entity> SpawnedEntities { get; } = new List<Entity>();
    public Field NeighbourDist;
    public Field Beauty;
    public Field Pollution;
    public Field EntityDensity;
    public Attr Store { get; set; } = Definitions.StartingCommodities;
    public int Beds { get; set; } = 0;
    public int WorkforceDemand { get; set; } = 0;
    public int Population { get; set; } = 10;

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
        EntityDensity = new Field(Res);
        Entities.Add(new Entity {
            Pos = Terrain.GetCellFloor(Res/2, Res/2), Type=Definitions.treeCollector
        });
        Entities.Add(new Entity {
            Pos = Terrain.GetCellFloor(Res/4, Res/4), Type=Definitions.tree
        });
        Entities.Add(new Entity {
            Pos = Terrain.GetCellFloor(Res/2, Res/4), Type=Definitions.tree
        });
        Entities.Add(new Entity {
            Pos = Terrain.GetCellFloor(Res/4, Res/2), Type=Definitions.tree
        });
        Pan = Terrain.GetCellFloor(Res / 2, Res / 2);
    }

    public void AddBuilding(BuildingType type, Cell cell) {
        foreach (var e in Buildings.Where(e => e.IsOccupying(cell))) {
            e.IsDead = true;
        }
        var building = new Building { type = type, Cell = cell };
        Buildings.Add(building);
        SelectedBuilding = building;
        CalcNeighbourDistances();
    }

    public void AddEntity(EntityType type, Vector3 pos) {
        SpawnedEntities.Add(new Entity { Type = type, Pos = pos });
    }

    public void UpdateEntityDensity() {
        for (int x = 0; x < Res; x++) {
            for (int y = 0; y < Res; y++) {
                EntityDensity[x, y] = 0;
            }
        }
        foreach (var e in Entities) {
            EntityDensity[e.Pos] += 1;
        }
    }

    public void CalcNeighbourDistances() {
        var defaultDist = Buildings.Count > 0 ? 1000 : 0;
        for (int i = 0; i < Terrain.Res; i++) {
            for (int j = 0; j < Terrain.Res; j++) {
                NeighbourDist[i, j] = defaultDist;
                foreach (var e in Buildings) {
                    var x = NeighbourDist[i, j];
                    var dist = (e.Cell.ToVector - new Vector3(i, 0, j)).magnitude;
                    NeighbourDist[i, j] = Math.Min(x, dist);
                }
            }
        }
    }

    public bool ShouldTrigger(float dt, float period) {
        return (Time > period) && (Time % period > (Time+dt) % period);
    }

    public void Stabilize(int steps) {
        var dt = 0.4f;
        for (int i = 0; i < steps; i++) {
            Terrain.Update(dt);
        }
        for (int i = 0; i < steps; i++) {
            EnvironmentalBeauty(dt);
            Beauty.Smooth(dt);
        }
    }

    public void Update(float dt) {
        bool anythingDied = false;
        Time += dt;
        Terrain.Update(dt);
        TerrainController.OnWaterChange();
        foreach (var building in Buildings) {
            building.UpdateConstructing(dt, this);
        }
        foreach (var mob in Entities) {
            mob.Update(dt, this);
            anythingDied = anythingDied || mob.IsDead;
        }
        if (anythingDied) {
            UpdateEntityDensity();
        }
        if (ShouldTrigger(dt, UpdatePeriod)) UpdateEconomy(dt);
        if (ShouldTrigger(dt, UpdatePeriod*4)) UpdateEnvironment(dt);
        if (ShouldTrigger(dt, 10)) SaveGame();
        Terrain.Water[25, 25] += dt;
    }

    public void UpdateEconomy(float dt) {
        foreach (var building in Buildings) {
            building.Update(dt, this);
        }
        if (SpawnedEntities.Count > 0) {
            Entities.AddRange(SpawnedEntities);
            SpawnedEntities.RemoveAll(e => true);
            UpdateEntityDensity();
        }
        Beds = Buildings.Sum(e => e.type.beds);
        WorkforceDemand = Buildings.Sum(e => e.type.workforce);
    }

    public void UpdateEnvironment(float dt) {
        foreach (var building in Buildings) {
            building.Update(dt, this);
        }
        EnvironmentalBeauty(dt);
        PollutionNaturalDecay(dt);
        Beauty.Smooth(dt);
        Pollution.Smooth(dt*PollutionDispersal);
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

    public void SelectCell() {
        SelectedCell = new Cell(HoverPoint);
        SelectedBuilding = Buildings.FirstOrDefault(e => e.IsOccupying(SelectedCell));
    }

    public void SaveGame() {
        var path = Application.persistentDataPath + "/save.json";
        Debug.Log($"Saving to ${path}");
        var data = JsonUtility.ToJson(SavedGame.Create(this), prettyPrint: true);
        File.WriteAllText(path, data);
    }

    public void LoadGame() {
        var path = Application.persistentDataPath + "/save.json";
        Debug.Log($"Loading from ${path}");
        if (File.Exists(path)) {
            var json = File.ReadAllText(path);
            var state = JsonUtility.FromJson<SavedGame>(json);
            Store = state.Store;
            Population = state.Population;
            Terrain = new TerrainGrid(state.Height, state.Water);
            Beauty.field = state.Beauty.field;
            Pollution.field = state.Pollution.field;
            Buildings = state.Buildings.Select(SavedBuilding.Instantiate).ToList();
            CalcNeighbourDistances();
            UpdateEnvironment(0.01f);
        }

    }
}

[Serializable]
public class SavedGame {
    public Attr Store;
    public int Population;
    public Field Height;
    public Field Water;
    public Field Beauty;
    public Field Pollution;
    public SavedBuilding[] Buildings;
    public static SavedGame Create(Game game) => new SavedGame {
        Store = game.Store,
        Population = game.Population,
        Height = game.Terrain.Height,
        Water = game.Terrain.Water,
        Beauty = game.Beauty,
        Pollution = game.Pollution,
        Buildings = game.Buildings.Select(SavedBuilding.Create).ToArray()
    };
}

[Serializable]
public class SavedBuilding {
    public Cell Cell;
    public string Type;
    public float BuildProgress;
    public bool IsEnabled;
    public static SavedBuilding Create(Building e) => new SavedBuilding {
        Cell = e.Cell,
        Type = e.type.name,
        BuildProgress = clamp(0, 1, e.BuildProgress),
        IsEnabled = e.IsEnabled
    };
    public static Building Instantiate(SavedBuilding e) => new Building {
        Cell = e.Cell,
        BuildProgress = e.BuildProgress,
        type = Definitions.types.First(t => t.name == e.Type)
    };
}
