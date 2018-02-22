using UnityEngine;
using static Util;

public class SelectionCtrl : MonoBehaviour {
    Game Game => Game.Instance;
    Cell cursor = new Cell(0, 0);
    int cursorSize = 1;
       
	void Update () {
        var cell = Game.Instance.SelectedCell;
        var size = Game.CursorSize;
        if (cell != cursor || size != cursorSize) {
            var m = TerrainCtrl.BuildTerrainMesh(Game.Terrain.Height, cell, size+1);
            gameObject.GetComponent<MeshFilter>().mesh = m;
            gameObject.GetComponent<Renderer>().material.color = rgb(0xFF0);
            cursor = cell;
            cursorSize = size;
        }
	}
}
