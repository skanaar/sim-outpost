using UnityEngine;

public class Materials {
    public static Material Water => Resources.Load<Material>("water-material");
    public static Material Ground => Resources.Load<Material>("ground-material");
    public static void Set(GameObject obj, Material material) {
        obj.GetComponent<Renderer>().material = material;
    }
}
