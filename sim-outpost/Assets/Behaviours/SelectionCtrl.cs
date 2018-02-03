using UnityEngine;
using static Util;

public class SelectionCtrl : MonoBehaviour {

    Cell lastSelection = new Cell(0, 0);
    
	void Update () {
        var cell = Manager.Instance.SelectedCell;
        if (cell == lastSelection) {
            return;
        }
        lastSelection = cell;
        gameObject.transform.position = new Vector3(cell.i, 0, cell.j);
        var mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.vertices = new Vector3[]{
                new Vector3(0, Game.Terrain.Height[cell.i+0, cell.j+0] + 0.02f, 0),
                new Vector3(1, Game.Terrain.Height[cell.i+1, cell.j+0] + 0.02f, 0),
                new Vector3(0, Game.Terrain.Height[cell.i+0, cell.j+1] + 0.02f, 1),
                new Vector3(1, Game.Terrain.Height[cell.i+1, cell.j+1] + 0.02f, 1)
            };
        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        mesh.RecalculateBounds();
        var selectionColor = Game.SelectedCellIsBuildable ? rgb(0xFF0) : rgb(0xF00);
        gameObject.GetComponent<Renderer>().material.color = selectionColor;
	}
}
