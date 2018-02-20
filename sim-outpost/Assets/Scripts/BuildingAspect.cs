using System.Linq;
using UnityEngine;
using static Util;

public interface BuildingAspect {
    void Update(float dt, Building self, Game game);
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
            if (game.Population > game.Beds || game.Population > 2*game.WorkforceDemand) {
                game.Population -= 1;
                game.Pollution[self.Cell] += LandingPollution;
            }
        }
    }
}

public class TreeHarvesterAspect : BuildingAspect {
    float Range = 7;
    float Period = 3;
    public void Update(float dt, Building self, Game game) {
        if (self.IsProducing && ShouldTrigger(game.Time, dt, Period)) {
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