using System.Collections.Generic;

public class Definitions {
    public static BuildingType reactor = new BuildingType {
        name = "Reactor",
        model = "cylinder",
        height = 2f,
        w = 1,
        h = 1,
        turnover = new Attr { energy = 2, fuel = -1 },
        cost = new Attr { metal = 30 },
        buildTime = 10
    };
    public static BuildingType greenhouse = new BuildingType {
        name = "Green house",
        model = "block",
        height = 0.5f,
        w = 1,
        h = 1,
        turnover = new Attr { food = 3, oxygen = 1 },
        cost = new Attr { biosludge = 3, metal = 2 },
        buildTime = 2
    };
    public static BuildingType habitat = new BuildingType {
        name = "Habitat",
        model = "capsule",
        height = 1f,
        w = 1,
        h = 1,
        turnover = new Attr { food = -2, oxygen = -1 },
        cost = new Attr { food = 3, metal = 10 },
        buildTime = 8
    };

    public static List<BuildingType> types = new List<BuildingType> {
        reactor,
        greenhouse,
        habitat
    };
}
