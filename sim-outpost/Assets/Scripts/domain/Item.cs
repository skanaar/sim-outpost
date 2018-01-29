using UnityEngine;

public class Item {
    public Vector3 Pos;
    public float Age;
    public ItemType Type;
    public GameObject GameObject;
    public bool IsDead;
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
