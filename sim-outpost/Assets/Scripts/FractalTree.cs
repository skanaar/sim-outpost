using UnityEngine;
using System.Linq;

public class FractalTree {

    public class Segment {
        public Vector3 start;
        public Vector3 dir;
        public Segment[] children;
        public Segment(Vector3 start, Vector3 dir, float[] childOffsets, float fanning, float falloff, int depth) {
            this.start = start;
            this.dir = dir;
            if (depth == 0) {
                children = new Segment[0];
            } else {
                children = childOffsets.Select(e => {
                    return new Segment(
                        start + (e * dir),
                        fanning * (dir + dir.magnitude * Random.onUnitSphere),
                        childOffsets,
                        fanning,
                        falloff * falloff,
                        depth - 1
                    );
                }).ToArray();
            }
        }
    }

    public Segment trunk;

    public FractalTree() {
        trunk = new Segment(
            Vector3.zero,
            new Vector3(0, 1, 0),
            new float[]{ 0.5f, 0.65f, 0.8f, 0.9f },
            0.8f,
            0.5f,
            2
        );
    }
}
