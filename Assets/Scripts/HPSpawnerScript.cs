using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSpawnerScript : MonoBehaviour {

	public float spawnFrequency;
	public GameObject healthPack;

	bool hasSpawned;
	float lastSpawned;
	// Use this for initialization
	void Start () {
		hasSpawned = false;
		lastSpawned = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!hasSpawned) {
			if (Time.time - lastSpawned > spawnFrequency) {
				healthPack.transform.position = transform.position + new Vector3 (0f, 1.5f, 0f);
				Instantiate (healthPack);
				hasSpawned = true;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player")){
			hasSpawned = false;
			lastSpawned = Time.time;
		}
	}
}
