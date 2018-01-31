using UnityEngine;

public class Lathe {
    public Mesh mesh = new Mesh();

    public Lathe(int res, Vector3[] path) {
        var slices = res + 1;
        var vertices = new Vector3[path.Length * slices];
        for (int j = 0; j < slices; j++) {
            var matrix = Rotate(-360 * j / res);
            for (int i = 0; i < path.Length; i++) {
                vertices[i + path.Length * j] = matrix * path[i];
            }
        }

        var triangles = new int[6 * (path.Length - 1) * (slices - 1)];
        int index = 0;
        for (int i = 0; i < path.Length - 1; i++) {
            for (int j = 0; j < slices - 1; j++) {
                triangles[index + 0] = (i + 1) + path.Length * (j + 0);
                triangles[index + 1] = (i + 0) + path.Length * (j + 0);
                triangles[index + 2] = (i + 0) + path.Length * (j + 1);
                triangles[index + 3] = (i + 1) + path.Length * (j + 1);
                triangles[index + 4] = (i + 1) + path.Length * (j + 0);
                triangles[index + 5] = (i + 0) + path.Length * (j + 1);
                index += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    public static Matrix4x4 Rotate(float angle) {
        return Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, angle, 0)));
    }
}

public class Models {
    public static Lathe ring = new Lathe(12, new Vector3[] {
        Vec(10, 0), Vec(12, 4), Vec(14, 0), Vec(12, -4), Vec(10, 0)
    });
    public static Lathe pillar = new Lathe(6, new Vector3[] { Vec(3, -14), Vec(4, -30) });
    public static Mesh turbine = Compose(
        new CombineInstance {
            mesh = ring.mesh,
            transform = Scale(0.4) * Offset(0, 3) * Rot(0, -30) * Rot(90, 0)
        },
        new CombineInstance {
            mesh = pillar.mesh,
            transform = Scale(0.4) * Offset(0, 3)
        }
    );

    public static Lathe dome = new Lathe(8, new Vector3[] {
        Vec(0, 5), Vec(4, 4), Vec(5, 2), Vec(5, 0)
    });
    public static Mesh greenhouse = Compose(new CombineInstance {
        mesh = dome.mesh, transform = Matrix4x4.identity
    });

    public static Lathe dish = new Lathe(8, new Vector3[] {
        Vec(1, 9), Vec(7, 7), Vec(10, 3), Vec(7, 7), Vec(1, 9), Vec(0, -5)
    });
    public static Mesh solar = Compose(new CombineInstance {
        mesh = dish.mesh, transform = Offset(0, 0.6f) * Rot(135, -30) * Scale(0.5)
    });

    public static Lathe atmoplant = new Lathe(8, new Vector3[] {
        Vec(0, 13), Vec(1, 13), Vec(2, 15), Vec(3, 15), Vec(3, 7), Vec(5, 0)
    });

    public static Lathe syntactor = new Lathe(8, new Vector3[] {
        Vec(0, 15), Vec(3, 15), Vec(4, 14), Vec(4, 5), Vec(3, 4), Vec(3, 0)
    });

    static Mesh Compose(params CombineInstance[] meshes) {
        var mesh = new Mesh();
        mesh.CombineMeshes(meshes);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }

    static Vector3 Vec(int x, int y) => 0.1f * new Vector3(x, y, 0);

    static Matrix4x4 Scale(double s) {
        return Matrix4x4.Scale(new Vector3((float)s, (float)s, (float)s));
    }

    static Matrix4x4 Rot(int x, int y, int z = 0) {
        return Matrix4x4.Rotate(Quaternion.Euler(new Vector3(x, y, z)));
    }

    static Matrix4x4 Offset(double x, double y, double z = 0) {
        return Matrix4x4.Translate(new Vector3((float)x, (float)y, (float)z));
    }
}
