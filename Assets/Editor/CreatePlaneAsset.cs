using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreatePlaneAsset {
	
	[MenuItem("Assets/Create/Simple Plane")]
	public static void CreatePlane() {
		var toSave = CreateMesh(0.5f, 0.5f);
		AssetDatabase.CreateAsset(toSave, "Assets/Plane.asset");
	}
	
	public static GameObject CreateMeshGO(string name, Mesh m) {
		var toReturn = new GameObject(name);
		var mf = toReturn.AddComponent<MeshFilter>();
		mf.mesh = m;
		toReturn.AddComponent<MeshRenderer>();
		
		return toReturn;
	}
	
	public static Mesh CreateMesh(float hw, float hh) {
		var toReturn = new Mesh();
		var verts = new Vector3[4];
		var uvs = new Vector2[4];
		var normals = new Vector3[4];
		var indices = new int[6];
		
		for(int i = 0; i < 4; i++) {
			verts[i].x = i % 2 == 0 ? -hw : hw;
			verts[i].y = i / 2 == 0 ? hh : -hh;
			verts[i].z = 0;
			
			uvs[i].x = i % 2 == 0 ? 0.0f : 1.0f;
			uvs[i].y = i / 2 == 0 ? 1.0f : 0.0f;
			
			normals[i].x = 0; 
			normals[i].y = 0; 
			normals[i].z = 1;
		}
		
		indices[0] = 0;
		indices[1] = 1;
		indices[2] = 2;
		indices[3] = 2;
		indices[4] = 1;
		indices[5] = 3;
		
		toReturn.vertices = verts;
		toReturn.uv = uvs;
		toReturn.normals = normals;
		toReturn.triangles = indices;
		
		RecalculateTangents(toReturn);
		
		toReturn.Optimize();
		
		return toReturn;
	}
	
	public static void RecalculateTangents(Mesh theMesh) {
        int vertexCount = theMesh.vertexCount;
        Vector3[] vertices = theMesh.vertices;
        Vector3[] normals = theMesh.normals;
        Vector2[] texcoords = theMesh.uv;
        int[] triangles = theMesh.triangles;
        int triangleCount = triangles.Length/3;
        var tangents = new Vector4[vertexCount];
        var tan1 = new Vector3[vertexCount];
        var tan2 = new Vector3[vertexCount];
        int tri = 0;

        for (int i = 0; i < triangleCount; i++) {
            var i1 = triangles[tri];
            var i2 = triangles[tri+1];
            var i3 = triangles[tri+2];

            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];

            var w1 = texcoords[i1];
            var w2 = texcoords[i2];
            var w3 = texcoords[i3];

            var x1 = v2.x - v1.x;
            var x2 = v3.x - v1.x;
            var y1 = v2.y - v1.y;
            var y2 = v3.y - v1.y;
            var z1 = v2.z - v1.z;
            var z2 = v3.z - v1.z;

            var s1 = w2.x - w1.x;
            var s2 = w3.x - w1.x;

            var t1 = w2.y - w1.y;
            var t2 = w3.y - w1.y;

            var r = 1.0f / (s1 * t2 - s2 * t1);

            var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;

            tri += 3;
        }

        for (int i = 0; i < vertexCount; i++) {
            var n = normals[i];
            var t = tan1[i];

            // Gram-Schmidt orthogonalize
            Vector3.OrthoNormalize(ref n, ref t);

            tangents[i].x  = t.x;
            tangents[i].y  = t.y;
            tangents[i].z  = t.z;

            // Calculate handedness
            tangents[i].w = ( Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f ) ? -1.0f : 1.0f;
        }       
        theMesh.tangents = tangents;
    }
}
