using UnityEngine;

public class Entity : Killable {
    public Vector3 Pos { get; set; }
    public bool IsDead { get; set; }
    public GameObject GameObject { get; set; }
    public EntityType Type { get; set; }
    public float Age { get; set; }

    public void Update(float dt, Game game) {
        Age += dt;
        foreach (var aspect in Type.Aspects) {
            aspect.Update(dt, this, game);
        }
    }
}

public interface Killable {
    bool IsDead { get; }
    GameObject GameObject { get; }
}

public class EntityType {
    public string Name { get; set; }
    public Attr Contents;
    public float MaxAge;
    public EntityAspect[] Aspects = new EntityAspect[0];
}

public interface EntityAspect {
    void Update(float dt, Entity self, Game game);
}

public class BeautifulCreature : EntityAspect {
    public float Beauty { get; set; }
    public void Update(float dt, Entity self, Game game) {
        var cell = game.Beauty.CellWithin(new Cell(self.Pos));
        game.Beauty[cell] += dt * Beauty;
    }
}

public abstract class MoveToTargetAspect : EntityAspect {
    public float Speed = 0.5f;
    public abstract Vector3? TargetPos { get; }
    public abstract void FindNewTarget(Game game);
    public abstract void ReachTarget(Game game);
    public void Update(float dt, Entity self, Game game) {
        if (TargetPos == null) {
            FindNewTarget(game);
        }
        else {
            var dir = (TargetPos.Value - self.Pos);
            var p = self.Pos + dt * Speed * dir.normalized;
            var h = game.Terrain.Height[p] + game.Terrain.Water[p];
            self.Pos = new Vector3(p.x, h, p.z);
            if (dir.magnitude < 0.5f) {
                ReachTarget(game);
            }
        }
    }
}

public class TreeCollectorAspect : MoveToTargetAspect {
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
}

public class RandomMoveAspect : MoveToTargetAspect {
    Vector3? targetPos = null;
    public override Vector3? TargetPos => targetPos;
    public override void FindNewTarget(Game game) {
        targetPos = game.Terrain.RandomPos();
    }
    public override void ReachTarget(Game game) {
        targetPos = null;
    }
}
