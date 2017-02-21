using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ZombieController : MonoBehaviour {


	public float speed;
	public float maxSpeed;
	public GameObject healthPack;
	public int maxHealth;

	GameObject player;
	Rigidbody rb;
	Animator animator;
	GameObject controller;
	GameObject spawner;
	int health;
	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		animator = GetComponent<Animator> ();
		animator.SetBool ("Attack", false);
		controller = GameObject.FindGameObjectWithTag ("GameController");
		spawner = GameObject.FindGameObjectWithTag ("Spawner");
		health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate()
	{
		if (player.activeInHierarchy) {
			Vector3 playerPos = player.transform.position;
			playerPos.y = 1f;
			transform.LookAt (playerPos);
			if (rb.velocity.magnitude < maxSpeed) {
				rb.AddRelativeForce (new Vector3 (0f, 0f, 1f) * speed);
			}
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag ("Player")) {
			animator.SetBool ("Attack", true);
		}
	}

	void OnCollisionExit(Collision col)
	{
		if (col.gameObject.CompareTag ("Player")) {
			animator.SetBool ("Attack", false);
		}
	}

	void OnDestroy()
	{

	}

	void TakeDamage()
	{
		--health;
		if (health == 0) {
			Destroy (gameObject);
			for(int i = 0; i < maxHealth; ++i)
			{
				controller.SendMessage ("IncScore");
			}
			spawner.SendMessage ("ZombieShot");
			if (Random.value > 0.97f)
			{
				healthPack.transform.position = transform.position + new Vector3 (0f, 1f, 0f);
				Instantiate(healthPack);
			}
		}
	}
}
