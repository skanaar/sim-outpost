using UnityEngine;
using System.Linq;

public class FractalTree {

    public class TreeShape {
        public float[] childOffsets;
        public float falloff;
        public float fanning;
        public int maxDepth = 2;
    }

    public class Segment {
        public Vector3 start;
        public Vector3 dir;
        public Segment[] children;
        public Segment(TreeShape shape, Vector3 start, Vector3 dir, int depth) {
            this.start = start;
            this.dir = dir;
            if (depth == shape.maxDepth) {
                children = new Segment[0];
            } else {
                children = shape.childOffsets.Select(e => {
                    var r = Random.onUnitSphere;
                    return new Segment(
                        shape: shape,
                        start: start + (e * dir),
                        dir: shape.falloff * (dir + shape.fanning * dir.magnitude * r),
                        depth: depth + 1
                    );
                }).ToArray();
            }
        }
    }

    public Segment trunk;

    public FractalTree() {
        trunk = new Segment(
            shape: new TreeShape {
                childOffsets = new float[] { 0.5f, 0.65f, 0.8f, 0.9f },
                fanning = 0.75f,
                falloff = 0.8f,
                maxDepth = 2
            },
            start: Vector3.zero,
            dir: new Vector3(0, 0.5f, 0),
            depth: 0
        );
    }
}
