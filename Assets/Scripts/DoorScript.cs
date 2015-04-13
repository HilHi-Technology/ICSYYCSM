using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {
    private Vector2 startingPoint;
    private Vector2[] vertices;
    private bool seenByPlayer;
    public LayerMask playerSightMask;
    public GameObject player;
    private Rigidbody2D rigidBody2D;
	// Use this for initialization
	void Start () {
        startingPoint = transform.position;
        vertices = GetComponent<PolygonCollider2D>().points;
        rigidBody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(startingPoint.x, startingPoint.y, transform.position.z);
        seenByPlayer = false;
        if (LightScript.areEyesClosed) {
            seenByPlayer = false;
        } else {
            foreach (Vector2 i in vertices) {
                Vector2 vertex = transform.TransformPoint(i);
                RaycastHit2D ray = Physics2D.Raycast(vertex, (Vector2)player.transform.position - vertex, Mathf.Infinity, playerSightMask);
                //Debug.Log(ray.collider.tag);
                //Debug.DrawLine(vertex, ray.point, Color.white);
                if (ray.collider.tag == "Player") {
                    seenByPlayer = true;
                    break;
                }
            }
        }

        if (seenByPlayer) {
            transform.position = new Vector3(transform.position.x, transform.position.y,- 5);
            rigidBody2D.fixedAngle = false;
        } else {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            rigidBody2D.fixedAngle = true;
        }
	}
}
