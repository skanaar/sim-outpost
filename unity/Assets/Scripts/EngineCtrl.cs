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

    void Start() {
        var terrain = GameObject.CreatePrimitive(PrimitiveType.Cube);
        terrain.AddComponent<TerrainCtrl>();
        foreach (var item in Manager.Instance.Buildings) {
            item.gameObject = GameObject.CreatePrimitive(models[item.type.model]);
        }
    }

    void Update() {
        Manager.Instance.Update(Time.deltaTime);
        foreach (var item in Manager.Instance.Buildings) {
            var obj = item.gameObject;
            var height = item.type.height;
            obj.transform.position = Manager.Instance.Terrain.GetCellFloor(item.i, item.j);
            obj.transform.position += new Vector3(item.type.w / 2, height / 2, item.type.h / 2);
            obj.transform.localScale = new Vector3(1, height, 1);
            var material = item.gameObject.GetComponent<Renderer>().material;
            material.color = item.enabled ? colors[item.type.model] : new Color(0.3f, 0.3f, 0.3f);
        }
    }
}
