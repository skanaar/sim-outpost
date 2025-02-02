﻿using System;
using System.IO;
using System.Linq;
using UnityEngine;
using static Util;

public class Persister {

    public static void SaveGame(Game game) {
        var path = Application.persistentDataPath + "/save.json";
        Debug.Log($"Saving to ${path}");
        var data = JsonUtility.ToJson(SavedGame.Create(game), prettyPrint: true);
        File.WriteAllText(path, data);
    }

    public static void LoadGame(Game game) {
        var path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path)) {
            Debug.Log($"Loading from ${path}");
            var json = File.ReadAllText(path);
            var g = JsonUtility.FromJson<SavedGame>(json);
            game.Store = g.Store;
            game.Population = g.Population;
            game.Terrain = new TerrainGrid(new Field(g.Height), new Field(g.Water), game.Wells); // TODO wells
            game.Entities = g.Entities.Select(SavedEntity.Instantiate).ToList();
            game.Beauty = new Field(g.Beauty);
            game.Pollution = new Field(g.Pollution);
            game.Buildings = g.Buildings.Select(SavedBuilding.Instantiate).ToList();
            game.CalcNeighbourDistances();
            game.UpdateEnvironment(0.01f);
        }
    }

    [Serializable]
    public class SavedGame {
        public Attr Store;
        public int Population;
        public float[] Height;
        public float[] Water;
        public float[] Beauty;
        public float[] Pollution;
        public SavedBuilding[] Buildings;
        public SavedEntity[] Entities;
        public static SavedGame Create(Game game) => new SavedGame {
            Store = game.Store,
            Population = game.Population,
            Height = game.Terrain.Height.RawData,
            Water = game.Terrain.Water.RawData,
            Beauty = game.Beauty.RawData,
            Pollution = game.Pollution.RawData,
            Buildings = game.Buildings.Select(SavedBuilding.Create).ToArray(),
            Entities = game.Entities.Select(SavedEntity.Create).ToArray()
        };
    }

    [Serializable]
    public class SavedBuilding {
        public Cell Cell;
        public string Type;
        public float BuildProgress;
        public bool IsEnabled;
        public static SavedBuilding Create(Building e) => new SavedBuilding {
            Cell = e.Cell,
            Type = e.type.name,
            BuildProgress = clamp(0, 1, e.BuildProgress),
            IsEnabled = e.IsEnabled
        };
        public static Building Instantiate(SavedBuilding e) => new Building {
            Cell = e.Cell,
            BuildProgress = e.BuildProgress,
            type = Definitions.types.First(t => t.name == e.Type)
        };
    }

    [Serializable]
    public class SavedEntity {
        public Vector3 Pos;
        public string Type;
        public float Age;
        public bool IsEnabled;
        public static SavedEntity Create(Entity e) => new SavedEntity {
            Pos = e.Pos,
            Type = e.Type.Name,
            Age = e.Age
        };
        public static Entity Instantiate(SavedEntity e) => new Entity {
            Pos = e.Pos,
            Age = e.Age,
            Type = Definitions.EntityTypes.First(t => t.Name == e.Type)
        };
    }
}
