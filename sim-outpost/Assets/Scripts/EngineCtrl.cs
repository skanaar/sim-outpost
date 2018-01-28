using System.Collections.Generic;
using UnityEngine;

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
        Selection = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Selection.transform.localScale = new Vector3(1, 0.25f, 1);
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
        Selection.transform.position = Manager.Instance.Terrain.GetCellFloor(Manager.Instance.SelectedCell);
        foreach (var item in Manager.Instance.Buildings) {
            var obj = item.gameObject;
            var height = item.type.height;
            obj.transform.position = Manager.Instance.Terrain.GetCellFloor(item.i, item.j);
            obj.transform.position += new Vector3(item.type.w / 2, height / 2, item.type.h / 2);
            obj.transform.localScale = new Vector3(1, height, 1);
            var material = item.gameObject.GetComponent<Renderer>().material;
            material.color = item.enabled ? colors[item.type.model] : new Color(0.3f, 0.3f, 0.3f);
        }
        InputFilter.Update();
        if (InputFilter.IsStartOfTap) {
            Manager.Instance.SelectedCell = Manager.Instance.GridAtPoint(Manager.Instance.HoverPoint);
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
}