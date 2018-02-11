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
        Selection.AddComponent<SelectionCtrl>();

        Terrain = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Terrain.name = "terrain";
        Game.Instance.TerrainController = Terrain.AddComponent<TerrainCtrl>();

        var water = GameObject.CreatePrimitive(PrimitiveType.Cube);
        water.name = "water";
        water.AddComponent<WaterCtrl>();
    }

    void AttachGameObject(Building building) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = building.type.name;
        obj.transform.parent = Terrain.transform;
        building.GameObject = obj;
        Mesh mesh = Definitions.Model(building.type.name);
        building.GameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    void AttachGameObject(Item e) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = "tree";
        obj.transform.parent = Terrain.transform;
        e.GameObject = obj;
        obj.transform.position = e.Pos;
        var ctrl = obj.AddComponent<TreeCtrl>();
        ctrl.Tree = new FractalTree();
    }

    void AttachGameObject(Mobile e) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "mobile";
        obj.transform.parent = Terrain.transform;
        e.GameObject = obj;
        obj.transform.position = e.Pos;
        obj.transform.localScale = 0.25f * new Vector3(1, 1, 1);
    }

    Color BuildingColor(Building e) {
        if (!e.IsEnabled || !e.IsSupplied) return rgb(0x555);
        if (e.BuildProgress < 1) return rgb(0xDDD);
        return Definitions.models[e.type.name].Color;
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
            obj.transform.position = Game.Terrain.GetCellFloor(e.Cell);
            obj.transform.position += new Vector3(e.type.w / 2, 0, e.type.h / 2);
            obj.transform.localScale = new Vector3(1, (0.2f + 0.8f*e.BuildProgress), 1);
            e.GameObject.GetComponent<Renderer>().material.color = BuildingColor(e);
        }
        PruneDead(Game.Buildings);
        Game.Buildings.RemoveAll(e => e.IsDead);

        foreach (var e in Game.Mobiles) {
            if (e.GameObject == null) AttachGameObject(e);
            e.GameObject.transform.position = e.Pos;
        }
        PruneDead(Game.Mobiles);
        Game.Mobiles.RemoveAll(e => e.IsDead);

        foreach (var item in Game.Items.Where(e => !e.IsDead)) {
            if (item.GameObject == null) AttachGameObject(item);
            var size = item.Age / item.Type.MaxAge;
            var obj = item.GameObject;
            obj.transform.localScale = size * new Vector3(1, 1, 1);
            var material = item.GameObject.GetComponent<Renderer>().material;
            material.color = rgb(0x0A0);
        }
        PruneDead(Game.Items);
        Game.Items.RemoveAll(e => e.IsDead);

        InputFilter.Update();
        if (InputFilter.IsStartOfTap && Input.mousePosition.x > 110) {
            Game.SelectCell();
        }
    }
}

class InputFilter {
    static bool tapp = false;
    static bool hold = false;

    public static bool IsStartOfTap => tapp && !hold;

    public static void Update() {
        hold = tapp;
        tapp = Input.GetButtonDown("Fire1");
    }

    public static void AbortTap() {
        tapp = false;
        hold = false;
    }
}
