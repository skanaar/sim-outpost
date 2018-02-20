using UnityEngine;
using System.Linq;

public class Entity : Killable {
    public Vector3 Pos { get; set; }
    public Vector3 Dir { get; set; } = new Vector3(1, 0, 0);
    public bool IsDead { get; set; }
    public GameObject GameObject { get; set; }
    public EntityType Type { get; set; }
    public float Age { get; set; }
    public EntityAspect[] aspects;
    public EntityAspect[] Aspects {
        get {
            return aspects = (aspects ?? Type.Aspects.Select(e => e.Clone()).ToArray());
        }
    }

    public void Update(float dt, Game game) {
        foreach (var aspect in Aspects) {
            aspect.Update(dt, this, game);
        }
        Age += dt;
        if (Type.MaxAge > 0 && Age > Type.MaxAge) {
            IsDead = true;
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
