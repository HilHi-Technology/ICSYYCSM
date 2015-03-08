using UnityEngine;
using System.Collections;

public class MeshDraw : MonoBehaviour {
    //public Vector3[] vertices;
    //public Vector2[] uv;
	// Use this for initialization
    public Vector3[] vertices = new Vector3[4];
    public Color[] colors = new Color[4];
    private int i = 1;
	void Start () {
        //Debug.Log("Test");
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        //vertices = new Vector3[3] { new Vector3(0, 0, 0), new Vector3(2, 2, 0), new Vector3(2, 0, 0) };
        mesh.vertices = vertices;
        mesh.colors = colors;
        //mesh.vertices = new Vector3[] { new Vector3(0, 2, 0), //Top Left
        //                                new Vector3(2, 2, 0), //Top Right
        //                                new Vector3(2, 0, 0), //Bot Right
        //                                                    };//Bot Left
        //mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)};
        //if (vertices[0] != null && vertices[1] != null && vertices[2] != null) {
            mesh.triangles = new int[] { 1, 3, 0, 2, 0 , 3};
        //}
        //mesh.colors = new Color[] { new Color(1, 1, 1, 1), new Color(1, 1, 1, 1), new Color(0, 0, 0, 0) };
        //mesh.colors = new Color[] { new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(1, 1, 1, 0.5f), new Color(1, 1, 1, 0.5f) };
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Test" + i);
        i++;

	}
}   
