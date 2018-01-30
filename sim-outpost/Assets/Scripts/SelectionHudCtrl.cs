using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Util;

public class SelectionHudCtrl : MonoBehaviour {

    Text text;

    void Start() {
        text = gameObject.GetComponent<Text>();
    }

    void Update() {
        var e = Game.SelectedBuilding;
        if (e != null) {
            text.text = BuildingDesc(e);
        }
        else if (Game.Terrain.Height.ContainsCell(Game.SelectedCell)) {
            text.text = CellDesc(Manager.Instance.SelectedCell);
        }
    }

    private static string CellDesc(Cell cell) {
        return "height " + (int)(10 * Game.Terrain.Height.GetCell(cell));
    }

    string BuildingDesc(Building e) {
        return string.Join("\n", new string[]{
            (e.BuildProgress < 1 ? "Constructing " : "") + e.type.name,
            "------",
            (e.IsEnabled ? "Enabled" : "Disabled"),
            (e.IsSupplied ? "Running" : "Not Supplied"),
            "-- turnover --",
            (e.BuildProgress < 1 ? e.type.cost : e.type.turnover).HudString
        });
    }
}
