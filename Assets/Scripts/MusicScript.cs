using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {
    /*Delete any other music objects not this object*/
	// Use this for initialization
	void Start () {
        GameObject[] musics = GameObject.FindGameObjectsWithTag("Music");

        foreach (GameObject m in musics) {
            if (m != this.gameObject) {
                Destroy(m);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
