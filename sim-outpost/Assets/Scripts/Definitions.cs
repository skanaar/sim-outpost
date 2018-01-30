using System.Collections.Generic;
using UnityEngine;

public class Definitions {
    
    public static Attr StartingCommodities = new Attr {
        metal = 100, biosludge = 20, fuel = 100
    };

    public static Dictionary<string, Mesh> models = new Dictionary<string, Mesh> {
        { "Reactor", Models.pillar.mesh },
        { "Turbine", Models.turbine },
        { "Solar", Models.solar },
        { "Greenhouse", Models.greenhouse },
        { "Harvester", Models.greenhouse },
        { "Extractor", Models.greenhouse },
        { "Habitat", Models.greenhouse },
        { "Atmoplant", Models.atmoplant.mesh },
        { "Syntactor", Models.syntactor.mesh }
    };

    public static Dictionary<string, Color> colors = new Dictionary<string, Color> {
        { "Reactor", new Color(0.5f, 0.5f, 1f) },
        { "Turbine", new Color(1f, 1f, 1f) },
        { "Solar", new Color(1f, 1f, 0) },
        { "Greenhouse", new Color(0.5f, 1f, 0.5f) },
        { "Harvester", new Color(0.5f, 1f, 0.5f) },
        { "Extractor", new Color(0.75f, 0.75f, 1f) },
        { "Habitat", new Color(1f, 0.5f, 0.5f)},
        { "Atmoplant", new Color(0.8f, 0.8f, 1f)},
        { "Syntactor", new Color(1f, 0.5f, 0f)}
    };

    public static ItemType tree = new ItemType {
        Name = "Tree",
        Kind = ItemKind.Plant,
        Contents = new Attr{ biomass = 10, biosludge = 10 },
        MaxAge = 10
    };

    public static BuildingType reactor = new BuildingType {
        name = "Reactor",
        turnover = new Attr { energy = 2, fuel = -1 },
        cost = new Attr { metal = 30 },
        buildTime = 10
    };
    public static BuildingType turbine = new BuildingType {
        name = "Turbine",
        turnover = new Attr { energy = 1 },
        cost = new Attr { metal = 10 },
        buildTime = 2,
        Aspects = new List<BuildingAspect> { new WindTurbineAspect() }
    };
    public static BuildingType solar = new BuildingType {
        name = "Solar",
        turnover = new Attr { energy = 2 },
        cost = new Attr { metal = 1 },
        buildTime = 2,
        Aspects = new List<BuildingAspect> { new SolarEnergyAspect() }
    };
    public static BuildingType extractor = new BuildingType {
        name = "Extractor",
        turnover = new Attr { energy = -2, metal = 1 },
        cost = new Attr { energy = 20 },
        buildTime = 3
    };
    public static BuildingType harvester = new BuildingType {
        name = "Harvester",
        turnover = new Attr { energy = -1 },
        cost = new Attr { metal = 10 },
        buildTime = 3,
        Aspects = new List<BuildingAspect>{ new TreeHarvesterAspect() }
    };
    public static BuildingType greenhouse = new BuildingType {
        name = "Greenhouse",
        turnover = new Attr { food = 3, oxygen = 1 },
        cost = new Attr { biosludge = 3, metal = 2 },
        buildTime = 2
    };
    public static BuildingType habitat = new BuildingType {
        name = "Habitat",
        turnover = new Attr { food = -2, oxygen = -1 },
        cost = new Attr { food = 3, metal = 10 },
        buildTime = 8
    };
    public static BuildingType atmoplant = new BuildingType {
        name = "Atmoplant",
        turnover = new Attr { energy = -10, oxygen = 2 },
        cost = new Attr { energy = 200, metal = 10 },
        buildTime = 8
    };
    public static BuildingType syntactor = new BuildingType {
        name = "Syntactor",
        turnover = new Attr { biosludge = -10, chems = 1, fuel = 2 },
        cost = new Attr { biosludge = 3, metal = 10 },
        buildTime = 4
    };

    public static List<BuildingType> types = new List<BuildingType> {
        reactor,
        turbine,
        solar,
        greenhouse,
        habitat,
        harvester,
        extractor,
        atmoplant,
        syntactor
    };
}
