using UnityEngine;
using UnityEngine.UI;

public class SelectionHudCtrl : MonoBehaviour {
    Game Game => Game.Instance;

    Text text;

    void Start() {
        text = gameObject.GetComponent<Text>();
    }

    void Update() {
        var e = Game.SelectedBuilding;
        if (e != null) {
            text.text = BuildingDesc(e);
        }
        else if (Game.Terrain.Height.field.ContainsCell(Game.SelectedCell)) {
            text.text = CellDesc(Game.Instance.SelectedCell);
        }
    }

    string CellDesc(Cell cell) {
        return string.Join("\n", new string[]{
            "height " + (int)(10 * Game.Terrain.Height[cell]),
            "beauty " + (int)(10 * Game.Beauty[cell]),
            "support " + (int)Game.NeighbourDist[cell],
            "peak " + new PeakProminence(Game.Terrain.Height)[cell].ToString("0.##")
        });
    }

    string BuildingDesc(Building e) {
        var inBuild = e.BuildProgress < 1;
        return string.Join("\n", new string[]{
            (inBuild ? "Constructing " : "") + e.type.name,
            "------",
            (e.IsEnabled ? "Enabled" : "Disabled"),
            (e.IsSupplied ? "Running" : "Not Supplied"),
            "",
            (inBuild ? "-- cost --" : "-- turnover --"),
            (inBuild ? e.type.cost.HudString() : e.LastTurnover.HudString(true))
        });
    }
}
