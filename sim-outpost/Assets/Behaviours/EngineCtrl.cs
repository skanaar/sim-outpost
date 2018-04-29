using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Util;

public class EngineCtrl : MonoBehaviour {

    Game Game => Game.Instance;

    public GameObject Selection;
    public GameObject Terrain;

    void Start() {
        Selection = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Selection.name = "selection";
        Selection.transform.position = new Vector3(0, 0.02f, 0);
        Selection.AddComponent<SelectionCtrl>();

        Terrain = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Terrain.name = "terrain";
        Game.Instance.TerrainController = Terrain.AddComponent<TerrainCtrl>();

        var water = GameObject.CreatePrimitive(PrimitiveType.Cube);
        water.name = "water";
        water.AddComponent<WaterCtrl>();
        Game.Stabilize(100);
        Persister.LoadGame(Game);
    }

    void AttachGameObject(Building building) {
        var prefab = building.type.name.ToLower().Replace(' ', '-');
        var obj = Instantiate(Resources.Load<GameObject>("Prefabs/" + prefab));
        obj.name = building.type.name;
        obj.transform.parent = Terrain.transform;
        building.GameObject = obj;
    }

    void AttachGameObjectToTree(Entity e) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = "tree";
        obj.transform.parent = Terrain.transform;
        e.GameObject = obj;
        obj.transform.position = e.Pos;
        var ctrl = obj.AddComponent<TreeCtrl>();
        ctrl.Tree = new FractalTree();
    }

    void AttachGameObject(Entity e) {
        var prefab = e.Type.Name.ToLower().Replace(' ', '-');
        var obj = Instantiate(Resources.Load<GameObject>("Prefabs/" + prefab));
        obj.name = "mobile";
        obj.transform.parent = Terrain.transform;
        e.GameObject = obj;
        obj.transform.position = e.Pos;
        obj.transform.localScale = 0.25f * new Vector3(1, 1, 1);
    }

    Color BuildingColor(Building e) {
        if (!e.IsEnabled || !e.IsSupplied) return rgb(0x555);
        if (e.BuildProgress < 1) return rgb(0xDDD);
        return rgb(0xFFF);
    }

    void PruneDead(IEnumerable<Killable> killables) {
        foreach (var zombie in killables.Where(e => e.IsDead)) {
            if (zombie.GameObject != null) {
                Destroy(zombie.GameObject);
            }
        }
    }

    void Update() {
        Game.Update(Time.deltaTime);
        foreach (var e in Game.Buildings) {
            if (e.GameObject == null) AttachGameObject(e);
            var obj = e.GameObject;
            var h = Game.Terrain.AverageHeight(e.Cell.i, e.Cell.j, e.type.w, e.type.h);
            var center = e.Cell.ToVector + 0.5f * new Vector3(e.type.w, 0, e.type.h);
            obj.transform.position = center + new Vector3(0,h,0);
            obj.transform.localScale = new Vector3(1, (0.2f + 0.8f*e.BuildProgress), 1);
            e.GameObject.GetComponent<Renderer>().material.color = BuildingColor(e);
        }
        PruneDead(Game.Buildings);
        Game.Buildings.RemoveAll(e => e.IsDead);

        foreach (var e in Game.Entities) {
            if (Game.NeighbourDist[e.Pos] > Game.VisionRange) { continue; }
            if (e.GameObject == null) AttachGameObject(e);
            e.GameObject.transform.position = e.Pos;
            e.GameObject.transform.rotation = Quaternion.LookRotation(e.Dir, Vector3.up);
        }
        PruneDead(Game.Entities);
        Game.Entities.RemoveAll(e => e.IsDead);

        InputFilter.Update();
        if (InputFilter.IsStartOfTap && Input.mousePosition.x > 110) {
            Game.SelectCell();
        }
        if (InputFilter.Hold) {
            var delta = new Vector3(InputFilter.Swipe.x, 0, InputFilter.Swipe.y);
            Game.Pan += 0.003f * Game.Zoom * delta;
        }
    }
}

class InputFilter {
    static Vector3 start = Vector3.zero;
    public static bool Hold => Input.GetButton("Fire1");
    public static bool aborted = false;

    public static bool IsStartOfTap => !aborted && Input.GetButtonDown("Fire1");

    public static void Update() {
        aborted = false;
        if (IsStartOfTap) {
            start = Input.mousePosition;
        }
    }

    public static void AbortTap() {
        aborted = true;
    }

    public static Vector3 Swipe => Input.mousePosition - start;
}
