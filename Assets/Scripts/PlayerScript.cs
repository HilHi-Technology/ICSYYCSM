﻿using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
    Vector2 playerPos;
    public int rayCastAmnt; //Amount of rays being cast to check for visibility
    public float visionRange;
    public float shadowLength;
    public float shadowOffset;
    public Transform badBox;
    private bool badBoxHit;
    public float speed; //How fast the player moves
    private Renderer renderer;
    static public bool playerDiedThisLevel = false;

    // Use this for initialization
	void Start () {
        Cursor.visible = false;
        renderer = GetComponent<Renderer>();
        Application.targetFrameRate = -1;
        
	}

    


    void Update() {
        playerPos = new Vector2(transform.position.x, transform.position.y); //Changing player position into a vector2 instead of v3
        //Debug.Log(renderer.isVisible);
    }
	



	// Update is called once every 0.2 seconds (not every frame)
    void FixedUpdate() {

        //Get keyboard input for the movement and calculate the player's velocity
        float horMove = Input.GetAxisRaw("Horizontal") * speed;
        float verMove = Input.GetAxisRaw("Vertical") * speed;
        GetComponent<Rigidbody2D>().velocity = new Vector2(horMove, verMove); //Set the velocity


    }

    void Blink() {

    }




		
		
	}













