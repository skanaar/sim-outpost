using UnityEngine;

public class CameraCtrl : MonoBehaviour {
    Manager Game => Manager.Instance;

	void Start() {
        var terrain = Manager.Instance.Terrain;
        transform.position = terrain.GetCellFloor(terrain.Res / 2, terrain.Res / 2);
        transform.Rotate(new Vector3(25, 20, 0));
        Camera cam = GetComponent<Camera>();
        cam.farClipPlane = 200;
        cam.nearClipPlane = -200;
	}

    void Update() {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        int layerMask = (1 << TerrainCtrl.TerrainLayer);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            Manager.Instance.HoverPoint = hit.point;
        }
    }

    void OnGUI() {
        if (Game.SelectedBuilding == null) {
            var terraform = new Terraform { Game = Game };
            if (GUI.Button(new Rect(10, 10, 100, 20), "Level")) {
                terraform.LevelTerrain(Game.SelectedCell);
            }
            if (GUI.Button(new Rect(10, 40, 100, 20), "Raise")) {
                terraform.AdjustTerrain(Game.SelectedCell, 0.25f);
            }
            if (GUI.Button(new Rect(10, 70, 100, 20), "Lower")) {
                terraform.AdjustTerrain(Game.SelectedCell, -0.25f);
            }
            if (Game.IsBuildable(Game.SelectedCell) && Game.SelectedBuilding == null) {
                for (int i = 0; i < Definitions.types.Count; i++) {
                    var type = Definitions.types[i];
                    var didClick = GUI.Button(new Rect(10, 110+30*i, 100, 20), type.name);
                    if (didClick) {
                        InputFilter.AbortTap();
                        Game.AddBuilding(type, Game.SelectedCell);
                    }
                }
            }
        }
        else {
            if (GUI.Button(new Rect(10, 10, 100, 20), "Toggle")) {
                Game.SelectedBuilding.IsEnabled = !Game.SelectedBuilding.IsEnabled;
            }
        }
    }
}
