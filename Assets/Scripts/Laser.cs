using UnityEngine;
using System.Collections;



public class Laser : MonoBehaviour {
	public Transform Alarm;
	public float six = 1;
	// Use this for initialization
	void Start () {
		Debug.Log (six);
	}
	
	// Update is called once per frame
	void Update () {

	



	}


	void OnTriggerEnter2D(Collider2D collider) {

	

		Debug.Log ("Collision: " + collider.name);
		if (collider.tag == "Player") 
		{
			collider.gameObject.SendMessage("AlarmOn",null,SendMessageOptions.DontRequireReceiver);

            GetComponent<SpriteRenderer>().color = Color.blue;
 
			SpriteRenderer renderer = Alarm.GetComponent<SpriteRenderer>();
			renderer.color = Color.red;
		}


	}
}
