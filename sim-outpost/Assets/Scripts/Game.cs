using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Util;

public class Game {
    public static Game Instance { get; private set; } = new Game();

    internal TerrainCtrl TerrainController;

    // constants
    public float BuildRange = 5;
    public float ShoreBeauty = 2f;
    public float BeautyDecay = 0.95f;
    public float BeautyBackground = -0.3f;
    public float WaterLowThreshold = 0.05f;

    // game state
    public float Time => UnityEngine.Time.time;
    public TerrainGrid Terrain;
    public List<Building> Buildings { get; } = new List<Building>();
    public List<Item> Items { get; } = new List<Item>();
    public List<Mobile> Mobiles { get; } = new List<Mobile>();
    public Field NeighbourDist;
    public Field Beauty;
    public Attr Store { get; set; } = Definitions.StartingCommodities;
    public int Beds { get; set; } = 0;
    public int Population { get; set; } = 0;

    // ui state
    public float Zoom = 1.0f;
    public Vector3 HoverPoint { get; set; } = new Vector3(0, 0, 0);
    public Cell SelectedCell { get; set; } = new Cell(0, 0);
    public Building SelectedBuilding { get; set; } = null;

    public Game() {
        Terrain = new TerrainGrid(30);
        NeighbourDist = new Field(Terrain.Res);
        Beauty = new Field(Terrain.Res);
        Mobiles.Add(new Mobile {
            Pos = Vector3.zero,
            Aspects = Seq<MobileAspect>(new TreeCollectorAspect{ Home = Terrain.RandomPos() })
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
        foreach (var building in Buildings) {
            building.Update(dt, this);
        }
        foreach (var mob in Mobiles) {
            mob.Update(dt, this);
        }
        foreach (var item in Items) {
            var viability = Terrain.Viability[(int)item.Pos.x, (int)item.Pos.z];
            if (viability < 0.25f) {
                item.IsDead = true;
            }
            item.Age = Math.Min(item.Age + viability * dt, item.Type.MaxAge);
        }
        GrowTrees(dt);
        UpdateBeauty(dt);
        Beds = (int)Buildings.Sum(e => e.type.beds * compress(Beauty[e.Cell], halfAt: 5));
    }

    public void UpdateBeauty(float dt) {
        for (int x = 1; x < Terrain.Res; x++) {
            for (int y = 1; y < Terrain.Res; y++) {
                var w0 = Terrain.Water[x, y] > WaterLowThreshold;
                var wx = Terrain.Water[x-1, y] > WaterLowThreshold;
                var wy = Terrain.Water[x, y-1] > WaterLowThreshold;
                if (w0 != wx || w0 != wy) { // cell is shoreline
                    Beauty[x, y] += dt*ShoreBeauty;
                }
                Beauty[x, y] = max(0, Beauty[x, y]*lerp(1,BeautyDecay,dt) + dt*BeautyBackground);
            }
        }
        foreach (var building in Buildings) {
            Beauty[building.Cell] += dt * building.type.beauty;
        }
        foreach (var e in Items) {
            Beauty[Beauty.CellWithin(new Cell(e.Pos))] += dt * e.Type.Beauty;
        }
        Beauty.Smooth(dt);
    }

    public void GrowTrees(float dt) {
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
