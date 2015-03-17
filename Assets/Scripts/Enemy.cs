using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public GameObject start;
    public GameObject dest;
    private PriorityQueue<int> frontier = new PriorityQueue<int>();
    Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();
    Dictionary<GameObject, float> costSoFar = new Dictionary<GameObject, float>();
    private int current_dest; 
    public List<GameObject> moveNodes = new List<GameObject>();
    public float speed;

    // Use this for initialization
    void Start() {
        //cameFrom.Add(start, default(GameObject));
        //costSoFar.Add(start, 0);
        //frontier.insert(start, 0);
        //int c = 0;
        ////Debug.Log(start.GetComponent<NodeScript>().neighbors);
        //for(int i = 0; i < 10; i++){
        //    //c++;
        //    Debug.Log("got here");
        //    GameObject current = frontier.get(); //Get the current node
        //    List<GameObject> neighbors = current.GetComponent<NodeScript>().neighbors; //Get the node's neighbors

        //    if (current == dest) {
        //        break; //Found our destination
        //    }
        //    //frontier.insert(current, 0);
        //    Debug.Log("Current: " + current);
        //    foreach (GameObject next in neighbors) { //Go through all neighbors
        //        float newCost = costSoFar[current] + Vector2.Distance(current.transform.position, next.transform.position); //Calculate cost
        //        Debug.Log("Neighbors: " + next + " Cost: " + newCost);
        //        //if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) { //If the frontier has already been expanded or we found a faster path
        //        costSoFar[next] = newCost; 
        //            //float priority = newCost;
        //        //Debug.Log(!costSoFar.ContainsKey(next));
        //            frontier.insert(next, newCost);
        //            cameFrom[next] = current;
        //        //}
        //    }
        //}
        //GameObject current2 = dest;
        //moveNodes.Add(current2);
        //while (current2 != start) {
        //    current2 = cameFrom[current2];
        //    //Debug.Log(current2);
        //    moveNodes.Add(current2);
        //}
        //moveNodes.Reverse();

        //Debug.Log(frontier.Count());
        //for(int i = 0; i < frontier.Count(); i++){
        //    if (i != 0) {
        //        Debug.Log(frontier.queue[i].Item1.transform.position);
        //    }
        //}

        for (int i = 0; i < 3; i++) {
            int n = Random.Range(0, 10);
            frontier.insert(n, n);
        }

        //for (int i = 1; i < 10; i++) {
        //    Debug.Log(frontier.queue[i]);
        //}

    }

    // Update is called once per frame
    void Update() {
        if (moveNodes.Count != 0) {
            if (move_to(moveNodes[current_dest].transform.position, speed)) {
                current_dest++;
                if (current_dest >= moveNodes.Count) {
                    current_dest = 0;
                    moveNodes.Reverse();
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
        }
        else {
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
    //void OnDrawGizmos() {
    //    Gizmos.color = Color.red;
    //    for (int i = 0; i < moveNodes.Count; i++) {
    //        if (i != 0) {
    //            Gizmos.DrawSphere(moveNodes[i].transform.position, 1f);
    //            Gizmos.DrawLine(moveNodes[i - 1].transform.position, moveNodes[i].transform.position);
    //        }
    //    }
    //    Gizmos.color = Color.green;

    //    for (int i = 0; i < frontier.Count(); i++) {
    //        if (i != 0) {
    //            Gizmos.DrawSphere(frontier.queue[i].Item1.transform.position, 0.8f);
    //        }
    //    }
    //}
}
