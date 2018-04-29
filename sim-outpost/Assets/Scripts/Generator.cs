using UnityEngine;
using static Util;

public class Generator {

    static Field FractalField(int res, float maxHeight, float[] curve, Noise noise) {
        var height = new float[res*res];
        for (int x = 0; x < res; x++) {
            for (int y = 0; y < res; y++) {
                var i = x + res*y;
                float slope = 20 * (1 - Mathf.Cos(sqrt(sq(x-res/2) + sq(y-res/2))/res));
                height[i] = Posterize(maxHeight * lerp(curve, noise[x, y]) - slope);
            }
        }
        return new Field(height);
    }

    static float Posterize(float v) {
        var steps = 3f;
        return lerp(v, Mathf.Round(v * steps) / steps, 0.8f);
    }

    public static Field GenerateTerrain(int res) {
        return FractalField(
            res,
            10,
            new float[] { 0f, 0.25f, 0.3f, 0.8f, 1 },
            new Noise { scale = 12f, octaves = 4 }
        );
    }
}
