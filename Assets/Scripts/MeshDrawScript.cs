using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshDrawScript : MonoBehaviour {
    public static List<Vector3> meshDrawList = new List<Vector3>();
    private Vector2 previousPoint;
    private List<Vector2> vList = new List<Vector2>();
    private Vector2 mouse_pos;
    public LayerMask rayMask;
    public GameObject MeshObj;
    public GameObject MeshObjParent;
    public List<GameObject> MeshList = new List<GameObject>();
	// Use this for initialization
	void Start () {
        //allMeshes = FindObjectsOfType(typeof(PolygonCollider2D)) as PolygonCollider2D[];
        
        
        //Physics2D.raycastsHitTriggers = true;
        //Physics2D.raycastsStartInColliders = false;
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        int i = 0;
        previousPoint = transform.position;
        foreach (GameObject obj in MeshList) {
            Destroy(obj);
        }
        //mesh.vertices = new Vector3[];
        mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PolygonCollider2D[] allMeshes = FindObjectsOfType(typeof(PolygonCollider2D)) as PolygonCollider2D[];
        //Debug.Log(allMeshes);
	    foreach(PolygonCollider2D col in allMeshes){
            //PolygonCollider2D col = obj.GetComponent<PolygonCollider2D>();
            foreach (Vector2 vLocal in col.points){ 
                Vector2 v = col.transform.TransformPoint(vLocal);
                vList.Add(v);
                //Debug.Log(v.ToString());
                //Debug.DrawLine(previousPoint, v, Color.white);
                //previousPoint = v;
            }
        }
        vList.Sort((item1, item2) => (Mathf.Atan2(item1.x - transform.position.x, item1.y - transform.position.y).CompareTo(Mathf.Atan2(item2.x - transform.position.x, item2.y - transform.position.y))));

        foreach (Vector2 v in vList) {
            i++;
            //Debug.Log(Vector2.Angle(transform.position, v));
            if (i > 1) {
                Debug.DrawLine(previousPoint, v, Color.red);
                RaycastHit2D previousRay = Physics2D.Raycast(transform.position, previousPoint - (Vector2)transform.position, (previousPoint - (Vector2)transform.position).magnitude, rayMask);
                RaycastHit2D ray = Physics2D.Raycast(transform.position, v - (Vector2)transform.position, (v - (Vector2)transform.position).magnitude, rayMask);
                //Debug.Log(ray.point);
            
                if (previousRay.collider == null) {
                    Debug.Log(previousRay.collider != null);
                    //previousRay.transform.parent.transform.position = new Vector3(2, 2, 0);
                    Debug.DrawLine(transform.position, previousRay.point, Color.blue);
                    Debug.DrawLine(transform.position, previousPoint, Color.green);
                    Debug.Log(previousPoint + " " + i);
                }
                if (ray.collider == null) {
                    Debug.Log(previousRay.collider != null);
                    //previousRay.transform.parent.transform.position = new Vector3(2, 2, 0);
                    Debug.DrawLine(transform.position, ray.point, Color.black);
                }
            

                //if (previousRay.collider != null && previousRay.collider != null) {
                
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
        RaycastHit2D previousRay2 = Physics2D.Raycast(transform.position, previousPoint - (Vector2)transform.position, (previousPoint - (Vector2)transform.position).magnitude * 2, rayMask);
        RaycastHit2D ray2 = Physics2D.Raycast(transform.position, vList[0] - (Vector2)transform.position, (vList[0] - (Vector2)transform.position).magnitude, rayMask);
        Debug.DrawLine(previousPoint, vList[0], Color.red);


        GameObject obj2 = Instantiate(MeshObj, Vector3.zero, Quaternion.identity) as GameObject;
        obj2.transform.parent = MeshObjParent.transform;
        MeshList.Add(obj2);
        MeshDraw scr2 = obj2.GetComponent<MeshDraw>();
        scr2.vertices[0] = previousRay2.point;
        //Debug.Log(previousRay.point + " " + i);
        scr2.vertices[1] = transform.position;
        scr2.vertices[2] = ray2.point;

        scr2.colors[0] = new Color(1, 1, 1, 0.5f);
        scr2.colors[1] = new Color(1, 1, 1, 0.5f); //White
        scr2.colors[2] = new Color(1, 1, 1, 0.5f);
        //Debug.DrawLine(transform.position, ray2.point, Color.white
        vList.Clear();
        //Debug.Log(Mathf.Atan2(mouse_pos.x - transform.position.x, mouse_pos.y - transform.position.y) * Mathf.Rad2Deg);
        
	}
}
