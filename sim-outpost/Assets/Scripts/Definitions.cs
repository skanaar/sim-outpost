using System.Collections.Generic;
using UnityEngine;

public class Definitions {
    
    public static Attr StartingCommodities = new Attr {
        metal = 100, biosludge = 20, fuel = 100, energy = 100, water = 100
    };

    public static EntityType treeCollector = new EntityType {
        Name = "Collector",
        Aspects = new EntityAspect[]{
            new TreeCollectorAspect { Home = new Vector3(10, 10, 10) }
        }
    };

    public static EntityType tree = new EntityType {
        Name = "Tree",
        Contents = new Attr { biomass = 10, biosludge = 10 },
        Aspects = new EntityAspect[]{ new BeautifulCreature { Beauty = 2 } },
        MaxAge = 10
    };
    
    public static List<BuildingType> types = new List<BuildingType> {
        new BuildingType("Relay", new TurnoverAspect(), new BeautyAspect(1)) {
            turnover = new Attr(),
            cost = new Attr { metal = 5 },
            buildTime = 2
        },
        new BuildingType("Reactor", new TurnoverAspect(), new PollutingAspect(1)) {
            turnover = new Attr { energy = 2, fuel = -1 },
            cost = new Attr { metal = 30 },
            buildTime = 10
        },
        new BuildingType("Turbine", new WindTurbineAspect()) {
            turnover = new Attr { energy = 1 },
            cost = new Attr { metal = 10 },
            buildTime = 2,
            Predicate = new BuildPredicate.Windy()
        },
        new BuildingType("Solar", new SolarPowerAspect()) {
            turnover = new Attr { energy = 2 },
            cost = new Attr { metal = 1 },
            buildTime = 2,
        },
        new BuildingType("Extractor", new TurnoverAspect(), new PollutingAspect(3)) {
            turnover = new Attr { energy = -2, metal = 1 },
            cost = new Attr { energy = 20 },
            buildTime = 3
        },
        new BuildingType("Harvester", new TreeHarvesterAspect()) {
            turnover = new Attr { energy = -1 },
            cost = new Attr { metal = 10 },
            buildTime = 3
        },
        new BuildingType("Greenhouse", new TurnoverAspect(), new BeautyAspect(0.1f)) {
            turnover = new Attr { food = 2, oxygen = 1 },
            cost = new Attr { biosludge = 3, metal = 2 },
            buildTime = 2
        },
        new BuildingType("Hydroponics", new TurnoverAspect()) {
            turnover = new Attr { food = 4, water = -1, oxygen = 1 },
            cost = new Attr { water = 10 },
            buildTime = 1,
            Predicate = new BuildPredicate.Upgrade{ From = "Greenhouse" }
        },
        new BuildingType("Habitat", new TurnoverAspect(), new PollutingAspect(0.1f)) {
            turnover = new Attr { food = -2, oxygen = -1 },
            cost = new Attr { food = 3, metal = 10 },
            beds = 10,
            buildTime = 8
        },
        new BuildingType("Atmoplant", new TurnoverAspect(), new PollutingAspect(1), new BeautyAspect(-2)) {
            turnover = new Attr { energy = -10, oxygen = 2 },
            cost = new Attr { energy = 200, metal = 10 },
            buildTime = 8
        },
        new BuildingType("Syntactor", new TurnoverAspect(),
                         new PollutingAspect(3), new BeautyAspect(-5)) {
            turnover = new Attr { biosludge = -10, chems = 1, fuel = 2 },
            cost = new Attr { biosludge = 3, metal = 10 },
            buildTime = 4
        },
        new BuildingType("Hydro Power", new HydroPowerAspect(), new BeautyAspect(-3)) {
            turnover = new Attr(),
            cost = new Attr { metal = 10 },
            buildTime = 2
        }
    };
}
