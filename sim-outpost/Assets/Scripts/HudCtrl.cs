using UnityEngine;
using UnityEngine.UI;

public class HudCtrl : MonoBehaviour {

    Text text;
    Manager Game => Manager.Instance;

	void Start () {
        text = gameObject.GetComponent<Text>(); 
	}
	
	void Update () {
        text.text = Game.Commodities.HudString;
    }
}
