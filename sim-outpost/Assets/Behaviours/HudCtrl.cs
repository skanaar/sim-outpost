using UnityEngine;
using UnityEngine.UI;

public class HudCtrl : MonoBehaviour {

    Text text;
    Game Game => Game.Instance;

	void Start () {
        text = gameObject.GetComponent<Text>(); 
	}
	
    void Update () {
        var state = string.Join("\n", new string[]{
            "beds " + Game.Beds,
            "colonists " + Game.Population,
            "workforce " + Game.WorkforceDemand,
            ""
        });
        text.text = state + "\n" + Game.Store.HudString();
    }
}
