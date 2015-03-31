using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    //private GameObject start;
    public GameObject dest; //destination
    //private PriorityQueue<GameObject> frontier = new PriorityQueue<GameObject>(); //Queue used for pathfinding


    //Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();//Keeps track of paths for pathfinding. More specifically keeps track
    ////of the node used to reach the current node. Will be used to reconstruct the path later.
    //Dictionary<GameObject, float> costSoFar = new Dictionary<GameObject, float>(); //Costs in distance for a path
    private int current_dest; //Used for moving from patrol nodes to patrol nodes
    public List<Vector2> patrolNodes = new List<Vector2>(); //Store patrol nodes
    public float speed; //Speed of enemy
    private float patrolWait; //Wait time between 
    private bool isWaiting;
    private NodeScript[] allNodes;
    public LayerMask pathMask;
    // Use this for initialization
    void Start() {
        isWaiting = false; //Reset the waiting state

        allNodes = FindObjectsOfType(typeof(NodeScript)) as NodeScript[];
        


    }

    // Update is called once per frame
    void Update() {
        patrolNodes = AStar(dest.transform.position, allNodes, pathMask);
        if (isWaiting) {
            patrolWait += Time.deltaTime; //Increment the timer
            if (patrolWait >= 0f) {
                patrolWait = 0;
                isWaiting = false;
            }
        } else {
            if (patrolNodes.Count != 0) {
                if (move_to(patrolNodes[current_dest], speed)) { //move toward the next node and return true if it reaches the node
                    isWaiting = true; //Wait a bit before going to the next dest
                    current_dest++; //Set the next destination
                    if (current_dest >= patrolNodes.Count) { //If the final destination is reached
                        current_dest = 0;
                        patrolNodes.Reverse(); //Reverse the destination
                    }
                }
            }
        }
    }

    bool move_to(Vector2 destination, float speed) {
        /* Move toward a given destination
         * returns whether destination is reached (true if reached)
         */
        if (new Vector2(transform.position.x, transform.position.y) != destination) { //If destination isn't reached yet
            Vector2 movement = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            transform.position = new Vector3(movement.x, movement.y, transform.position.z); //Move toward destination
            return false;
        } else {
            return true; //Destination reached
        }
    }
    void look_at(GameObject obj, Vector3 target) {
        /*Makes the object rotate toward the given point
         *Example usage: lookAt(gameObject, Camera.main.ScreenToWorldPoint(Input.mousePosition)); //Object looks at mouse
         * 
         */

        //Vector2.angle here is used to get the angle between the (1,0) vector(the horizontal line) and the vector between the object and the mouse
        if (transform.position.y < target.y) { //If the mouse is on the top side of the object

            //Make the angle negative (e.g. if the mouse position relative to the object is (1,1), vector2.angle((0,1),(1,1)) would return 45, which is facing the left side.
            //If we make that number negative, it would face the right side.
            transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(new Vector2(1, 0), target - obj.transform.position));
        }
        if (transform.position.y > target.y) { //If the mouse is on the bottom side of the object
            transform.rotation = Quaternion.Euler(0, 0, -Vector2.Angle(new Vector2(1, 0), target - obj.transform.position));
        }
    }

    //Debug stuffs, don't worry about it
    void OnDrawGizmos() {
        //Patrol nodes
        Gizmos.color = Color.red;
        for (int i = 0; i < patrolNodes.Count; i++) {
            if (i != 0) {
                //Gizmos.DrawSphere(patrolNodes[i].transform.position, 1f);
                Gizmos.DrawLine(patrolNodes[i - 1], patrolNodes[i]);
            }
        }
    }

    List<Vector2> AStar(Vector2 destination, NodeScript[] roomNodes, LayerMask mask) {
        //Physics2D.raycastsStartInColliders = false;
        List<Vector2> pathVectors = new List<Vector2>();
        List<GameObject> pathNodes= new List<GameObject>(); //Store patrol nodes
        PriorityQueue<GameObject> frontier = new PriorityQueue<GameObject>(); //Queue used for pathfinding
        Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();//Keeps track of paths for pathfinding. More specifically keeps track
        //of the node used to reach the current node. Will be used to reconstruct the path later.
        Dictionary<GameObject, float> costSoFar = new Dictionary<GameObject, float>(); //Costs in distance for a path

        if (Physics2D.CircleCast(transform.position, 0.5f, destination - (Vector2)transform.position, Vector2.Distance(destination, transform.position), mask).collider == null) { //Check if the player has a direct path to the destination
            pathVectors.Add(transform.position);
            pathVectors.Add(destination);
            return pathVectors;
        }


        GameObject destNode = new GameObject();
        destNode.transform.position = destination;
        NodeScript destScript = destNode.AddComponent<NodeScript>();


        //Destination node creation
        bool foundGoal = false;
        foreach (NodeScript nodeScr in roomNodes) {
            GameObject node = nodeScr.gameObject;
            float distance = (destination - (Vector2)node.transform.position).magnitude;
            //Debug.Log(distance);
            RaycastHit2D ray = Physics2D.CircleCast(destination, 0.5f, (Vector2)node.transform.position - destination, distance, mask);

            if (ray.collider == null) { //If ray reached the node without hitting a wall
                //Debug.Log("got here");
                foundGoal = true;
                nodeScr.neighbors.Add(destNode);
                destScript.neighbors.Add(node);
            }
        }
        if (!foundGoal) {
            Destroy(destNode);
            pathVectors.Clear();
            return pathVectors;
        }

        GameObject startNode = new GameObject();
        startNode.transform.position = transform.position;
        NodeScript startScript = startNode.AddComponent<NodeScript>();

        //Finding best starting node
        foreach (NodeScript nodeScr in roomNodes) {
            GameObject node = nodeScr.gameObject;
            //Debug.Log(node);
            float distance = (node.transform.position - transform.position).magnitude;
            RaycastHit2D ray = Physics2D.CircleCast(transform.position, 0.5f, node.transform.position - transform.position, distance, mask);
            //Debug.Log(ray.collider);
            if (ray.collider == null) { //If ray reached the node without hitting a wall
                nodeScr.neighbors.Add(startNode);
                startScript.neighbors.Add(node);
            }
        }

        //foreach (GameObject node in destScript.neighbors) {
        //    Debug.Log(node);
        //}


        //The pathfinding algorithm used is A*. The resource used is http://www.redblobgames.com/pathfinding/a-star/introduction.html

        //cameFrom is a dictionary. The key stores the current node, and the value stores the previous node used to reach the current node. 
        cameFrom.Add(startNode, default(GameObject)); //The first node doesn't come from anywhere, so default is used to pretty much denotes null

        //costSoFar's key denotes current node, while the value stores the cost in distance it takes to get from the start to the node.
        costSoFar.Add(startNode, 0);

        //frontier is a priority queue, which basically sorts whatever is put into it in order from smallest to largest. Read up on A* to see why this is used.
        //1st parameter of insert() is the node, while the 2nd parameter is the cost it takes to get there.
        frontier.insert(startNode, 0);

        while (!frontier.isEmpty()) {
            GameObject current = frontier.get(); //Get the current node to expand upon
            //Debug.Log(current);
            List<GameObject> neighbors = current.GetComponent<NodeScript>().neighbors; //Get the node's neighbors, to be expanded upon

            if (current == destNode) { //If the frontier that is about to be expanded is the goal then we've reached the goal. At that point we can construct a path to the goal using cameFrom.
                
                break; //Found our destination

            }
            foreach (GameObject next in neighbors) { //Go through all neighbors in the current frontier node
                float newCost = costSoFar[current] + Vector2.Distance(current.transform.position, next.transform.position); //Calculate cost to reach each neighbor
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) { //If the frontier isn't already expanded or we found a faster path.
                    costSoFar[next] = newCost; //Give the neighbor node a cost.
                    frontier.insert(next, newCost);
                    cameFrom[next] = current; //The neighbor's node came from the current node
                }
            }
        }


        //PATH RECONSTRUCTION
        //Path is reconstructed backward, starting from the goal and trace its way to the start using cost so far. Patrol nodes are added along the paths for the enemy to move toward.
        GameObject p_current = destNode;
        pathNodes.Add(p_current);
        while (p_current != startNode) { //Add nodes until we reach the start.
            //Debug.Log(p_current);
            p_current = cameFrom[p_current];
            pathNodes.Add(p_current);
        }
        pathNodes.Reverse(); //Reverse the patrol nodes because they are added in backward .
        

        foreach (GameObject node in pathNodes) {
            pathVectors.Add(node.transform.position);
        }
        foreach (NodeScript node in allNodes) {
            if (node.neighbors.Contains(destNode)) {
                node.neighbors.Remove(destNode);
            }
            if (node.neighbors.Contains(startNode)) {
                node.neighbors.Remove(startNode);
            }
        }
        Destroy(startNode);
        Destroy(destNode);
        //Debug.Log(destNode);

        return pathVectors;
    }
}
