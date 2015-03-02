using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawScript : MonoBehaviour {
    /*Script placed on the main camera in order to utilize openGL to draw things. Add points to the drawlist to draw quads*/
    public static List<Vector2> drawList = new List<Vector2>();
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    public Material mat;
    void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        GL.Color(Color.black);
        foreach (Vector2 quad in drawList){
            GL.Vertex3(Camera.main.WorldToViewportPoint(new Vector3(quad.x, quad.y, 0)).x, Camera.main.WorldToViewportPoint(new Vector3(quad.x, quad.y, 0)).y, 0);
            //Debug.Log(Camera.main.WorldToViewportPoint(new Vector3(quad.x, quad.y, 0)).x + " " + Camera.main.WorldToViewportPoint(new Vector3(quad.x, quad.y, 0)).y);
        }
        GL.End();
        GL.PopMatrix();
        drawList = new List<Vector2>(); //reset the draw list
    }
}
