using UnityEngine;

public class CameraCtrl : MonoBehaviour {

	void Start() {
        var terrain = Manager.Instance.Terrain;
        transform.position = terrain.GetCellFloor(terrain.GridRes / 2, terrain.GridRes / 2);
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
        for (int i = 0; i < Definitions.types.Count; i++) {
            var type = Definitions.types[i];
            var didClick = GUI.Button(new Rect(10, 10 + 40 * i, 100, 30), type.name);
            if (didClick) {
                Manager.Instance.StartBuild(type);
            }
        }
    }
}
