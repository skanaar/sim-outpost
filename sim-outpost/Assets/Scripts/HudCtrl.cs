using UnityEngine;
using UnityEngine.UI;

public class HudCtrl : MonoBehaviour {

    Text text;

	void Start () {
        text = gameObject.GetComponent<Text>(); 
	}
	
	void Update () {
        text.text = Manager.Instance.Commodities.HudString;
	}
}
