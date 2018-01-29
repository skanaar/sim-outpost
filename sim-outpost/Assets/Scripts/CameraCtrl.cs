using UnityEngine;

public class CameraCtrl : MonoBehaviour {

	void Start() {
        var terrain = Manager.Instance.Terrain;
        transform.position = terrain.GetCellFloor(terrain.Res / 2, terrain.Res / 2);
        transform.Rotate(new Vector3(25, 60, 0));
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
        if (Manager.Instance.SelectedBuilding != null) {
            if (GUI.Button(new Rect(10, 10, 100, 30), "Toggle")) {
                Manager.Instance.SelectedBuilding.IsEnabled = !Manager.Instance.SelectedBuilding.IsEnabled;
            }
        }
        for (int i = 0; i < Definitions.types.Count; i++) {
            var type = Definitions.types[i];
            var didClick = GUI.Button(new Rect(10, 50 + 40 * i, 100, 30), type.name);
            if (didClick) {
                InputFilter.AbortTap();
                Manager.Instance.StartBuild(type);
            }
        }
    }
}
