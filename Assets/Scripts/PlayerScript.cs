using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
    Vector2 playerPos;
    public float speed; //How fast the player moves

    // Use this for initialization
	void Start () {

	}

    


    void Update() {
        //Selecting color slots to determine which color will be swap out when player changes color.
    
    }
	



	// Update is called once every 0.2 seconds (not every frame)
    void FixedUpdate() {

        //Get keyboard input for the movement and calculate the player's velocity
        float horMove = Input.GetAxisRaw("Horizontal") * speed;
        float verMove = Input.GetAxisRaw("Vertical") * speed;
        rigidbody2D.velocity = new Vector2(horMove, verMove); //Set the velocity
    }
}
