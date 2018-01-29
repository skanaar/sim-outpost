using System.Collections.Generic;
using System.Linq;
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
        Manager.Instance.TerrainController = terrain.AddComponent<TerrainCtrl>();
    }

    void AttachGameObject(Building building) {
        building.GameObject = GameObject.CreatePrimitive(Definitions.models[building.type.name]);
        if (building.type.name == "Turbine")
            building.GameObject.GetComponent<MeshFilter>().mesh = Models.turbine;
        if (building.type.name == "Greenhouse")
            building.GameObject.GetComponent<MeshFilter>().mesh = Models.greenhouse;
        if (building.type.name == "Solar")
            building.GameObject.GetComponent<MeshFilter>().mesh = Models.solar;
        if (building.type.name == "Atmoplant")
            building.GameObject.GetComponent<MeshFilter>().mesh = Models.atmoplant.mesh;
        if (building.type.name == "Syntactor")
            building.GameObject.GetComponent<MeshFilter>().mesh = Models.syntactor.mesh;
    }

    void AttachGameObject(Item building) {
        building.GameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    void Update() {
        Manager.Instance.Update(Time.deltaTime);
        Cursor.transform.position = Manager.Instance.HoverPoint;

        foreach (var e in Manager.Instance.Buildings) {
            if (e.GameObject == null) AttachGameObject(e);
            var obj = e.GameObject;
            var height = e.type.height;
            obj.transform.position = Game.Terrain.GetCellFloor(e.Cell);
            obj.transform.position += new Vector3(e.type.w / 2, 0, e.type.h / 2);
            obj.transform.localScale = new Vector3(1, height * (0.2f + 0.8f*e.BuildProgress), 1);
            var material = e.GameObject.GetComponent<Renderer>().material;
            material.color = e.IsEnabled && e.IsSupplied ? Definitions.colors[e.type.name] : rgb(0x555);
            if (e.BuildProgress < 1) { material.color = rgb(0xDDD); }
        }

        foreach (var item in Manager.Instance.Items.Where(e => !e.IsDead)) {
            if (item.GameObject == null) AttachGameObject(item);
            var size = item.Age / item.Type.MaxAge;
            var obj = item.GameObject;
            obj.transform.position = item.Pos + Vector3.up * size;
            obj.transform.localScale = size * new Vector3(0.5f, 2, 0.5f);
            var material = item.GameObject.GetComponent<Renderer>().material;
            material.color = rgb(0x0A0);
        }

        foreach (var zombie in Manager.Instance.Items.Where(e => e.IsDead)) {
            if (zombie.GameObject != null) {
                Destroy(zombie.GameObject);
            }
        }
        Manager.Instance.Items.RemoveAll(e => e.IsDead);

        InputFilter.Update();
        if (InputFilter.IsStartOfTap && Input.mousePosition.x > 100) {
            Game.SelectCell();
            var selected = Manager.Instance.SelectedCell;

            Selection.transform.position = new Vector3(selected.i, 0, selected.j);
            var mesh = Selection.GetComponent<MeshFilter>().mesh;
            mesh.vertices = new Vector3[]{
                new Vector3(0, Game.Terrain.Height[selected.i, selected.j] + 0.02f, 0),
                new Vector3(1, Game.Terrain.Height[selected.i+1, selected.j] + 0.02f, 0),
                new Vector3(0, Game.Terrain.Height[selected.i, selected.j+1] + 0.02f, 1),
                new Vector3(1, Game.Terrain.Height[selected.i+1, selected.j+1] + 0.02f, 1)
            };
            mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
            mesh.RecalculateBounds();
            Selection.GetComponent<Renderer>().material.color = Game.SelectedCellIsBuildable ? rgb(0xFF0) : rgb(0xF00);
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
