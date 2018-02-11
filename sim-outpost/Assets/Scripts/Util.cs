using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FloatExtensions {
    public static bool NonZero(this float self) => Math.Abs(self) > 0.01f;
}

public static class Util {
    public static int min(int a, int b) => Math.Min(a, b);
    public static int max(int a, int b) => Math.Max(a, b);
    public static float min(float a, float b) => Math.Min(a, b);
    public static float max(float a, float b) => Math.Max(a, b);
    public static float sin(float x) => (float)Math.Sin(x);
    public static float cos(float x) => (float)Math.Cos(x);
    public static float sq(float x) => x * x;
    public static int sq(int x) => x * x;
    public static float sqrt(float x) => (float)Math.Sqrt(x);
    public static float clamp(float low, float high, float x) => min(high, max(low, x));
    public static int clamp(int low, int high, int x) => min(high, max(low, x));
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
    public static Color rgba(int hex) {
        return new Color(
            ((hex & 0xf000) >> 12) / 16.0f,
            ((hex & 0x0f00) >> 8) / 16.0f,
            ((hex & 0x00f0) >> 4) / 16.0f,
            ((hex & 0x000f) >> 0) / 16.0f
        );
    }
    public static List<T> Seq<T>(params T[] items) => items.ToList();
}