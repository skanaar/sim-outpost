public class Terraform {

    public Game Game;

    public void LevelTerrain(Cell cell) {
        if (Game.Store.energy > 10) {
            cell = Game.Terrain.Height.CellWithin(cell);
            Game.Store += new Attr { energy = -10 };
            var grid = Game.Terrain.Height;
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
            Game.TerrainController?.OnTerrainChange();
        }
    }

    public void AdjustTerrain(Cell cell, float delta) {
        if (Game.Store.energy > 10) {
            cell = Game.Terrain.Height.CellWithin(cell);
            Game.Store += new Attr { energy = -10 };
            Game.Terrain.Height[cell.i + 0, cell.j + 0] += delta;
            Game.Terrain.Height[cell.i + 1, cell.j + 0] += delta;
            Game.Terrain.Height[cell.i + 0, cell.j + 1] += delta;
            Game.Terrain.Height[cell.i + 1, cell.j + 1] += delta;
            Game.TerrainController?.OnTerrainChange();
        }
    }
}
