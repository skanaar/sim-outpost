using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Util;

public class Building : Killable {
    public BuildingType type;
    public Cell Cell;
    public bool IsEnabled = true;
    public bool IsSupplied = true;
    public float BuildProgress;
    public Attr LastTurnover = Attr.Zero;
    public bool IsDead { get; set; }
    public GameObject GameObject { get; set; }

    public bool IsProducing => BuildProgress >= 1 && IsEnabled && IsSupplied;
    public bool IsOccupying(Cell cell) {
        return cell.i >= Cell.i && cell.i < Cell.i + type.w &&
               cell.j >= Cell.j && cell.j < Cell.j + type.h;
    }

    public void Update(float dt, Game game) {
        if (BuildProgress >= 1) {
            foreach (var aspect in type.Aspects) {
                aspect.Update(dt, this, game);
            }
        }
    }

    public void UpdateConstructing(float dt, Game game) {
        if (BuildProgress < 1) {
            ConstructionAspect.Instance.Update(dt, this, game);
        }
    }
}

public class BuildingType {
    public string name;
    public int w = 1;
    public int h = 1;
    public Attr turnover;
    public Attr cost;
    public int beds;
    public int workforce;
    public float beauty;
    public float pollution;
    public float buildTime;
    public List<BuildingAspect> Aspects = new List<BuildingAspect>();
    public BuildPredicate Predicate = new BuildPredicate.FlatGround();
    public BuildingType(string name, params BuildingAspect[] aspects) {
        this.name = name;
        Aspects = aspects.ToList();
    }
}

public interface BuildingAspect {
    void Update(float dt, Building self, Game game);
}

public class ConstructionAspect : BuildingAspect {
    public static ConstructionAspect Instance = new ConstructionAspect();
    public void Update(float dt, Building self, Game game) {
        var deltaCost = (-dt / self.type.buildTime) * self.type.cost;
        self.IsSupplied = (Attr.Zero <= deltaCost + game.Store);
        if (self.IsSupplied && self.IsEnabled) {
            game.Store += deltaCost;
            self.BuildProgress += dt / self.type.buildTime;
        }
    }
}

public class TurnoverAspect : BuildingAspect {
    public void Update(float dt, Building self, Game game) {
        self.IsSupplied = (Attr.Zero <= self.type.turnover+game.Store);
        if (self.IsProducing) {
            self.LastTurnover = self.type.turnover;
            game.Store += dt * self.LastTurnover;
        }
    }
}

public class BeautyAspect : BuildingAspect {
    public float Amount { get; set; }
    public BeautyAspect(float amount) {
        Amount = amount;
    }
    public void Update(float dt, Building self, Game game) {
        game.Beauty[self.Cell] += dt * Amount;
    }
}

public class PollutingAspect : BuildingAspect {
    public float Amount { get; set; }
    public PollutingAspect(float amount) {
        Amount = amount;
    }
    public void Update(float dt, Building self, Game game) {
        if (self.IsProducing) {
            game.Pollution[self.Cell] = max(0, game.Pollution[self.Cell] + dt * Amount);
        }
    }
}

public class MigrationAspect : BuildingAspect {
    int LandingPeriod = 10;
    int LandingPollution = 5;
    public void Update(float dt, Building self, Game game) {
        var isLaunchWindow = game.ShouldTrigger(dt, LandingPeriod);
        if (self.IsProducing && isLaunchWindow) {
            if (game.Population < game.Beds && game.Population < game.WorkforceDemand) {
                game.Population += 1;
                game.Pollution[self.Cell] += LandingPollution;
            }
            if (game.Population > game.Beds || game.Population > game.WorkforceDemand) {
                game.Population -= 1;
                game.Pollution[self.Cell] += LandingPollution;
            }
        }
    }
}

public class TreeHarvesterAspect : BuildingAspect {
    float Range => 7;
    public void Update(float dt, Building self, Game game) {
        if (self.IsProducing && Random.value * 2 < dt) {
            var pos = self.Cell.ToVector;
            var tree = game.Entities
               .Where(e => e.Type.Name == "Tree")
               .FirstOrDefault(e => (e.Pos - pos).magnitude < Range);
            if (tree != null) {
                tree.IsDead = true;
                self.LastTurnover = (tree.Age/tree.Type.MaxAge)*tree.Type.Contents;
                game.Store += self.LastTurnover;
            }
        }
    }
}

public class WindTurbineAspect : BuildingAspect {
    public void Update(float dt, Building self, Game game) {
        if (self.IsProducing) {
            var height = clamp(0, 1, 0.25f + game.Terrain.PeakProminence[self.Cell]);
            self.LastTurnover = height * self.type.turnover;
            game.Store += dt * self.LastTurnover;
        }
    }
}

public class SolarPowerAspect : BuildingAspect {
    public void Update(float dt, Building self, Game game) {
        if (self.IsProducing) {
            var influx = 0.5f + 0.5f * sin(game.Time * 0.1f);
            self.LastTurnover = influx * self.type.turnover;
            game.Store += dt * self.LastTurnover;
        }
    }
}

public class HydroPowerAspect : BuildingAspect {
    int Range = 4;
    float MaxIntake = 0.5f;
    float EnergyPerWater = 10f;
    public string Name => "Hydro Power";
    public void Update(float dt, Building self, Game game) {
        if (self.IsProducing) {
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
                        lowCell = new Cell(i, j);
                    }
                }
            }
            for (int i = max(0, p.i-Range); i < min(res-1, p.i+Range); i++) {
                for (int j = max(0, p.j-Range); j < min(res-1, p.j+Range); j++) {
                    var water = ground[i, j] + game.Terrain.Water[i, j];
                    if (low < water) {
                        var intake = min(game.Terrain.Water[i, j], MaxIntake);
                        game.Terrain.Water[i, j] -= dt * intake;
                        game.Terrain.Water[lowCell.i, lowCell.j] += dt * intake;
                        self.LastTurnover = new Attr { energy = intake * EnergyPerWater };
                        game.Store += dt * self.LastTurnover;
                    }
                }
            }
        }
    }
}

public abstract class BuildPredicate {
    public abstract bool CanBuild(BuildingType type, Cell cell, Game game);
    public static bool IsEmpty(BuildingType type, Cell cell, Game game) {
        return game.Buildings.All(e => !e.IsOccupying(cell));
    }

    public class Upgrade : BuildPredicate {
        public string From;
        public override bool CanBuild(BuildingType type, Cell cell, Game game) {
            return game.Buildings.Any(e => e.Cell == cell && e.type.name == From);
        }
    }

    public class FlatGround : BuildPredicate {
        public override bool CanBuild(BuildingType type, Cell cell, Game game) {
            var empty = IsEmpty(type, cell, game);
            var flat = game.Terrain.Slope[cell] < Game.Instance.MaxBuildingSlope;
            var water = game.Terrain.Water.Maximum(cell.i, cell.j, type.w, type.h);
            return empty && flat && (water < game.WaterLowThreshold);
        }
    }

    public class Windy : BuildPredicate {
        public override bool CanBuild(BuildingType type, Cell cell, Game game) {
            return IsEmpty(type, cell, game) && game.Terrain.PeakProminence[cell] > 0;
        }
    }
}
