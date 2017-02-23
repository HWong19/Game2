using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletController : MonoBehaviour {

	public float bulletspeed;
	public float maxDistance;
	public bool canPassThroughEnemies;

	Vector3 startPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;

	}

	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (0f, 0f, bulletspeed) * Time.deltaTime);
		Vector3 travelDistance = startPos - transform.position;
		if (travelDistance.magnitude > maxDistance) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter( Collider other)
	{
		if (other.gameObject.CompareTag ("Zombie") || other.gameObject.CompareTag("Big Zombie") || other.gameObject.CompareTag("Freezing Zombie")) {
			other.gameObject.SendMessage ("TakeDamage");
			if (!canPassThroughEnemies) {
				Destroy (gameObject);
			}
		} else if (other.gameObject.CompareTag ("Player")) {
			other.gameObject.SendMessage ("PlayerShot");
			Destroy (gameObject);
		} else if (other.gameObject.CompareTag ("Wall") || other.gameObject.CompareTag("Floor")) {
			Destroy (gameObject);
		}
	}
}
