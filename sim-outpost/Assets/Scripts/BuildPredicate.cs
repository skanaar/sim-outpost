using System.Linq;

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
            var flat = game.Terrain.Slope[cell] < Game.MaxBuildingSlope;
            var water = game.Terrain.Water.Maximum(cell.i, cell.j, type.w, type.h);
            return empty && flat && (water < Game.WaterLowThreshold);
        }
    }

    public class Windy : BuildPredicate {
        public override bool CanBuild(BuildingType type, Cell cell, Game game) {
            return IsEmpty(type, cell, game) && game.Terrain.PeakProminence[cell] > 0;
        }
    }
}
