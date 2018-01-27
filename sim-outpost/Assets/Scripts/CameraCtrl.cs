using UnityEngine;

public class CameraCtrl : MonoBehaviour {

	void Start () {
        var terrain = Manager.Instance.Terrain;
        transform.position = terrain.GetCellFloor(terrain.GridRes / 2, terrain.GridRes / 2);
        transform.Rotate(new Vector3(25, 60, 0));
	}
}
