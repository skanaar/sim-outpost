using System.Collections.Generic;
using UnityEngine;
using static Util;

public class Definitions {
    
    public static Attr StartingCommodities = new Attr {
        metal = 100, biosludge = 20, fuel = 100, energy = 100
    };

    public struct Look {
        public Mesh Mesh;
        public Color Color;
        public Look(Mesh mesh, Color color) {
            Mesh = mesh;
            Color = color;
        }
    }

    public static Dictionary<string, Look> models = new Dictionary<string, Look> {
        { "Reactor", new Look(Models.pillar.mesh, rgb(0x88F)) },
        { "Turbine", new Look(Models.turbine, rgb(0xFFF)) },
        { "Solar", new Look(Models.solar, rgb(0xFF0)) },
        { "Greenhouse", new Look(Models.greenhouse, rgb(0x8F8)) },
        { "Harvester", new Look(Models.greenhouse, rgb(0x8F8)) },
        { "Extractor", new Look(Models.greenhouse, rgb(0xCCF)) },
        { "Habitat", new Look(Models.greenhouse, rgb(0xF88)) },
        { "Atmoplant", new Look(Models.atmoplant.mesh, rgb(0xCCF)) },
        { "Syntactor", new Look(Models.syntactor.mesh, rgb(0xF80)) },
        { "Hydro Power", new Look(Models.syntactor.mesh, rgb(0xF8F)) }
    };

    public static ItemType tree = new ItemType {
        Name = "Tree",
        Kind = ItemKind.Plant,
        Contents = new Attr { biomass = 10, biosludge = 10 },
        MaxAge = 10
    };
    
    public static List<BuildingType> types = new List<BuildingType> {
        new BuildingType("Reactor", new TurnoverAspect()) {
            turnover = new Attr { energy = 2, fuel = -1 },
            cost = new Attr { metal = 30 },
            buildTime = 10
        },
        new BuildingType("Turbine", new WindTurbineAspect()) {
            turnover = new Attr { energy = 1 },
            cost = new Attr { metal = 10 },
            buildTime = 2
        },
        new BuildingType("Solar", new SolarPowerAspect()) {
            turnover = new Attr { energy = 2 },
            cost = new Attr { metal = 1 },
            buildTime = 2,
        },
        new BuildingType("Extractor", new TurnoverAspect()) {
            turnover = new Attr { energy = -2, metal = 1 },
            cost = new Attr { energy = 20 },
            buildTime = 3
        },
        new BuildingType("Harvester", new TreeHarvesterAspect()) {
            turnover = new Attr { energy = -1 },
            cost = new Attr { metal = 10 },
            buildTime = 3
        },
        new BuildingType("Greenhouse", new TurnoverAspect()) {
            turnover = new Attr { food = 3, oxygen = 1 },
            cost = new Attr { biosludge = 3, metal = 2 },
            buildTime = 2
        },
        new BuildingType("Habitat", new TurnoverAspect()) {
            turnover = new Attr { food = -2, oxygen = -1 },
            cost = new Attr { food = 3, metal = 10 },
            buildTime = 8
        },
        new BuildingType("Atmoplant", new TurnoverAspect()) {
            turnover = new Attr { energy = -10, oxygen = 2 },
            cost = new Attr { energy = 200, metal = 10 },
            buildTime = 8
        },
        new BuildingType("Syntactor", new TurnoverAspect()) {
            turnover = new Attr { biosludge = -10, chems = 1, fuel = 2 },
            cost = new Attr { biosludge = 3, metal = 10 },
            buildTime = 4
        },
        new BuildingType("Hydro Power", new HydroPowerAspect()) {
            turnover = new Attr(),
            cost = new Attr { metal = 10 },
            buildTime = 2
        }
    };
}
