using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightScript : MonoBehaviour {
    private Vector2 previousPoint; //Store the last vertex in order to draw light
    private List<Vector2> vList = new List<Vector2>(); //Contains all the vertices in the game, and will sort those vertex
    public LayerMask rayMask; //Used to only cast light rays on certain layers of objects
    public GameObject DrawObj; //Objects that are used to draw the lights. One will be created for each triangle of light drawn
    public List<GameObject> DrawList = new List<GameObject>(); //The list that contains all the mesh objects to destroy them after every loop
    public int shadow_length;
    public float shadow_offset;

    public Transform eyelids;
    private SpriteRenderer eyelidsRenderer;
    private bool eyesClosed;
    public float eyesCloseTime;
    public float eyesOpenTime;
    private Color newEyelidsColor;

    public float eyeClosedVisionRadius;
    public float eyeClosedShadowRadius;
    // Use this for initialization
    void Start() {
        eyelidsRenderer = eyelids.GetComponent<SpriteRenderer>();
        eyelidsRenderer.color = new Color(eyelidsRenderer.color.r, eyelidsRenderer.color.g, eyelidsRenderer.color.b, 1f);
        eyesClosed = false;

        newEyelidsColor = new Color(eyelidsRenderer.color.r, eyelidsRenderer.color.g, eyelidsRenderer.color.b, 1f);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Blink") && eyesClosed) {
            Debug.Log("open");
            //newEyelidsColor = new Color(eyelidsRenderer.color.r, eyelidsRenderer.color.g, eyelidsRenderer.color.b, 0);
            eyesClosed = false;
        }
        else if (Input.GetButtonDown("Blink") && !eyesClosed) {
            Debug.Log("close");
            //newEyelidsColor = new Color(eyelidsRenderer.color.r, eyelidsRenderer.color.g, eyelidsRenderer.color.b, 1);
            eyesClosed = true;
        }

        //if (!eyesClosed) {
        //    eyelidsRenderer.color = Color.Lerp(eyelidsRenderer.color, newEyelidsColor, eyesOpenTime * Time.deltaTime);
        //}
        //if (eyesClosed) {
        //    eyelidsRenderer.color = Color.Lerp(eyelidsRenderer.color, newEyelidsColor, eyesCloseTime * Time.deltaTime);
        //}
        

        foreach (GameObject obj in DrawList) { //Destroy all objects used to draw lighting
            Mesh sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
            Destroy(sharedMesh);
            Destroy(obj);
        }
        DrawList.Clear();


        if (!eyesClosed) {
            previousPoint = transform.position; //Initialize previous point

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


            foreach (Vector2 v in vList) { //For all vertices
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
                    DrawRectangle(shadow_prev_point, shadow_prev_point_extend, shadow_cur_point, shadow_cur_point_extend, Color.black);
                    DrawTriangle(light_prev_point, player_pos, light_cur_point, new Color(1, 1, 1, 0.5f));

                }
                previousPoint = v;


            }
            vList.Clear();
        } else {
            float angleStep = 1;
            float angle = angleStep;
            for (; angle < 360; angle += angleStep) {
                float prevCos = Mathf.Cos(angle - angleStep * Mathf.Deg2Rad);
                float prevSin = Mathf.Sin(angle - angleStep * Mathf.Deg2Rad);
                float curCos = Mathf.Cos(angle * Mathf.Deg2Rad);
                float curSin = Mathf.Sin(angle * Mathf.Deg2Rad);

                Vector2 prevPoint = new Vector2(prevCos, prevSin).normalized * eyeClosedVisionRadius + (Vector2)transform.position;
                Vector2 prevPointExtend = prevPoint.normalized * eyeClosedShadowRadius + (Vector2)transform.position;
                Vector2 curPoint = new Vector2(curCos, curSin).normalized * eyeClosedVisionRadius + (Vector2)transform.position;
                Vector2 curPointExtend = curPoint.normalized * eyeClosedShadowRadius + (Vector2)transform.position;

                DrawRectangle(prevPoint, prevPointExtend, curPoint, curPointExtend, Color.black);
            }
            //Complete the last angle draw
            float prevCos2 = Mathf.Cos(angle - angleStep * Mathf.Deg2Rad);
            float prevSin2 = Mathf.Sin(angle - angleStep * Mathf.Deg2Rad);
            float curCos2 = Mathf.Cos(angle * Mathf.Deg2Rad);
            float curSin2 = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 prevPoint2 = new Vector2(prevCos2, prevSin2).normalized * eyeClosedVisionRadius;
            Vector2 prevPointExtend2 = prevPoint2.normalized * eyeClosedShadowRadius;
            Vector2 curPoint2 = new Vector2(curCos2, curSin2).normalized * eyeClosedVisionRadius;
            Vector2 curPointExtend2 = curPoint2.normalized * eyeClosedShadowRadius;

            DrawRectangle(prevPoint2, prevPointExtend2, curPoint2, curPointExtend2, Color.black);
        }
    }

    void DrawRectangle(Vector3 botLeft, Vector3 topLeft, Vector3 botRight, Vector3 topRight, Color color) {
        //Create draw objects for each light triangles being drawn in order to draw them.
        //These objects and their meshes must be deleted at the beginning of the next loop, otherwise memory leaks and funky things happen.
        GameObject shadow_obj = Instantiate(DrawObj, Vector3.zero, Quaternion.identity) as GameObject;
        DrawList.Add(shadow_obj); //Add draw objects to the this list to be removed eventually
        MeshDraw shadow_scr = shadow_obj.GetComponent<MeshDraw>();
        shadow_scr.vertices = new Vector3[4];
        shadow_scr.vertices[0] = (Vector3)botLeft;
        shadow_scr.vertices[1] = (Vector3)topLeft;
        shadow_scr.vertices[2] = (Vector3)botRight;
        shadow_scr.vertices[3] = (Vector3)topRight;

        //Set the color of the vision
        shadow_scr.colors = new Color[4];
        shadow_scr.colors[0] = color; //Previous point
        shadow_scr.colors[1] = color; //This vertex is the player's position
        shadow_scr.colors[2] = color; //Current point
        shadow_scr.colors[3] = color; //Current point
        shadow_scr.isTriangle = false;
        //Debug.DrawLine(transform.position, ray.point, Color.white);
    }
    void DrawTriangle(Vector3 point1, Vector3 point2, Vector3 point3, Color color) {
        //These objects and their meshes must be deleted at the beginning of the next loop, otherwise memory leaks and funky things happen.
        GameObject light_obj = Instantiate(DrawObj, Vector3.zero, Quaternion.identity) as GameObject;
        DrawList.Add(light_obj); //Add draw objects to the this list to be removed eventually
        MeshDraw light_scr = light_obj.GetComponent<MeshDraw>();

        light_scr.vertices = new Vector3[3];
        light_scr.vertices[0] = point1;
        light_scr.vertices[1] = point2;
        light_scr.vertices[2] = point3;

        light_scr.colors = new Color[3];
        light_scr.colors[0] = color;
        light_scr.colors[1] = color; //White
        light_scr.colors[2] = color;
        light_scr.isTriangle = true;
    }
}
