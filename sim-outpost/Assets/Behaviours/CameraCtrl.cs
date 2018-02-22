using System.Linq;
using UnityEngine;
using static Util;

public class CameraCtrl : MonoBehaviour {
    static int btnW = 100;
    static int btnH = 30;
    static int padding = 10;
    static string[] overlayNames = { "Terrain", "Beauty", "Pollution", "X Flow" };

    Game Game => Game.Instance;
    Camera self;
    float Height = 0;

	void Start() {
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
        float rotX = clamp(0, 90, 25*Game.Zoom);
        float rotY = clamp(0, 45, 30/Game.Zoom-10);
        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
        Height = lerp(Height, Game.Terrain.Height[Game.Pan], 0.07f);
        transform.position = new Vector3(Game.Pan.x, Height, Game.Pan.z);
    }

    void OnGUI() {
        if (GUI.Button(new Rect(Screen.width/2 - 110, 10, btnW, btnH), "Zoom in")) {
            Game.Zoom *= 0.8f;
        }
        if (GUI.Button(new Rect(Screen.width/2 + 10, 10, btnW, btnH), "Zoom out")) {
            Game.Zoom /= 0.8f;
        }
        var ot = overlayNames[Game.DataOverlay];
        if (GUI.Button(new Rect(Screen.width/2 + 120, 10, btnW, btnH), ot)) {
            Game.DataOverlay = (Game.DataOverlay+1)%4;
        }

        if (Game.SelectedBuilding == null) {
            var terraform = new Terraform { Game = Game };
            if (GUI.Button(new Rect(10, 10, btnW, btnH), "Level")) {
                terraform.LevelTerrain(Game.SelectedCell);
            }
            if (GUI.Button(new Rect(10, 20+btnH, btnW, btnH), "Raise")) {
                terraform.AdjustTerrain(Game.SelectedCell, 0.25f);
            }
            if (GUI.Button(new Rect(10, 30+btnH*2, btnW, btnH), "Lower")) {
                terraform.AdjustTerrain(Game.SelectedCell, -0.25f);
            }
        }
        else {
            if (GUI.Button(new Rect(10, 10, btnW, btnH), "Toggle")) {
                Game.SelectedBuilding.IsEnabled = !Game.SelectedBuilding.IsEnabled;
            }
        }
        if (GUI.Button(new Rect(10, 40+btnH*3, btnW/3, btnH), "1x")) {
            Game.CursorSize = 1;
        }
        if (GUI.Button(new Rect(10+btnW/3, 40+btnH*3, btnW/3, btnH), "2x")) {
            Game.CursorSize = 2;
        }
        if (GUI.Button(new Rect(10+2*btnW/3, 40+btnH*3, btnW/3, btnH), "3x")) {
            Game.CursorSize = 3;
        }
        if (Game.NeighbourDist[Game.SelectedCell] <= Game.BuildRange) {
            var i = 0;
            var dh = btnH + padding;
            foreach (var type in Definitions.types.Where(t => t.w == Game.CursorSize)) {
                if (type.Predicate.CanBuild(type, Game.SelectedCell, Game)) {
                    i++;
                    var y = 3*dh+dh*i;
                    var didClick = GUI.Button(new Rect(10, y, btnW, btnH), type.name);
                    if (didClick) {
                        InputFilter.AbortTap();
                        Game.AddBuilding(type, Game.SelectedCell);
                    }
                }
            }
        }
    }
}
