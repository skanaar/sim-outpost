using System.Collections.Generic;
using UnityEngine;

public class Mobile : Killable {
    public Vector3 Pos { get; set; }
    public bool IsDead { get; set; }
    public GameObject GameObject { get; set; }
    public List<MobileAspect> Aspects = new List<MobileAspect>();

    public void Update(float dt, Game game) {
        foreach (var aspect in Aspects) {
            aspect.Update(dt, this, game);
        }
    }
}

public interface MobileAspect {
    void Update(float dt, Mobile self, Game game);
}

public abstract class MoveToTargetAspect : MobileAspect {
    public float Speed = 0.5f;
    public abstract Vector3? TargetPos { get; }
    public abstract void FindNewTarget(Game game);
    public abstract void ReachTarget(Game game);
    public void Update(float dt, Mobile self, Game game) {
        if (TargetPos == null) {
            FindNewTarget(game);
        }
        else {
            var dir = (TargetPos.Value - self.Pos);
            var p = self.Pos + dt * Speed * dir.normalized;
            self.Pos = new Vector3(p.x, game.Terrain.Height[p], p.z);
            if (dir.magnitude < 0.5f) {
                ReachTarget(game);
            }
        }
    }
}

public class TreeCollectorAspect : MoveToTargetAspect {
    public float Range = 5f;
    public Vector3 Home { get; set; }
    public Item Target { get; set; }
    public override Vector3? TargetPos => Target?.Pos;
    public override void FindNewTarget(Game game) {
        foreach (var item in game.Items) {
            if (item.Type.Kind != ItemKind.Plant) continue;
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
