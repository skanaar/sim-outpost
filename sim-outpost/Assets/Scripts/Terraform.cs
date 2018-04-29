public class Terraform {

    public Game Game;

    public void LevelTerrain(Cell cell, int size) {
        if (Game.Store.energy > 10) {
            cell = Game.Terrain.Height.CellWithin(cell);
            Game.Store += new Attr { energy = -10 };
            var average = Game.Terrain.Height.Average(cell.i, cell.j, size, size);
            for (int i = 0; i <= size; i++) {
                for (int j = 0; j <= size; j++) {
                    Cell c = Game.Terrain.Height.CellWithin(cell.Add(i, j));
                    Game.Terrain.Height[c] = average;
                }
            }
            Game.Terrain.Height.ApplyChanges();
            Game.TerrainController?.OnTerrainChange();
        }
    }

    public void AdjustTerrain(Cell cell, int size, float delta) {
        var cost = 10*size*size;
        if (Game.Store.energy > cost) {
            Game.Store += new Attr { energy = -cost };
            for (int i = 0; i <= size; i++) {
                for (int j = 0; j <= size; j++) {
                    Cell c = Game.Terrain.Height.CellWithin(cell.Add(i, j));
                    Game.Terrain.Height[c] += delta;
                }
            }
            Game.Terrain.Height.ApplyChanges();
            Game.TerrainController?.OnTerrainChange();
        }
    }
}
