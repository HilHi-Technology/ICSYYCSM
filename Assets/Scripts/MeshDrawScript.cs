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
    public int shadow_length;
    public float shadow_offset;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        previousPoint = transform.position; //Initialize previous point, but it won't be used until it receives another 

        foreach (GameObject obj in DrawList) { //Destroy all objects used to draw lighting
            Destroy(obj);
        }
        DrawList.Clear();
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

                //Shoot a ray at previous vertex and current vertex. Circlecast is used to increase the ray width, so it won't miss the vertex it's shooting at.
                RaycastHit2D previousRay = Physics2D.CircleCast(transform.position, 0.0009f, previousPoint - (Vector2)transform.position, 2000, rayMask); 
                RaycastHit2D ray = Physics2D.CircleCast(transform.position, 0.0009f, v - (Vector2)transform.position, 2000, rayMask);

                //Set vertices to be drawn
                Vector2 shadow_prev_point = previousRay.point + (previousPoint - (Vector2)transform.position).normalized * shadow_offset;
                Vector2 shadow_prev_point_extend = previousRay.point + (previousPoint - (Vector2)transform.position).normalized * shadow_length;
                Vector2 shadow_cur_point = ray.point + (v - (Vector2)transform.position).normalized * shadow_offset;
                Vector2 shadow_cur_point_extend = ray.point + (v - (Vector2)transform.position).normalized * shadow_length;

                Vector2 light_prev_point = previousRay.point;
                Vector2 player_pos = transform.position;
                Vector2 light_cur_point = ray.point;

                if (previousRay.collider != ray.collider) { //If the 2 vertices are not part of the same object
                    //Shoot another ray to extend the vertex to the next 
                    Vector2 start = ray.point + ((v - (Vector2)transform.position).normalized * 0.05f);
                    Vector2 pStart = previousRay.point + ((previousPoint - (Vector2)transform.position).normalized * 0.05f);
                    RaycastHit2D previousRay2 = Physics2D.Raycast(pStart, previousPoint - (Vector2)transform.position, 2000, rayMask);
                    RaycastHit2D ray2 = Physics2D.Raycast(start, v - (Vector2)transform.position, 2000, rayMask);

                    shadow_prev_point = previousRay2.point + (previousPoint - (Vector2)transform.position).normalized * shadow_offset;
                    shadow_prev_point_extend = previousRay2.point + (previousPoint - (Vector2)transform.position).normalized * shadow_length;
                    shadow_cur_point = ray2.point + (v - (Vector2)transform.position).normalized * shadow_offset;
                    shadow_cur_point_extend = ray2.point + (v - (Vector2)transform.position).normalized * shadow_length;

                    light_prev_point = previousRay2.point;
                    player_pos = transform.position;
                    light_cur_point = ray2.point;

                    Debug.DrawLine(pStart, previousRay2.point, Color.yellow);
                    Debug.DrawLine(start, ray2.point, Color.yellow);
                }

                //Create draw objects for each light triangles being drawn in order to draw them.
                GameObject shadow_obj = Instantiate(DrawObj, Vector3.zero, Quaternion.identity) as GameObject; 
                DrawList.Add(shadow_obj); //Add draw objects to the this list to be removed eventually
                MeshDraw shadow_scr = shadow_obj.GetComponent<MeshDraw>();
                shadow_scr.vertices = new Vector3[4];
                shadow_scr.vertices[0] = (Vector3)shadow_prev_point;
                shadow_scr.vertices[1] = (Vector3)shadow_prev_point_extend;
                shadow_scr.vertices[2] = (Vector3)shadow_cur_point;
                shadow_scr.vertices[3] = (Vector3)shadow_cur_point_extend;

                //Set the color of the vision
                shadow_scr.colors = new Color[4];
                shadow_scr.colors[0] = new Color(0, 0, 0, 1f); //Previous point
                shadow_scr.colors[1] = new Color(0, 0, 0, 1f); //This vertex is the player's position
                shadow_scr.colors[2] = new Color(0, 0, 0, 1f); //Current point
                shadow_scr.colors[3] = new Color(0, 0, 0, 1f); //Current point
                shadow_scr.is_light = false;
                Debug.DrawLine(transform.position, ray.point, Color.white);

                GameObject light_obj = Instantiate(DrawObj, Vector3.zero, Quaternion.identity) as GameObject;
                DrawList.Add(light_obj); //Add draw objects to the this list to be removed eventually
                MeshDraw light_scr = light_obj.GetComponent<MeshDraw>();

                light_scr.vertices = new Vector3[3];
                light_scr.vertices[0] = light_prev_point;
                light_scr.vertices[1] = player_pos;
                light_scr.vertices[2] = light_cur_point;
                //light_scr.vertices[3] = new Vector3(9999,9999,9999); //Generic number that pretty much represents null

                light_scr.colors = new Color[3];
                light_scr.colors[0] = new Color(1, 1, 1, 0.5f);
                light_scr.colors[1] = new Color(1, 1, 1, 0.5f); //White
                light_scr.colors[2] = new Color(1, 1, 1, 0.5f);
                light_scr.is_light = true;
                //}
                
            }
            previousPoint = v;


        }
        vList.Clear();
    }
}
