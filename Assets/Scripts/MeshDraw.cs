using UnityEngine;
using System.Collections;

public class MeshDraw : MonoBehaviour {
    //public Vector3[] vertices;
    //public Vector2[] uv;
	// Use this for initialization
    public Vector3[] vertices;
    public Color[] colors;
    private int i = 1;
    public bool isTriangle;
	void Start () {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        //is_light = false;
        
        if (isTriangle) {
            mesh.triangles = new int[] { 0, 1, 2 };
            //Debug.Log(mesh.vertices.Length);
        } else { 
            mesh.triangles = new int[] { 1, 3, 0, 2, 0, 3 };
            //Debug.Log(mesh.vertices.Length);
        }
        
	}
}
