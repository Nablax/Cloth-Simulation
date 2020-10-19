using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ClothGenerator : MonoBehaviour {

	public int xSize = 10, ySize = 10;
	public float stringLen = 5;
	private Mesh mesh;
	private Vector3[] vertices;

	private void Awake () {
		GenerateMesh();
		GenerateCollider();
	}
	private void GenerateCollider(){
		gameObject.AddComponent<MeshCollider>();
	}

	private void GenerateMesh () {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Cloth";

		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		for (int i = 0, y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				vertices[i] = new Vector3(x, 0, y) * stringLen;
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;

		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}
}