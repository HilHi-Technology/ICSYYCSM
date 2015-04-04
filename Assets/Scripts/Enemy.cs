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
    public Vector2 target;
    public float speed; //Speed of enemy
    private float patrolWait; //Wait time between 
    private bool isWaiting;
    static public NodeScript[] allNodes;
    public LayerMask pathMask;
    public LayerMask playerSightMask;
    public GameObject player;
    private LightScript lightScript;
    private bool seenByPlayer;
    private SpriteRenderer renderer;
    private Vector2[] vertices;

    // Use this for initialization
    void Awake() {
        isWaiting = false; //Reset the waiting state
        target = default(Vector2);

        allNodes = FindObjectsOfType(typeof(NodeScript)) as NodeScript[];
        lightScript = player.GetComponent<LightScript>();
        //Debug.Log(Vector2.Angle(new Vector2(0, 0), new Vector2(0, 2)));
        vertices = GetComponent<PolygonCollider2D>().points;
    }

    void Start() {
        renderer = GetComponent<SpriteRenderer>();

    }
    // Update is called once per frame
    void Update() {
        //Debug.Log(renderer.isVisible);
        patrolNodes = AStar(dest.transform.position, allNodes, pathMask);
        target = patrolNodes[current_dest];
        //ConeOfVision(target - (Vector2)transform.position, 45, 4, player, pathMask);
        if (isWaiting) {
            patrolWait += Time.deltaTime; //Increment the timer
            if (patrolWait >= 0f) {
                patrolWait = 0;
                isWaiting = false;
            }
        } else {
            if (patrolNodes.Count != 0 && seenByPlayer) {
                look_at(gameObject, target);
                if (move_to(target, speed)) { //move toward the next node and return true if it reaches the node
                    isWaiting = true; //Wait a bit before going to the next dest
                    current_dest++; //Set the next destination
                    if (current_dest >= patrolNodes.Count) { //If the final destination is reached
                        current_dest = 0;
                        patrolNodes.Reverse(); //Reverse the destination
                    }
                }
            }
        }
        seenByPlayer = false;
        if (lightScript.areEyesClosed) {
            seenByPlayer = false;
        }
        else {
            foreach (Vector2 i in vertices) {
                Vector2 vertex = transform.TransformPoint(i);
                RaycastHit2D ray = Physics2D.Raycast(vertex, (Vector2)player.transform.position - vertex, Mathf.Infinity, playerSightMask);
                if (ray.collider.tag == "Player") {
                    seenByPlayer = true;
                    break;
                }
            }
        }
        Debug.Log(seenByPlayer);
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
                //Gizmos.DrawLine(patrolNodes[i - 1], patrolNodes[i]);
            }
        }
    }

    List<Vector2> AStar(Vector2 destination, NodeScript[] roomNodes, LayerMask mask) {
        //Physics2D.raycastsStartInColliders = false;
        List<Vector2> pathVectors = new List<Vector2>(); //Will return this. Contains all the vectors needed to generate the path. Derived from the nodes themselves
        List<GameObject> pathNodes= new List<GameObject>(); //Store path nodes
        PriorityQueue<GameObject> frontier = new PriorityQueue<GameObject>(); //Priority queue used to find the shortest path.
        Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();//Keeps track of paths for pathfinding. More specifically keeps track of the node used to reach the current node. Will be used to reconstruct the path later.
        Dictionary<GameObject, float> costSoFar = new Dictionary<GameObject, float>(); //Costs in distance for a path. Used in conjunction with priority queue to find the shortest path.

        if (Physics2D.CircleCast(transform.position, 0.5f, destination - (Vector2)transform.position, Vector2.Distance(destination, transform.position), mask).collider == null) {
            //Check if the player has a direct path to the destination and return the straight path to goal if so.
            pathVectors.Add(transform.position);
            pathVectors.Add(destination);
            return pathVectors; 
        }

        //Create new destination node.
        GameObject destNode = new GameObject(); 
        destNode.transform.position = destination;
        NodeScript destScript = destNode.AddComponent<NodeScript>();
        destScript.mask = mask;
        destScript.FindOtherNodes();

        if (destScript.neighbors.Count == 0) {
            //If there isn't a path to the goal, cleanup and return an empty path.
            Destroy(destNode);
            pathVectors.Clear();
            return pathVectors;
        }

        //Creating a new start node
        GameObject startNode = new GameObject();
        startNode.transform.position = transform.position;
        NodeScript startScript = startNode.AddComponent<NodeScript>();
        startScript.mask = mask;
        startScript.FindOtherNodes();


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
            //Derive path vectors from the nodes.
            pathVectors.Add(node.transform.position);
        }
        foreach (NodeScript node in allNodes) {
            //Disconnect destination node and start node from the rest.
            if (node.neighbors.Contains(destNode)) {
                node.neighbors.Remove(destNode);
            }
            if (node.neighbors.Contains(startNode)) {
                node.neighbors.Remove(startNode);
            }
        }
        Destroy(startNode);
        Destroy(destNode);

        return pathVectors;
    }

    //bool ConeOfVision(Vector2 direction, float widthAngle, float distance, GameObject target, LayerMask mask, int amount = 10) {

    //    bool ret = false;
    //    float dirAngle = Vector2.Angle(direction, new Vector2(1, 0));
    //    if (direction.y < 0) {
    //        dirAngle = 360 - dirAngle;
    //    }
    //    //Debug.Log(dirAngle);

    //    for (float angle = dirAngle - widthAngle/2; angle <= dirAngle + widthAngle/2; angle += widthAngle/amount) {
    //        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
    //        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
    //        Vector2 rayVector = new Vector2(cos, sin);
    //        //Debug.Log(rayVector);
    //        RaycastHit2D ray = Physics2D.Raycast(transform.position, rayVector, distance, mask);
    //        if (ray.collider != null) {
    //            Debug.DrawLine(transform.position, ray.point, Color.red);
    //        } else {
    //            Debug.DrawLine(transform.position, (Vector2)transform.position + rayVector * distance, Color.red);
    //        }
    //    }
    //    return ret;
    //}
}
