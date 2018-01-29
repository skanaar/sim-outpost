using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHudCtrl : MonoBehaviour {

    Text text;

    void Start() {
        text = gameObject.GetComponent<Text>();
    }

    void Update() {
        var str = string.Empty;
        var building = Manager.Instance.SelectedBuilding;
        if (building != null) {
            str = string.Join("\n", new string[]{
                (building.BuildProgress < 1 ? "Constructing " : "") + building.type.name,
                "------",
                (building.IsEnabled ? "Enabled" : "Disabled"),
                (building.IsSupplied ? "Running" : "Not Supplied"),
                "-- turnover --",
                (building.BuildProgress < 1 ? building.type.cost : building.type.turnover).HudString
            });
        }
        else if (Manager.Instance.Terrain.Height.ContainsCell(Manager.Instance.SelectedCell)) {
            str = "height " + (int)(10 * Manager.Instance.Terrain.Height.GetCell(Manager.Instance.SelectedCell));
        }
        text.text = str;
    }
}
