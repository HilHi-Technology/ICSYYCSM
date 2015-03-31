using UnityEngine;
using System.Collections;

public class Alarm : MonoBehaviour {


	// Use this for initialization
	void Start () {



	}
	
	// Update is called once per frame
	void AlarmOn() {

		//GetComponent<SpriteRenderer> ().color = Color.red;
		//SpriteRenderer renderer = Laser.GetComponent<SpriteRenderer>();
		//renerer.color = Color.blue;
		GetComponent<SpriteRenderer> ().color = Color.red;

	}
}
