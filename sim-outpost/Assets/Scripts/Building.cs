using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Util;

public class Building {
    public BuildingType type;
    public Cell Cell;
    public bool IsEnabled = true;
    public bool IsSupplied;
    public float BuildProgress;
    public GameObject GameObject;

    public bool IsOccupying(Cell cell) {
        return cell.i >= Cell.i && cell.i < Cell.i + type.w &&
               cell.j >= Cell.j && cell.j < Cell.j + type.h;
    }
}

public class BuildingType {
    public string name;
    public int w = 1;
    public int h = 1;
    public Attr turnover;
    public Attr cost;
    public float buildTime;
    public List<BuildingAspect> Aspects = new List<BuildingAspect>();
    public BuildingType(string name, params BuildingAspect[] aspects) {
        this.name = name;
        Aspects = aspects.ToList();
    }
}

public interface BuildingAspect {
    void Update(float dt, Building self, Manager game);
}
public class TurnoverAspect : BuildingAspect {
    public void Update(float dt, Building self, Manager game) {
        self.IsSupplied = (Attr.Zero <= self.type.turnover+game.Store);
        if (self.IsSupplied && self.IsEnabled) {
            game.Store += dt * self.type.turnover;
        }
    }
}

public class TreeHarvesterAspect : BuildingAspect {
    float Range => 7;
    public void Update(float dt, Building self, Manager game) {
        if (self.IsEnabled && self.IsSupplied && Random.value * 2 < dt) {
            var pos = self.Cell.ToVector;
            var tree = game.Items
               .Where(e => e.Type.Kind == ItemKind.Plant)
               .FirstOrDefault(e => (e.Pos - pos).magnitude < Range);
            if (tree != null) {
                tree.IsDead = true;
                game.Store += (tree.Age/tree.Type.MaxAge)*tree.Type.Contents;
            }
        }
    }
}

public class WindTurbineAspect : BuildingAspect {
    public void Update(float dt, Building self, Manager game) {
        if (self.IsEnabled && self.IsSupplied) {
            var height = clamp(0, 1, 0.25f + game.Terrain.PeakProminence[self.Cell]);
            game.Store += dt * height * self.type.turnover;
        }
    }
}

public class SolarPowerAspect : BuildingAspect {
    public void Update(float dt, Building self, Manager game) {
        if (self.IsEnabled && self.IsSupplied) {
            var influx = 0.5f + 0.5f * sin(game.Time * 0.1f);
            game.Store += dt * influx * self.type.turnover;
        }
    }
}

public class HydroPowerAspect : BuildingAspect {
    int Range = 4;
    float MaxIntake = 0.5f;
    float EnergyPerWater = 10f;
    public string Name => "Hydro Power";
    public void Update(float dt, Building self, Manager game) {
        if (self.IsEnabled && self.IsSupplied) {
            var ground = game.Terrain.Height;
            var res = game.Terrain.Height.Res;
            var p = self.Cell;
            var low = Mathf.Infinity;
            var lowCell = new Cell();
            for (int i = max(0, p.i-Range); i < min(res-1, p.i+Range); i++) {
                for (int j = max(0, p.j-Range); j < min(res-1, p.j+Range); j++) {
                    var level = ground[i, j] + game.Terrain.Water[i, j];
                    if (level < low) {
                        low = level;
                        lowCell = new Cell { i = i, j = j };
                    }
                }
            }
            for (int i = max(0, p.i-Range); i < min(res-1, p.i+Range); i++) {
                for (int j = max(0, p.j-Range); j < min(res-1, p.j+Range); j++) {
                    var water = ground[i, j] + game.Terrain.Water[i, j];
                    if (low < water) {
                        var intake = dt * min(game.Terrain.Water[i, j], MaxIntake);
                        game.Terrain.Water.field[i, j] -= intake;
                        game.Terrain.Water.field[lowCell.i, lowCell.j] += intake;
                        game.Store += new Attr { energy = intake * EnergyPerWater };
                    }
                }
            }
        }
    }
}
