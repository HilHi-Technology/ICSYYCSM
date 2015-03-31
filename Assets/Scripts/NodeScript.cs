using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeScript : MonoBehaviour {
    public List<GameObject> neighbors = new List<GameObject>();
    public LayerMask mask;
    
	// Use this for initialization
	void Start () {
        FindOtherNodes();
	}
	
    public void FindOtherNodes(){
        foreach (NodeScript nodeScr in Enemy.allNodes) {
            //Connect the node with any possible room nodes.
            GameObject node = nodeScr.gameObject;
            if (node != gameObject) {
                float distance = (node.transform.position - transform.position).magnitude;
                RaycastHit2D ray = Physics2D.CircleCast(transform.position, 0.5f, node.transform.position - transform.position, distance, mask);
                if (ray.collider == null) { //If ray reached the node without hitting a wall.
                    //Connect the nodes together unless they contain each other.
                    if (!nodeScr.neighbors.Contains(gameObject)) {
                        nodeScr.neighbors.Add(gameObject);
                    }
                    if (!neighbors.Contains(node)) {
                        neighbors.Add(node);
                    }
                }
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
