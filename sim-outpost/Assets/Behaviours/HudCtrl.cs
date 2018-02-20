using System.Linq;
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
        text.text = state + "\n" + HudString();
    }

    struct HudRow {
        public string Name;
        public float Value;
        public float Capacity;
        public override string ToString() => Name + " " + Value + "/" + Capacity;
    }

    public string HudString(bool showDecimals = false) {
        var e = Game.Store;
        var c = Game.StoreCapacity;
        var format = showDecimals ? "0.#" : "0";
        HudRow[] lines = {
            new HudRow{ Name = "water", Value = e.water, Capacity = c.water },
            new HudRow{ Name = "credits", Value = e.credits, Capacity = c.credits },
            new HudRow{ Name = "ore", Value = e.ore, Capacity = c.ore },
            new HudRow{ Name = "metal", Value = e.metal, Capacity = c.metal },
            new HudRow{ Name = "oxygen", Value = e.oxygen, Capacity = c.oxygen },
            new HudRow{ Name = "fuel", Value = e.fuel, Capacity = c.fuel },
            new HudRow{ Name = "biosludge", Value = e.biosludge, Capacity = c.biosludge },
            new HudRow{ Name = "biomass", Value = e.biomass, Capacity = c.biomass },
            new HudRow{ Name = "chems", Value = e.chems, Capacity = c.chems },
            new HudRow{ Name = "food", Value = e.food, Capacity = c.food },
            new HudRow{ Name = "energy", Value = e.energy, Capacity = c.energy },
        };
        return string.Join("\n", lines
           .Where(x => x.Value.NonZero())
           .Select(x => x.Name+" "+x.Value.ToString(format)+"/"+x.Capacity));
    }
}
