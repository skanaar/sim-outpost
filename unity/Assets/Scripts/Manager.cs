using System.Collections.Generic;

public class Manager {

    static BuildingType reactor = new BuildingType {
        name = "Reactor",
        model = "cylinder",
        height = 2f,
        w = 1,
        h = 1,
        turnover = new Attr { energy = 2, fuel = -1 }
    };
    static BuildingType greenhouse = new BuildingType {
        name = "Green house",
        model = "block",
        height = 0.5f,
        w = 1,
        h = 1,
        turnover = new Attr { food = 3, oxygen = 1 }
    };
    static BuildingType habitat = new BuildingType {
        name = "Habitat",
        model = "capsule",
        height = 1f,
        w = 1,
        h = 1,
        turnover = new Attr { food = -2, oxygen = -1 }
    };

    public TerrainGrid Terrain { get; } = new TerrainGrid(21);
    public List<Building> Buildings { get; } = new List<Building>{
        new Building{ type = reactor, i = 1, j = 3 },
        new Building{ type = greenhouse, i = 6, j = 6 },
        new Building{ type = greenhouse, i = 5, j = 4 },
        new Building{ type = habitat, i = 8, j = 8 },
    };

    public static Manager Instance { get; private set; } = new Manager();

    public Attr Commodities { get; set; } = new Attr{ fuel = 100 };

    public void Update(float deltaTime) {
        Terrain.Update(deltaTime);
        foreach (var building in Buildings) {
            building.enabled = (Attr.Zero <= building.type.turnover + Commodities);
            if (building.enabled) {
                Commodities = Commodities + deltaTime * building.type.turnover;
            }
        }
    }
}
