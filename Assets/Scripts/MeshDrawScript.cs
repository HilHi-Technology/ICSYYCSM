using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshDrawScript : MonoBehaviour {
    private Vector2 previousPoint; //Store the last vertex in order to draw the vision
    private List<Vector2> vList = new List<Vector2>(); //Contains all the vertex in the game, and will sort those vertex
    private Vector2 mouse_pos; 
    public LayerMask rayMask; //Used to only cast light rays on certain layers of objects
    public GameObject MeshObj; //Mesh objects that are used to draw the lights
    public GameObject MeshObjParent; //Used to group all mesh objects together 
    public List<GameObject> MeshList = new List<GameObject>(); //The list that contains all the mesh objects to destroy them after every loop
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        previousPoint = transform.position; //Initialize previous point, but it won't be used until it receives another 
        foreach (GameObject obj in MeshList) {
            Destroy(obj);
        }
        //mesh.vertices = new Vector3[];
        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PolygonCollider2D[] allMeshes = FindObjectsOfType(typeof(PolygonCollider2D)) as PolygonCollider2D[];
        //Debug.Log(allMeshes);
        foreach (PolygonCollider2D col in allMeshes) {
            foreach (Vector2 vLocal in col.points) {
                Vector2 v = col.transform.TransformPoint(vLocal);
                vList.Add(v);
                //Debug.Log(v.ToString());
                //Debug.DrawLine(previousPoint, v, Color.white);
                //previousPoint = v;
            }
        }
        vList.Sort((item1, item2) => (Mathf.Atan2(item1.x - transform.position.x, item1.y - transform.position.y).CompareTo(Mathf.Atan2(item2.x - transform.position.x, item2.y - transform.position.y))));
        vList.Add(vList[0]);
        foreach (Vector2 v in vList) {
            //Debug.Log(Vector2.Angle(transform.position, v));
            if (previousPoint != (Vector2)transform.position) {
                Debug.DrawLine(previousPoint, v, Color.red);
                RaycastHit2D previousRay = Physics2D.CircleCast(transform.position, 0.0009f, previousPoint - (Vector2)transform.position, 2000, rayMask);
                RaycastHit2D ray = Physics2D.CircleCast(transform.position, 0.0009f, v - (Vector2)transform.position, 2000, rayMask);


                GameObject obj = Instantiate(MeshObj, Vector3.zero, Quaternion.identity) as GameObject;
                obj.transform.parent = MeshObjParent.transform;
                MeshList.Add(obj);
                MeshDraw scr = obj.GetComponent<MeshDraw>();
                scr.vertices[0] = previousRay.point;
                //Debug.Log(ray.point + " " + i);
                scr.vertices[1] = transform.position;
                scr.vertices[2] = ray.point;
                //Debug.DrawLine(transform.position, ray.point);

                scr.colors[0] = new Color(1, 1, 1, 0.5f);
                scr.colors[1] = new Color(1, 1, 1, 0.5f); //White
                scr.colors[2] = new Color(1, 1, 1, 0.5f);
                Debug.DrawLine(transform.position, ray.point, Color.white);
            }
            //}
            previousPoint = v;


        }
        vList.Clear();
    }
}
