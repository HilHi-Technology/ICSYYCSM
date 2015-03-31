using UnityEngine;              //I dont know what this is
using System.Collections;        //No idea what this is, its  a pretty common theme
 


public class Laser : MonoBehaviour {         //Still no idea
/// T/// </summary>
	public Transform Alarm;         //Gets acess to the alarm of something

	// Use this for initialization 
	void Start () {
		          //Supposed to print six, but eh
	}
	
	// Update is called once per frame
	void Update () {

	



	}


	void OnTriggerEnter2D(Collider2D collider) {               //Calls for update when its run into

	

		//Debug.Log ("Collision: " + collider.name + "#triggered");                                              //Prints out Collsion, the name of the tag that collided, and #trigger
		if (collider.tag == "Player") 
		{
			collider.gameObject.SendMessage("AlarmOn",null,SendMessageOptions.DontRequireReceiver);            //sends out a message

            GetComponent<SpriteRenderer>().color = Color.blue;                                                //Turns the laser blue
 
			SpriteRenderer renderer = Alarm.GetComponent<SpriteRenderer>();          
			renderer.color = Color.red;                                                                       //Turns the alarm red
		}


	}
}
