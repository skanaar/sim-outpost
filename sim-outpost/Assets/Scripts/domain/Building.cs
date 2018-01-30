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
}

public interface BuildingAspect {
    string Name { get; }
    void Update(float deltaTime, float time, Building self, Manager game);
}

public class TreeHarvesterAspect : BuildingAspect {
    float Range => 7;
    public string Name => "Tree Harvesting";
    public void Update(float deltaTime, float time, Building self, Manager game) {
        if (self.IsEnabled && self.IsSupplied && Random.value * 2 < deltaTime) {
            var pos = self.Cell.ToVector;
            var tree = game.Items
                           .Where(e => e.Type.Kind == ItemKind.Plant)
                           .FirstOrDefault(e => (e.Pos - pos).magnitude < Range);
            if (tree != null) {
                tree.IsDead = true;
                Attr contents = (tree.Age / tree.Type.MaxAge) * tree.Type.Contents;
                game.Commodities = game.Commodities + contents;
            }
        }
    }
}

public class WindTurbineAspect : BuildingAspect {
    public string Name => "Wind Power Generator";
    public void Update(float deltaTime, float time, Building self, Manager game) {
        if (self.IsEnabled && self.IsSupplied) {
            var height = game.Terrain.Height.GetCell(self.Cell);
            // remove added turnover and put back wind-adjusted turnover
            Attr generated = deltaTime * (height - 1f) * self.type.turnover;
            game.Commodities = game.Commodities + generated;
        }
    }
}

public class SolarEnergyAspect : BuildingAspect {
    public string Name => "Solar Power";
    public void Update(float deltaTime, float time, Building self, Manager game) {
        if (self.IsEnabled && self.IsSupplied) {
            var influx = 0.5f + 0.5f * sin(time * 0.1f);
            // remove added turnover and put back solar-cycle-adjusted turnover
            Attr generated = deltaTime * influx * self.type.turnover;
            game.Commodities = game.Commodities + generated;
        }
    }
}
