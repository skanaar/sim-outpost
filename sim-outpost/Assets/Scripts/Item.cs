using UnityEngine;

public class Item : Killable {
    public Vector3 Pos;
    public float Age;
    public ItemType Type;
    public GameObject GameObject { get; set; }
    public bool IsDead { get; set; }
}

public enum ItemKind {
    Plant,
    Animal,
    Mineral
}

public class ItemType {
    public ItemKind Kind;
    public Vector2 Pos;
    public string Name;
    public Attr Contents;
    public float MaxAge;
}

public interface Killable {
    bool IsDead { get; }
    GameObject GameObject { get; }
}
