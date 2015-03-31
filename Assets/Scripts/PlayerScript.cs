using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
    Vector2 playerPos;
    public LayerMask lightLayerMask;
    public LayerMask badBoxMask;
    public int rayCastAmnt; //Amount of rays being cast to check for visibility
    public float visionRange;
    public float shadowLength;
    public float shadowOffset;
    public Transform badBox;
    private bool badBoxHit;
    public Transform youWon;
    private GUIText youWonText;
    public float speed; //How fast the player moves


    // Use this for initialization
	void Start () {
        Application.targetFrameRate = 1;
        youWonText = youWon.GetComponent<GUIText>();
	}

    


    void Update() {
        //Selecting color slots to determine which color will be swap out when player changes color.
        //Casting rays and creating dynamic shadows
        playerPos = new Vector2(transform.position.x, transform.position.y); //Changing player position into a vector2 instead of v3
        Vector2 prevPoint1 = new Vector2(-9999, -9999); //Previous points are used to keep track of previous ray hits in order to draw quads
        Vector2 prevPoint2 = new Vector2(-9999, -9999); //Initialized as -9999 because you can't initialize it as null
        Vector2 startPoint1 = new Vector2(-9999, -9999); //The first ray hit position. Used to connect the last ray hit and the first ray hit together
        Vector2 startPoint2 = new Vector2(-9999, -9999);
        badBoxHit = false;
        //for (int i = 0; i < rayCastAmnt; i++) //Iterate through all the rays
        //{
        //    float angle = (float)i / (float)rayCastAmnt * 360; //Figure out what angle the ray is in degree.
        //    float cos = Mathf.Cos(angle * Mathf.Deg2Rad); //Figure out the cosine of the angle.  Also turn the angle into radians because mathf.sin/cos only take radians
        //    float sin = Mathf.Sin(angle * Mathf.Deg2Rad); //Figure out the sine of the angle.
        //    Vector2 direction = new Vector2(cos, sin); //Figure out the direction of the raycast (in vector form, e.g. 30deg would be (cos(30),sin(30))).
        //    RaycastHit2D rayhit = Physics2D.Raycast(transform.position, direction, visionRange, lightLayerMask); //Cast the ray and store the hit info
        //    RaycastHit2D badBoxRayHit = Physics2D.Raycast(transform.position, direction, visionRange, badBoxMask); //Cast the ray and store the hit info
        //    Vector2 hitPoint;
        //    if (rayhit.transform == null) { //check if the ray hits nothing
        //        hitPoint = new Vector2(cos * visionRange, sin * visionRange) + playerPos;
        //    }
        //    else { //If the ray hits something
        //        hitPoint = rayhit.point;
        //    }

        //    //Renderer ren = obj.transform.GetComponent<Renderer>();
        //    //ren.enabled = true;
        //    Vector2 curPoint1 = hitPoint + new Vector2(cos * shadowOffset, sin * shadowOffset); //Ray hit point + shadow offset
        //    Vector2 curPoint2 = hitPoint + new Vector2(cos * shadowLength, sin * shadowLength); //Ray hit point + shadow length

        //    if (prevPoint1 == new Vector2(-9999, -9999) && prevPoint2 == new Vector2(-9999, -9999)) { //If "uninitialized"
        //        prevPoint1 = curPoint1;
        //        prevPoint2 = curPoint2;
        //        startPoint1 = curPoint1; //Set first ray hit point, so we can connect the first and last together later
        //        startPoint2 = curPoint2;
        //        continue;
        //    }
        //    if (badBoxRayHit.collider != null) {
        //        if (badBoxRayHit.collider.tag == "Enemy") {
        //            badBoxHit = true;
        //            //Debug.Log("loL");
        //        }
        //    }

        //    //Draw the shadows
        //    DrawScript.drawList.Add(prevPoint1);
        //    DrawScript.drawList.Add(curPoint1);
        //    DrawScript.drawList.Add(curPoint2);
        //    DrawScript.drawList.Add(prevPoint2);

        //    prevPoint1 = curPoint1;
        //    prevPoint2 = curPoint2;

        //    if (i == rayCastAmnt - 1) { //If the last ray is casted, draw a shadow connecting the first and last shadow together
        //        DrawScript.drawList.Add(startPoint1);
        //        DrawScript.drawList.Add(curPoint1);
        //        DrawScript.drawList.Add(curPoint2);
        //        DrawScript.drawList.Add(startPoint2);
        //    }
        //}
        //if (!badBoxHit) {
        //    youWonText.enabled = true;
        //    Time.timeScale = 0;
        //}
    }
	



	// Update is called once every 0.2 seconds (not every frame)
    void FixedUpdate() {

        //Get keyboard input for the movement and calculate the player's velocity
        float horMove = Input.GetAxisRaw("Horizontal") * speed;
        float verMove = Input.GetAxisRaw("Vertical") * speed;
        GetComponent<Rigidbody2D>().velocity = new Vector2(horMove, verMove); //Set the velocity


    }





		
		
	}













