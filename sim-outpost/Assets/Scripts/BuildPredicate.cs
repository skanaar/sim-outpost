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
            if (!empty) return false;
            for (int i = 0; i <= type.w; i++) {
                for (int j = 0; j <= type.h; j++) {
                    Cell c = game.Terrain.Height.CellWithin(cell.Add(i, j));
                    var flat = game.Terrain.Slope[c] < Game.MaxBuildingSlope;
                    var water = game.Terrain.Water.Maximum(cell.i,cell.j,type.w,type.h);
                    if (water > Game.WaterLowThreshold || !flat) {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class Windy : BuildPredicate {
        public override bool CanBuild(BuildingType type, Cell cell, Game game) {
            return IsEmpty(type, cell, game) && game.Terrain.PeakProminence[cell] > 0;
        }
    }
}
