using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector2 SS = gameObject.GetComponent<Renderer>().material.mainTextureScale;
		SS.x = transform.localScale.x;
		SS.y = transform.localScale.y;
		gameObject.GetComponent<Renderer>().material.mainTextureScale = SS;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
