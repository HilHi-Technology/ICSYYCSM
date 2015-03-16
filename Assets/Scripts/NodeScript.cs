using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeScript : MonoBehaviour {
    public List<GameObject> neighbors = new List<GameObject>();
    
	// Use this for initialization
	void Start () {
        foreach (GameObject node in neighbors) {
            NodeScript scr = node.GetComponent<NodeScript>();
            if (!scr.neighbors.Contains(gameObject)) {
                scr.neighbors.Add(gameObject);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f);

        foreach (GameObject node in neighbors) {
            Gizmos.DrawLine(transform.position, node.transform.position);
        }
    }
}
