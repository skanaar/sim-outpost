﻿using System;
using System.Linq;
using static Util;

[Serializable]
public struct Attr {
    public static Attr Zero = new Attr();
    public static Attr Identity = new Attr {
        water = 1,
        credits = 1,
        ore = 1,
        metal = 1,
        oxygen = 1,
        fuel = 1,
        biosludge = 1,
        biomass = 1,
        chems = 1,
        food = 1,
        energy = 1
    };

    public float water;
    public float credits;
    public float ore;
    public float metal;
    public float oxygen;
    public float fuel;
    public float biosludge;
    public float biomass;
    public float chems;
    public float food;
    public float energy;

    public override string ToString() {
        return string.Join("\n", new string[]{
            "water " + water,
            "credits " + credits,
            "ore " + ore,
            "metal " + metal,
            "oxygen " + oxygen,
            "fuel " + fuel,
            "biosludge " + biosludge,
            "biomass " + biomass,
            "chems " + chems,
            "food " + food,
            "energy " + energy,
        });
    }

    struct KeyValue {
        public string Name;
        public float Value;
        public override string ToString() => Name + " " + Value;
    }

    public string HudString(bool showDecimals = false) {
        var format = showDecimals ? "0.#" : "0";
        KeyValue[] lines = {
            new KeyValue{ Name = "water", Value = water },
            new KeyValue{ Name = "credits", Value = credits },
            new KeyValue{ Name = "ore", Value = ore },
            new KeyValue{ Name = "metal", Value = metal },
            new KeyValue{ Name = "oxygen", Value = oxygen },
            new KeyValue{ Name = "fuel", Value = fuel },
            new KeyValue{ Name = "biosludge", Value = biosludge },
            new KeyValue{ Name = "biomass", Value = biomass },
            new KeyValue{ Name = "chems", Value = chems },
            new KeyValue{ Name = "food", Value = food },
            new KeyValue{ Name = "energy", Value = energy },
        };
        return string.Join("\n", lines
                           .Where(e => e.Value.NonZero())
                           .Select(e => e.Name + " " + e.Value.ToString(format)));
    }

    public Attr ClampToPositive() {
        return new Attr {
            water = max(0, water),
            credits = max(0, credits),
            ore = max(0, ore),
            metal = max(0, metal),
            oxygen = max(0, oxygen),
            fuel = max(0, fuel),
            biosludge = max(0, biosludge),
            biomass = max(0, biomass),
            chems = max(0, chems),
            food = max(0, food),
            energy = max(0, energy),
        };
    }

    public static bool operator <=(Attr a, Attr b) {
        return (
            a.water <= b.water &&
            a.credits <= b.credits &&
            a.ore <= b.ore &&
            a.metal <= b.metal &&
            a.oxygen <= b.oxygen &&
            a.fuel <= b.fuel &&
            a.biosludge <= b.biosludge &&
            a.biomass <= b.biomass &&
            a.chems <= b.chems &&
            a.food <= b.food &&
            a.energy <= b.energy
        );
    }

    public static bool operator >=(Attr a, Attr b) {
        return (
            a.water >= b.water &&
            a.credits >= b.credits &&
            a.ore >= b.ore &&
            a.metal >= b.metal &&
            a.oxygen >= b.oxygen &&
            a.fuel >= b.fuel &&
            a.biosludge >= b.biosludge &&
            a.biomass >= b.biomass &&
            a.chems >= b.chems &&
            a.food >= b.food &&
            a.energy >= b.energy
        );
    }

    public static Attr operator +(Attr a, Attr b) {
        return new Attr {
            water = a.water + b.water,
            credits = a.credits + b.credits,
            ore = a.ore + b.ore,
            metal = a.metal + b.metal,
            oxygen = a.oxygen + b.oxygen,
            fuel = a.fuel + b.fuel,
            biosludge = a.biosludge + b.biosludge,
            biomass = a.biomass + b.biomass,
            chems = a.chems + b.chems,
            food = a.food + b.food,
            energy = a.energy + b.energy,
        };
    }

    public static Attr operator *(float factor, Attr b) {
        return new Attr {
            water = factor * b.water,
            credits = factor * b.credits,
            ore = factor * b.ore,
            metal = factor * b.metal,
            oxygen = factor * b.oxygen,
            fuel = factor * b.fuel,
            biosludge = factor * b.biosludge,
            biomass = factor * b.biomass,
            chems = factor * b.chems,
            food = factor * b.food,
            energy = factor * b.energy,
        };
    }
}
