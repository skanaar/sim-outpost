using UnityEngine;

public interface EntityAspect {
    void Update(float dt, Entity self, Game game);
    EntityAspect Clone();
}

public class SpawnAspect : EntityAspect {
    public float MaxPollution { get; set; }
    public float MaxWater { get; set; }
    public float Period { get; set; }
    public float Distance { get; set; }
    public void Update(float dt, Entity self, Game game) {
        if (Util.ShouldTrigger(self.Age, dt, Period)) {
            var p = self.Pos + Random.onUnitSphere;
            var cell = game.Pollution.CellWithin(new Cell(p));
            if (game.EntityDensity[cell] < 1 &&
                game.Pollution[cell] < MaxPollution &&
                game.Terrain.Water[cell] < MaxWater
               ) {
                game.SpawnedEntities.Add(new Entity {
                    Type = self.Type,
                    Pos = new Vector3(p.x, game.Terrain.Height[p], p.z)
                });
            }
        }
    }
    public EntityAspect Clone() => new SpawnAspect {
        MaxPollution = MaxPollution,
        MaxWater = MaxWater,
        Distance = Distance,
        Period = Period
    };
}

public class BeautyEntityAspect : EntityAspect {
    public float Beauty { get; set; }
    public void Update(float dt, Entity self, Game game) {
        game.Beauty[self.Pos] += dt * Beauty;
    }
    public EntityAspect Clone() => new BeautyEntityAspect { Beauty = Beauty };
}

public abstract class MoveToTargetAspect {
    public float Speed = 0.5f;
    public abstract Vector3? TargetPos { get; }
    public abstract void FindNewTarget(Game game);
    public abstract void ReachTarget(Game game);
    public void Update(float dt, Entity self, Game game) {
        if (TargetPos == null) {
            FindNewTarget(game);
        }
        else {
            self.Dir = (TargetPos.Value - self.Pos);
            var p = self.Pos + dt * Speed * self.Dir.normalized;
            var h = game.Terrain.Height[p] + game.Terrain.Water[p];
            self.Pos = new Vector3(p.x, h, p.z);
            if (self.Dir.magnitude < 0.5f) {
                ReachTarget(game);
            }
        }
    }
}

public class TreeCollectorAspect : MoveToTargetAspect, EntityAspect {
    public float Range = 5f;
    public Vector3 Home { get; set; }
    public Entity Target { get; set; }
    public override Vector3? TargetPos => Target?.Pos;
    public override void FindNewTarget(Game game) {
        foreach (var item in game.Entities) {
            if (item.Type.Name != "Tree") continue;
            if ((item.Pos - Home).magnitude < Range) {
                Target = item;
                break;
            }
        }
    }
    public override void ReachTarget(Game game) {
        game.Store += Target.Type.Contents;
        Target.IsDead = true;
    }
    public EntityAspect Clone() => new TreeCollectorAspect();
}

public class RandomMoveAspect : MoveToTargetAspect, EntityAspect {
    Vector3? targetPos = null;
    public override Vector3? TargetPos => targetPos;
    public override void FindNewTarget(Game game) {
        targetPos = game.Terrain.RandomPos();
    }
    public override void ReachTarget(Game game) {
        targetPos = null;
    }
    public EntityAspect Clone() => new RandomMoveAspect();
}
