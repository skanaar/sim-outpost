using UnityEngine;
using UnityEngine.UI;

public class HudCtrl : MonoBehaviour {

    Text text;

	void Start () {
        text = gameObject.GetComponent<Text>(); 
	}
	
	void Update () {
        var str = Manager.Instance.Commodities.HudString;
        var building = Manager.Instance.SelectedBuilding;
        if (building != null) {
            str = str + string.Join("\n", new string[]{
                "\n------",
                building.type.name,
                (building.IsEnabled ? "Enabled" : "Disabled"),
                (building.IsSupplied ? "Running" : "Not Supplied"),
                building.type.turnover.HudString
            });
        }
        text.text = str;
	}
}
