using System.Linq;
using UnityEngine;

public class Item {
    public Vector3 Pos;
    public float Age;
    public ItemType Type;
    public GameObject GameObject;
    public bool IsDead;
}

public enum ItemKind {
    Plant,
    Animal,
    Mineral
}

public class ItemType {
    public ItemKind Kind;
    public Vector2 Pos;
    public string Name;
    public Attr Contents;
    public float MaxAge;
}

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
    public static Vector3 Vec(int x, int y) => new Vector3(x, y, 0);
    public static Matrix4x4 Scale(double s) => Matrix4x4.Scale(new Vector3((float)s, (float)s, (float)s));
    public static Lathe ring = new Lathe(12, new Vector3[] {
        Vec(10, 0), Vec(12, 4), Vec(14, 0), Vec(12, -4), Vec(10, 0)
    });
    public static Lathe pillar = new Lathe(6, new Vector3[] { Vec(3, -14), Vec(4, -30) });
    public static Mesh turbine = Compose(
        new CombineInstance { mesh = ring.mesh, transform = Scale(0.04) * Offset(0, 30) * Rot(0, -30) * Rot(90, 0) },
        new CombineInstance { mesh = pillar.mesh, transform = Scale(0.04) * Offset(0, 30) }
    );
    public static Lathe dome = new Lathe(8, new Vector3[] { Vec(0, 9), Vec(7, 7), Vec(10, 3), Vec(10, 0) });
    public static Mesh greenhouse = Compose(new CombineInstance { mesh = dome.mesh, transform = Scale(0.06) }
   );
    static Mesh Compose(params CombineInstance[] meshes){
        var mesh = new Mesh();
        mesh.CombineMeshes(meshes);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
    public static Matrix4x4 Rot(double x, double y, double z = 0) {
        return Matrix4x4.Rotate(Quaternion.Euler(new Vector3((float)x, (float)y, (float)z)));
    }
    public static Matrix4x4 Offset(double x, double y, double z = 0) {
        return Matrix4x4.Translate(new Vector3((float)x, (float)y, (float)z));
    }
}
