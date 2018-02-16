using UnityEngine;

public class Materials {
    public static Material Water => Resources.Load<Material>("Materials/water");
    public static Material Ground => Resources.Load<Material>("Materials/ground");
    public static Material GroundBeauty => Resources.Load<Material>("Materials/ground-beauty");
    public static Material GroundPollution => Resources.Load<Material>("Materials/ground-pollution");
}
