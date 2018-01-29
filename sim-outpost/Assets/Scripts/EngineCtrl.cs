using System.Collections.Generic;
using UnityEngine;
using static Util;

public class EngineCtrl : MonoBehaviour {

    Manager Game => Manager.Instance;

    public GameObject Cursor;
    public GameObject Selection;

    void Start() {
        Cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Cursor.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        Selection = GameObject.CreatePrimitive(PrimitiveType.Quad);

        var terrain = GameObject.CreatePrimitive(PrimitiveType.Cube);
        terrain.AddComponent<TerrainCtrl>();
    }

    void AttachGameObject(Building building) {
        building.GameObject = GameObject.CreatePrimitive(Definitions.models[building.type.name]);
    }

    void Update() {
        Manager.Instance.Update(Time.deltaTime);
        Cursor.transform.position = Manager.Instance.HoverPoint;

        foreach (var item in Manager.Instance.Buildings) {
            if (item.GameObject == null) AttachGameObject(item);
            var obj = item.GameObject;
            var height = item.type.height;
            obj.transform.position = Game.Terrain.GetCellFloor(item.Cell);
            obj.transform.position += new Vector3(item.type.w / 2, height / 2, item.type.h / 2);
            obj.transform.localScale = new Vector3(1, height * item.BuildProgress, 1);
            var material = item.GameObject.GetComponent<Renderer>().material;
            material.color = item.IsEnabled ? Definitions.colors[item.type.name] : rgb(0x555);
            if (item.BuildProgress < 1) { material.color = rgb(0xDDD); }
        }

        InputFilter.Update();
        if (InputFilter.IsStartOfTap && Input.mousePosition.x > 100) {
            Game.SelectCell();
            var selected = Manager.Instance.SelectedCell;
            
            var isBuildable = Game.Terrain.Slope(selected.i, selected.j) < 0.25f;
            Selection.transform.position = new Vector3(selected.i, 0, selected.j);
            Selection.GetComponent<MeshFilter>().mesh.vertices = new Vector3[]{
                new Vector3(0, Game.Terrain.height[selected.i, selected.j] + 0.02f, 0),
                new Vector3(1, Game.Terrain.height[selected.i+1, selected.j] + 0.02f, 0),
                new Vector3(0, Game.Terrain.height[selected.i, selected.j+1] + 0.02f, 1),
                new Vector3(1, Game.Terrain.height[selected.i+1, selected.j+1] + 0.02f, 1)
            };
            Selection.GetComponent<MeshFilter>().mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
            Selection.GetComponent<Renderer>().material.color = isBuildable ? rgb(0xFF0) : rgb(0xF00);
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
