using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerScript : MonoBehaviour {

	public GameObject player;
	public GameObject zombie;
	public Text zombieCountText;
	public int spawnCap;
	public float xscreen;
	public float zscreen;
	public float fastestSpawnFrequency;		//fastest spawn rate of enemies
	public float frequencyIncrement;		//how much delay to spawn time added every time a new zombie is added;

	int spawned;
	float spawnFrequency;
	// Use this for initialization
	void Start () {
		StartCoroutine (SpawnEnemy());
		spawned = 0;
		spawnFrequency = fastestSpawnFrequency;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player.transform.position;
		zombieCountText.text = "Enemies Alive: " + spawned;
		spawnFrequency = fastestSpawnFrequency + (frequencyIncrement * spawned);
	}

	IEnumerator SpawnEnemy()
	{
		yield return new WaitForSecondsRealtime (4f);
		while(true)
		{
			yield return new WaitForSeconds (spawnFrequency);
			if (spawned < spawnCap && player.activeInHierarchy) {
				float x;
				float z;
				float quadr = Random.value;
				if (quadr < 0.25f) {				//up
					x = Random.Range (-xscreen, xscreen);
					z = Random.Range (zscreen, zscreen + 20);
				} else if (quadr < 0.5f) {
					x = Random.Range (-xscreen, xscreen);
					z = Random.Range (-zscreen, -zscreen - 20);
				} else if (quadr < 0.75f) {
					x = Random.Range (xscreen, xscreen + 20);
					z = Random.Range (-zscreen, zscreen);
				} else {
					x = Random.Range (-xscreen, -xscreen - 20);
					z = Random.Range (-zscreen, zscreen);
				}
				if (gameObject.transform.position.x + x < 120 && gameObject.transform.position.x + x > -120 && gameObject.transform.position.z + z < 120 && gameObject.transform.position.z + z > -120)
				{
					InstantiateEnemy (x,z);
					++spawned;
				}
			}
		}
	}

	void ZombieShot()
	{
		--spawned;
	}

	void InstantiateEnemy(float x, float z)
	{
		Instantiate (zombie, gameObject.transform.position + new Vector3 (x, 0f, z), new Quaternion ());
	}

	void IncreaseSpawnRate()
	{
		fastestSpawnFrequency = fastestSpawnFrequency * 0.8f;
		frequencyIncrement = frequencyIncrement * 0.8f;
	}
}
