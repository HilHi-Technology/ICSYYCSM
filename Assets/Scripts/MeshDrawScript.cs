using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshDrawScript : MonoBehaviour {
    private Vector2 previousPoint; //Store the last vertex in order to draw light
    private List<Vector2> vList = new List<Vector2>(); //Contains all the vertex in the game, and will sort those vertex
    private Vector2 mouse_pos; 
    public LayerMask rayMask; //Used to only cast light rays on certain layers of objects
    public GameObject DrawObj; //Objects that are used to draw the lights. One will be created for each triangle of light drawn
    public List<GameObject> DrawList = new List<GameObject>(); //The list that contains all the mesh objects to destroy them after every loop
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        previousPoint = transform.position; //Initialize previous point, but it won't be used until it receives another 

        foreach (GameObject obj in DrawList) { //Destroy all objects used to draw lighting
            Destroy(obj);
        }
        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get the mouse position in world coordinates

        //Find all vertices and add it to a list
        PolygonCollider2D[] allMeshes = FindObjectsOfType(typeof(PolygonCollider2D)) as PolygonCollider2D[]; 
        foreach (PolygonCollider2D col in allMeshes) {
            foreach (Vector2 vLocal in col.points) {
                Vector2 v = col.transform.TransformPoint(vLocal);
                vList.Add(v);
            }
        }

        //Sort the vertices according to its angle to player
        vList.Sort((item1, item2) => (Mathf.Atan2(item1.x - transform.position.x, item1.y - transform.position.y).CompareTo(Mathf.Atan2(item2.x - transform.position.x, item2.y - transform.position.y))));
        vList.Add(vList[0]); //Add the last vertex in the series to complete the full circle of light


        foreach (Vector2 v in vList) { //For all vectors
            if (previousPoint != (Vector2)transform.position) { //Skip drawing on the first vertex, because there are not enough points to draw a triangle
                Debug.DrawLine(previousPoint, v, Color.red); //Draw debug lines connecting all the vertices

                //Shoot a ray at previous vertex and current vertex
                RaycastHit2D previousRay = Physics2D.CircleCast(transform.position, 0.0009f, previousPoint - (Vector2)transform.position, 2000, rayMask); 
                RaycastHit2D ray = Physics2D.CircleCast(transform.position, 0.0009f, v - (Vector2)transform.position, 2000, rayMask);

                //Set vertices to be drawn
                Vector2 vert0 = previousRay.point;
                Vector2 vert1 = transform.position;
                Vector2 vert2 = ray.point;

                if (previousRay.collider != ray.collider) { //If the 2 vertices are not part of the same object
                    //Shoot another ray to extend the vertex to the next 
                    Vector2 start = ray.point + ((v - (Vector2)transform.position).normalized * 0.05f); //(0.0009, 0.8*0.0009)
                    Vector2 pStart = previousRay.point + ((previousPoint - (Vector2)transform.position).normalized * 0.05f);
                    RaycastHit2D previousRay2 = Physics2D.Raycast(pStart, previousPoint - (Vector2)transform.position, 2000, rayMask);
                    RaycastHit2D ray2 = Physics2D.Raycast(start, v - (Vector2)transform.position, 2000, rayMask);
                    vert0 = previousRay2.point;
                    vert1 = transform.position;
                    vert2 = ray2.point;
                    Debug.DrawLine(pStart, previousRay2.point, Color.yellow); 
                    Debug.DrawLine(start, ray2.point, Color.yellow);
                }

                GameObject obj = Instantiate(DrawObj, Vector3.zero, Quaternion.identity) as GameObject;
                DrawList.Add(obj);
                MeshDraw scr = obj.GetComponent<MeshDraw>();
                scr.vertices[0] = vert0;
                //Debug.Log(ray.point + " " + i);
                scr.vertices[1] = vert1;
                scr.vertices[2] = vert2;
                //Debug.DrawLine(transform.position, ray.point);

                scr.colors[0] = new Color(1, 1, 1, 0.5f);
                scr.colors[1] = new Color(1, 1, 1, 0.5f); //White
                scr.colors[2] = new Color(1, 1, 1, 0.5f);
                Debug.DrawLine(transform.position, ray.point, Color.white);
                //}
                
            }
            previousPoint = v;


        }
        vList.Clear();
    }
}
