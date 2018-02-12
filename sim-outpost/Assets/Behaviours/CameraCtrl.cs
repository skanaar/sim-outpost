using UnityEngine;

public class CameraCtrl : MonoBehaviour {
    Game Game => Game.Instance;

    Camera self;

	void Start() {
        var terrain = Game.Instance.Terrain;
        transform.position = terrain.GetCellFloor(terrain.Res / 2, terrain.Res / 2);
        transform.Rotate(new Vector3(25, 20, 0));
        self = GetComponent<Camera>();
        self.farClipPlane = 200;
        self.nearClipPlane = -200;
	}

    void Update() {
        RaycastHit hit;
        Ray ray = self.ScreenPointToRay(Input.mousePosition);
        int layerMask = (1 << TerrainCtrl.TerrainLayer);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            Game.Instance.HoverPoint = hit.point;
        }
        self.orthographicSize = 5 * Game.Zoom;
        transform.rotation = Quaternion.Euler(25/Game.Zoom, 20*Game.Zoom, 0);
        transform.position = Game.Pan;
    }

    void OnGUI() {
        if (GUI.Button(new Rect(Screen.width/2 - 110, 10, 100, 20), "Zoom in")) {
            Game.Zoom *= 0.8f;
        }
        if (GUI.Button(new Rect(Screen.width/2 + 10, 10, 100, 20), "Zoom out")) {
            Game.Zoom /= 0.8f;
        }

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
            if (Game.NeighbourDist[Game.SelectedCell] > Game.BuildRange) {
                return;
            }
        }
        else {
            if (GUI.Button(new Rect(10, 10, 100, 20), "Toggle")) {
                Game.SelectedBuilding.IsEnabled = !Game.SelectedBuilding.IsEnabled;
            }
        }
        var i = 0;
        foreach (var type in Definitions.types) {
            if (type.Predicate.CanBuild(Game.SelectedCell, Game)) {
                i++;
                var didClick = GUI.Button(new Rect(10, 110+30*i, 100, 20), type.name);
                if (didClick) {
                    InputFilter.AbortTap();
                    Game.AddBuilding(type, Game.SelectedCell);
                }
            }
        }
    }
}
