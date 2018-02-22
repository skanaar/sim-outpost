using System.Collections.Generic;

public class Definitions {

    public static EntityType treeCollector = new EntityType {
        Name = "Collector",
        Aspects = new EntityAspect[]{
            new RandomMoveAspect()
        }
    };

    public static EntityType tree = new EntityType {
        Name = "Tree",
        Contents = new Attr { biomass = 10, biosludge = 10 },
        Aspects = new EntityAspect[]{
            new BeautyEntityAspect { Beauty = 5 },
            new SpawnAspect {
                MaxPollution = 0.1f, MaxWater = 0.1f, Distance = 4f, Period = 10f
            }
        },
        MaxAge = 40
    };

    public static List<EntityType> EntityTypes = new List<EntityType> {
        treeCollector,
        tree
    };

    public static List<BuildingType> types = new List<BuildingType> {
        new BuildingType("Hub") {
            turnover = new Attr(),
            cost = -1 * new Attr {
                metal = 100, biosludge = 20, fuel = 100, energy = 1000, water = 100
            },
            storage = 100 * Attr.Identity + new Attr{ energy = 900 },
            buildTime = 0.1f
        },
        new BuildingType("Relay", new TurnoverAspect(), new BeautyAspect(1)) {
            turnover = new Attr(),
            cost = new Attr { metal = 5 },
            buildTime = 2
        },
        new BuildingType("Turbine", new WindTurbineAspect()) {
            turnover = new Attr { energy = 1 },
            storage = new Attr { energy = 10 },
            cost = new Attr { metal = 10 },
            buildTime = 2,
            Predicate = new BuildPredicate.Windy()
        },
        new BuildingType("Harvester", new TreeHarvesterAspect()) {
            turnover = new Attr { energy = -1 },
            cost = new Attr { metal = 10 },
            workforce = 1,
            buildTime = 3
        },
        new BuildingType("Greenhouse", new TurnoverAspect(), new BeautyAspect(0.1f)) {
            turnover = new Attr { food = 2, oxygen = 1 },
            storage = new Attr { food = 5, oxygen = 5 },
            cost = new Attr { biosludge = 3, metal = 2 },
            workforce = 1,
            buildTime = 2
        },
        new BuildingType("Hydroponics", new TurnoverAspect()) {
            turnover = new Attr { food = 4, water = -1, oxygen = 1 },
            storage = new Attr { food = 20, water = 5, oxygen = 5 },
            cost = new Attr { water = 10 },
            buildTime = 1,
            workforce = 1,
            Predicate = new BuildPredicate.Upgrade{ From = "Greenhouse" }
        },
        new BuildingType("Habitat", new TurnoverAspect(), new PollutingAspect(0.1f)) {
            turnover = new Attr { food = -2, oxygen = -1 },
            storage = new Attr { food = 10, oxygen = 10 },
            cost = new Attr { food = 3, metal = 10 },
            beds = 10,
            buildTime = 8
        },
        new BuildingType("Hydro Power", new HydroPowerAspect(), new BeautyAspect(-3)) {
            turnover = new Attr(),
            storage = new Attr { energy = 20 },
            cost = new Attr { metal = 10 },
            workforce = 2,
            buildTime = 2
        },
        new BuildingType("Battery") {
            storage = new Attr { energy = 100 },
            cost = new Attr { metal = 10, chems = 20 },
            buildTime = 2,
        },

        // Size 2x
        new BuildingType("Solar", new SolarPowerAspect()) {
            turnover = new Attr { energy = 2 },
            storage = new Attr { energy = 5 },
            w = 2,
            h = 2,
            cost = new Attr { metal = 1 },
            buildTime = 2,
        },
        new BuildingType("Storage") {
            storage = 200 * new Attr {
                water = 1,
                credits = 1,
                ore = 1,
                metal = 1,
                oxygen = 1,
                fuel = 1,
                biosludge = 1,
                biomass = 1,
                chems = 1,
                food = 1,
                energy = 0
            },
            w = 2,
            h = 2,
            cost = new Attr { metal = 1 },
            buildTime = 2,
        },
        new BuildingType("Launch Pad", new PollutingAspect(1), new MigrationAspect()) {
            turnover = new Attr(),
            w = 2,
            h = 2,
            cost = new Attr { metal = 30 },
            buildTime = 10
        },
        new BuildingType("Extractor", new TurnoverAspect(), new PollutingAspect(3)) {
            turnover = new Attr { energy = -2, metal = 1 },
            storage = new Attr { metal = 50 },
            w = 2,
            h = 2,
            cost = new Attr { energy = 20 },
            workforce = 1,
            buildTime = 3
        },
        new BuildingType("Syntactor", new TurnoverAspect(),
                         new PollutingAspect(3), new BeautyAspect(-5)) {
            turnover = new Attr { biosludge = -10, chems = 1, fuel = 2 },
            storage = new Attr { chems = 20, fuel = 40 },
            w = 2,
            h = 2,
            cost = new Attr { biosludge = 3, metal = 10 },
            workforce = 4,
            buildTime = 4
        },
        new BuildingType("Atmoplant", new TurnoverAspect(), new PollutingAspect(1), new BeautyAspect(-2)) {
            turnover = new Attr { energy = -10, oxygen = 2 },
            storage = new Attr { oxygen = 40 },
            cost = new Attr { energy = 200, metal = 10 },
            workforce = 4,
            buildTime = 8
        }
    };
}
