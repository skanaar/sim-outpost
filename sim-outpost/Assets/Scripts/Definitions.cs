using System.Collections.Generic;
using UnityEngine;

public class Definitions {
    
    public static Attr StartingCommodities = new Attr { metal = 100, biosludge = 20, fuel = 100 };

    public static Dictionary<string, PrimitiveType> models = new Dictionary<string, PrimitiveType> {
        { "Greenhouse", PrimitiveType.Cube },
        { "Reactor", PrimitiveType.Cylinder },
        { "Harvester", PrimitiveType.Capsule },
        { "Extractor", PrimitiveType.Cylinder },
        { "Habitat", PrimitiveType.Capsule }
    };

    public static Dictionary<string, Color> colors = new Dictionary<string, Color> {
        { "Greenhouse", new Color(0.5f, 1f, 0.5f) },
        { "Reactor", new Color(0.5f, 0.5f, 1f) },
        { "Harvester", new Color(0.5f, 1f, 0.5f) },
        { "Extractor", new Color(0.75f, 0.75f, 1f) },
        { "Habitat", new Color(1f, 0.5f, 0.5f)}
    };

    public static BuildingType reactor = new BuildingType {
        name = "Reactor",
        model = "cylinder",
        height = 1f,
        w = 1,
        h = 1,
        turnover = new Attr { energy = 2, fuel = -1 },
        cost = new Attr { metal = 30 },
        buildTime = 10
    };
    public static BuildingType extractor = new BuildingType {
        name = "Extractor",
        model = "cylinder",
        height = 0.5f,
        w = 1,
        h = 1,
        turnover = new Attr { energy = -2, metal = 1 },
        cost = new Attr { metal = 10 },
        buildTime = 3
    };
    public static BuildingType harvester = new BuildingType {
        name = "Harvester",
        model = "capsule",
        height = 0.5f,
        w = 1,
        h = 1,
        turnover = new Attr { energy = -1, biosludge = 1 },
        cost = new Attr { metal = 10 },
        buildTime = 3
    };
    public static BuildingType greenhouse = new BuildingType {
        name = "Greenhouse",
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
        habitat,
        harvester,
        extractor
    };
}
