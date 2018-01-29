using System.Collections.Generic;
using UnityEngine;
using static Util;

public class EngineCtrl : MonoBehaviour {

    static Dictionary<string, PrimitiveType> models = new Dictionary<string, PrimitiveType> {
        { "block", PrimitiveType.Cube },
        { "cylinder", PrimitiveType.Cylinder },
        { "capsule", PrimitiveType.Capsule }
    };

    static Dictionary<string, Color> colors = new Dictionary<string, Color> {
        { "block", new Color(0.5f, 1f, 0.5f) },
        { "cylinder", new Color(0.5f, 0.5f, 1f) },
        { "capsule", new Color(1f, 0.5f, 0.5f)}
    };

    public GameObject Cursor;
    public GameObject Selection;

    void Start() {
        Cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Cursor.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        Selection = GameObject.CreatePrimitive(PrimitiveType.Quad);

        var terrain = GameObject.CreatePrimitive(PrimitiveType.Cube);
        terrain.AddComponent<TerrainCtrl>();
        foreach (var item in Manager.Instance.Buildings) {
            OnAddBuilding(item);
        }
        Manager.Instance.OnAddBuilding = OnAddBuilding;
    }

    void OnAddBuilding(Building building) {
        building.gameObject = GameObject.CreatePrimitive(models[building.type.model]);
    }

    void Update() {
        Manager.Instance.Update(Time.deltaTime);
        Cursor.transform.position = Manager.Instance.HoverPoint;

        foreach (var item in Manager.Instance.Buildings) {
            var obj = item.gameObject;
            var height = item.type.height;
            if (obj == null) print("GAMEOBJECT IS NULL");
            obj.transform.position = Manager.Instance.Terrain.GetCellFloor(item.i, item.j);
            obj.transform.position += new Vector3(item.type.w / 2, height / 2, item.type.h / 2);
            obj.transform.localScale = new Vector3(1, height * item.buildProgress, 1);
            var material = item.gameObject.GetComponent<Renderer>().material;
            material.color = item.enabled ? colors[item.type.model] : rgb(0x555);
            if (item.buildProgress < 1) { material.color = rgb(0xDDD); }
        }

        InputFilter.Update();
        if (InputFilter.IsStartOfTap && Input.mousePosition.x > 100) {
            Manager.Instance.SelectedCell = Manager.Instance.GridAtPoint(Manager.Instance.HoverPoint);
            var selected = Manager.Instance.SelectedCell;
            
            var isBuildable = Manager.Instance.Terrain.Slope(selected.i, selected.j) < 0.25f;
            Selection.transform.position = new Vector3(selected.i, 0, selected.j);
            Selection.GetComponent<MeshFilter>().mesh.vertices = new Vector3[]{
                new Vector3(0, Manager.Instance.Terrain.height[selected.i, selected.j] + 0.1f, 0),
                new Vector3(1, Manager.Instance.Terrain.height[selected.i+1, selected.j] + 0.1f, 0),
                new Vector3(0, Manager.Instance.Terrain.height[selected.i, selected.j+1] + 0.1f, 1),
                new Vector3(1, Manager.Instance.Terrain.height[selected.i+1, selected.j+1] + 0.1f, 1)
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