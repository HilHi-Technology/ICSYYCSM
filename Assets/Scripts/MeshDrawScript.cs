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
    public List<GameObject> MeshList = new List<GameObject>();
	// Use this for initialization
	void Start () {
        //allMeshes = FindObjectsOfType(typeof(PolygonCollider2D)) as PolygonCollider2D[];
        
        previousPoint = transform.position;
        //Physics2D.raycastsHitTriggers = true;
        //Physics2D.raycastsStartInColliders = false;
	}
	
	// Update is called once per frame
	void Update () {
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
            //Debug.Log(Vector2.Angle(transform.position, v));
            Debug.DrawLine(previousPoint, v, Color.red);
            previousPoint = v;
            RaycastHit2D ray = Physics2D.Raycast(transform.position, v - (Vector2)transform.position, (v - (Vector2)transform.position).magnitude, rayMask);
            //Debug.Log(ray.point);



            if (ray != null) {
                GameObject obj = Instantiate(MeshObj) as GameObject;
                MeshList.Add(obj);
                MeshDraw scr = obj.GetComponent<MeshDraw>();
                
                Debug.DrawLine(transform.position, ray.point, Color.white);
            }
            else {
                Debug.DrawLine(transform.position, v, Color.white);
            }



        }

        vList.Clear();
        //Debug.Log(Mathf.Atan2(mouse_pos.x - transform.position.x, mouse_pos.y - transform.position.y) * Mathf.Rad2Deg);
        
	}
}
