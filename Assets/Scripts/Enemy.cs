using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    private string state;
    private float timer; //Timer varaible
    public float battle_move_frequency; //How often enemy moves around in battle mode (in seconds)
    public float battle_move_range; //How far can the enemy move randomly
    private Vector2 destination;
    private bool movement_chosen = false; //Determines whether destination has already been chosen for that move cycle
    public float speed;
    public GameObject player;
    public float shoot_delay; //The delay after stopping to shoot
    public float after_shoot_delay; //The delay after shooting to move again
    private bool shot = false; //Whether object has shot anything yet
    public GameObject arrow;
    public float arrow_shoot_speed;
    private bool entered_view = false; //Has enemy entered the camera view yet?
    public float accuracy; //How much the enemy can miss by in degrees
    // Use this for initialization
    void Start() {
        timer = Time.time;
        state = "entering"; //Starting state
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update() {
        if (state == "entering") { //Move downward when first entering the game
            look_at(gameObject, transform.position - Vector3.up);
            move_to(transform.position - Vector3.up, speed);
        }
        else if (state == "moving") {
            look_at(gameObject, player.transform.position);
            if (Time.time - timer > battle_move_frequency) { //If it's time to move again
                //Movement  
                if (!movement_chosen) {
                    //Choose a destination
                    Vector2 d = Random.insideUnitCircle * battle_move_range;
                    //Debug.Log(d.magnitude);
                    destination = new Vector2(transform.position.x + d.x, transform.position.y + d.y);
                    Vector2 viewport_destination = Camera.main.WorldToViewportPoint(destination);//Destination coordinate in viewport coordinate

                    while (viewport_destination.x < 0 || viewport_destination.x > 1 || viewport_destination.y < 0 || viewport_destination.y > 1) { //If destination is out of boundary
                        //Repick destination
                        d = Random.insideUnitCircle * battle_move_range;
                        destination = new Vector2(transform.position.x + d.x, transform.position.y + d.y);
                        viewport_destination = Camera.main.WorldToViewportPoint(destination);//Destination coordinate in viewport coordinate
                    }

                    movement_chosen = true;

                }
                if (move_to(destination, speed)) {
                    movement_chosen = false;
                    timer = Time.time; //Timer reset
                    state = "shooting"; //Prepare to shoot
                }
            }
        }

        else if (state == "shooting") {
            look_at(gameObject, player.transform.position);
            if (!shot) {
                if (Time.time - timer > shoot_delay) { //If it's time to move again
                    //Shooting
                    GameObject t_arrow = Instantiate(arrow, transform.position, transform.rotation) as GameObject;
                    Quaternion rotation = t_arrow.transform.rotation; //Shortener
                    rotation = Quaternion.Euler(new Vector3(0, 0, (rotation.eulerAngles.z - accuracy / 2) + Random.value * accuracy));
                    t_arrow.transform.rotation = rotation;
                    t_arrow.rigidbody2D.velocity = t_arrow.transform.right * arrow_shoot_speed;

                    timer = Time.time;
                    shot = true;
                }
            }
            if (shot) {
                if (Time.time - timer > after_shoot_delay) { //After cooldown period
                    shot = false;
                    timer = Time.time;
                    state = "moving";
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
    //void OnBecameInvisible() {
    //    state = "moving";
    //}
    void OnWillRenderObject() {
        if (Camera.current == Camera.main && !entered_view) {
            state = "moving";
            entered_view = true;
        }
    }
}
