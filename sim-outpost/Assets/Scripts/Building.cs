using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Util;

public class Building : Killable {
    public BuildingType type;
    public Cell Cell;
    public bool IsEnabled = true;
    public bool IsSupplied = true;
    public float BuildProgress;
    public Attr LastTurnover = Attr.Zero;
    public bool IsDead { get; set; }
    public GameObject GameObject { get; set; }

    public bool IsProducing => BuildProgress >= 1 && IsEnabled && IsSupplied;
    public bool IsOccupying(Cell cell) {
        return cell.i >= Cell.i && cell.i < Cell.i + type.w &&
               cell.j >= Cell.j && cell.j < Cell.j + type.h;
    }

    public void Update(float dt, Game game) {
        if (BuildProgress >= 1) {
            foreach (var aspect in type.Aspects) {
                aspect.Update(dt, this, game);
            }
        }
    }

    public void UpdateConstructing(float dt, Game game) {
        if (BuildProgress < 1) {
            var deltaCost = (-dt / this.type.buildTime) * this.type.cost;
            this.IsSupplied = (Attr.Zero <= deltaCost + game.Store);
            if (this.IsSupplied && this.IsEnabled) {
                game.Store += deltaCost;
                this.BuildProgress += dt / this.type.buildTime;
            }
        }
    }
}

public class BuildingType {
    public string name;
    public int w = 1;
    public int h = 1;
    public Attr turnover;
    public Attr cost;
    public int beds;
    public int workforce;
    public float beauty;
    public float pollution;
    public float buildTime;
    public List<BuildingAspect> Aspects = new List<BuildingAspect>();
    public BuildPredicate Predicate = new BuildPredicate.FlatGround();
    public BuildingType(string name, params BuildingAspect[] aspects) {
        this.name = name;
        Aspects = aspects.ToList();
    }
}
