﻿using System;
using UnityEngine;

public static class Util {
    public static float min(float a, float b) => Math.Min(a, b);
    public static float max(float a, float b) => Math.Max(a, b);
    public static float sin(float x) => (float)Math.Sin(x);
    public static float cos(float x) => (float)Math.Cos(x);
    public static float sq(float x) => x * x;
    public static int sq(int x) => x * x;
    public static float lerp(float a, float b, float k) => a * (k - 1) + b * k;
    public static Color lerp(Color a, Color b, float k) => new Color(
        lerp(a.r, b.r, k),
        lerp(a.g, b.g, k),
        lerp(a.b, b.b, k),
        lerp(a.a, b.a, k)
    );
    public static float lerp(float[] values, float amount) {
        float a = max(0, min(0.999f, amount));
        float k = (a * (values.Length - 1)) % 1f;
        int i = (int)Math.Floor(a * (values.Length - 1));
        float c1 = values[i];
        float c2 = values[i + 1];
        return c1 * (1 - k) + c2 * k;
    }
    public static Color lerp(Color[] values, float amount) {
        float a = max(0, min(0.999f, amount));
        float k = (a * (values.Length - 1)) % 1f;
        int i = (int)Math.Floor(a * (values.Length - 1));
        var c1 = values[i];
        var c2 = values[i + 1];
        return new Color(
            c1.r * (1 - k) + c2.r * k,
            c1.g * (1 - k) + c2.g * k,
            c1.b * (1 - k) + c2.b * k,
            c1.a * (1 - k) + c2.a * k
        );
    }
    public static Color rgb(int hex) {
        return new Color(
            ((hex & 0xf00) >> 8) / 16.0f,
            ((hex & 0x0f0) >> 4) / 16.0f,
            ((hex & 0x00f) >> 0) / 16.0f
        );
    }
}