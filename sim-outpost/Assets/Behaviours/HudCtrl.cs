using UnityEngine;
using UnityEngine.UI;

public class HudCtrl : MonoBehaviour {

    Text text;
    Game Game => Game.Instance;

	void Start () {
        text = gameObject.GetComponent<Text>(); 
	}
	
	void Update () {
        text.text = Game.Store.HudString();
    }
}
